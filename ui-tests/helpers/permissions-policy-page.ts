import { Locator, Page, expect } from '@playwright/test';

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

const SETTINGS_LOAD_PATH = '/stott.security.optimizely/api/permission-policy/settings/get';

export type PermissionPolicyEnabledState =
  | 'Disabled'
  | 'None'
  | 'All'
  | 'ThisSite'
  | 'ThisAndSpecificSites'
  | 'SpecificSites';

export class PermissionsPolicyPage {
  constructor(private readonly page: Page, private readonly cmsUrl: string) {}

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#permissions-policy`);
    await expect(this.page.getByRole('button', { name: 'Switch Context' })).toBeVisible();
  }

  private async openContextModal(): Promise<Locator> {
    await this.page.getByRole('button', { name: 'Switch Context' }).click();
    const modal = this.page.locator('.modal.show', { hasText: 'Select Site Context' });
    await expect(modal).toBeVisible();
    return modal;
  }

  /**
   * Clicks a row in the context modal and waits until the page has fully settled
   * in the new context:
   *  - the modal has finished its fade-out (while fading it still intercepts clicks);
   *  - for site/host scopes, the settings request (which carries isInherited) has
   *    returned and the matching banner button is rendered. Until then the page
   *    still shows the previous scope's inherited state.
   */
  private async selectContextRow(row: Locator, expectedLabel: string, contextSpecific: boolean): Promise<void> {
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();

    const settingsLoad = contextSpecific
      ? this.page.waitForResponse(r => r.request().method() === 'GET' && r.url().includes(SETTINGS_LOAD_PATH))
      : undefined;

    await row.click();

    await expect(this.page.locator('.modal', { hasText: 'Select Site Context' })).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(expectedLabel);

    if (settingsLoad) {
      const { isInherited } = await (await settingsLoad).json() as { isInherited: boolean };
      const expectedButton = isInherited ? 'Create Override' : 'Revert to Inherited';
      await expect(this.page.getByRole('button', { name: expectedButton })).toBeVisible({ timeout: 10_000 });
    }
  }

  async switchToGlobal(): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: /^All Sites$/ }),
    }).first();
    await this.selectContextRow(row, 'All Sites', false);
  }

  /** Selects the site-level row for `siteName`; the context label shows the site name. */
  async switchToSite(siteName: string): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: new RegExp(`^${escapeRegExp(siteName)}$`) }),
    }).first();
    await this.selectContextRow(row, siteName, true);
  }

  /**
   * Selects the host row for `hostName` (e.g. "localhost:5000") under a site; the
   * context label shows "<siteName> - <hostName>".
   */
  async switchToHost(hostName: string, siteName: string): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal
      .locator('.list-group-item', { hasText: hostName })
      .filter({ hasText: 'Host-level configuration' })
      .first();
    await this.selectContextRow(row, `${siteName} - ${hostName}`, true);
  }

  /**
   * If the current scope shows the "inherited" alert with a Create Override button, click it
   * so individual directive cards become editable. No-op when the scope already has an override.
   * Call after switchToSite/switchToHost, which guarantee the banner reflects the current scope.
   */
  async ensureOverrideExists(): Promise<void> {
    const createOverride = this.page.getByRole('button', { name: 'Create Override' });
    if (await createOverride.isVisible()) {
      await createOverride.click();
      await expect(this.page.getByRole('button', { name: 'Revert to Inherited' })).toBeVisible({ timeout: 10_000 });
    }
  }

  /**
   * Ensure the Permissions-Policy response header is enabled at the current scope.
   * Reads the dropdown; if already Enabled, this is a no-op. Otherwise selects Enabled
   * and clicks Save.
   */
  async ensureHeaderEnabled(): Promise<void> {
    const select = this.page.locator(`select[aria-describedby='lblEnabled']`);
    await expect(select).toBeEnabled();
    const current = await select.inputValue();
    if (current === 'true') {
      return;
    }
    await select.selectOption('true');
    await this.page.locator('#btnSave').click();
    await expect(this.page.getByText('Permission Policy Settings have been successfully saved.', { exact: false })).toBeVisible({ timeout: 10_000 });
  }

  /**
   * Set the "Configuration" directive filter to "All Directives". The default filter
   * hides directives in some enabled states, which would prevent setDirective from
   * locating cards for directives currently in those states. Idempotent.
   */
  async ensureAllDirectivesFilter(): Promise<void> {
    const filter = this.page.locator(`select[aria-describedby='lblSourceFilters']`);
    await expect(filter).toBeVisible();
    const current = await filter.inputValue();
    if (current !== 'All') {
      await filter.selectOption('All');
    }
  }

  /**
   * Open the Edit modal for the directive whose card title matches `directiveTitle`,
   * set the enabled-state dropdown to `state`, fill specific-source rows where applicable,
   * Save, and wait for the success toast.
   */
  async setDirective(directiveTitle: string, state: PermissionPolicyEnabledState, sources: string[] = []): Promise<void> {
    await this.ensureAllDirectivesFilter();

    const card = this.page.locator('.card', {
      has: this.page.locator('.card-header', { hasText: new RegExp(`^${escapeRegExp(directiveTitle)}$`) }),
    }).first();
    await expect(card).toBeVisible();
    await card.getByRole('button', { name: 'Edit' }).click();

    const modal = this.page.locator('.modal.show', {
      has: this.page.locator('.modal-header', { hasText: directiveTitle }),
    }).first();
    await expect(modal).toBeVisible();

    await modal.locator(`select[aria-describedby='lblEnabledState']`).selectOption(state);

    if (state === 'ThisAndSpecificSites' || state === 'SpecificSites') {
      // Selecting either state auto-adds an empty source row when none exists.
      // Fill any existing inputs (the first 'Allow' state click already added one),
      // then click "Add Source" for further sources.
      const sourceInputs = modal.locator('input[type="text"][placeholder]');
      for (let i = 0; i < sources.length; i++) {
        if (i >= await sourceInputs.count()) {
          await modal.getByRole('button', { name: 'Add Source' }).click();
        }
        await sourceInputs.nth(i).fill(sources[i]);
      }
    }

    await modal.getByRole('button', { name: 'Save' }).click();
    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.getByText('Permission Policy Settings have been successfully saved.', { exact: false })).toBeVisible({ timeout: 10_000 });
  }
}
