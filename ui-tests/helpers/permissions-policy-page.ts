import { Page, expect } from '@playwright/test';

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

export type PermissionPolicyEnabledState =
  | 'Disabled'
  | 'None'
  | 'All'
  | 'ThisSite'
  | 'ThisAndSpecificSites'
  | 'SpecificSites';

interface ApplicationApiResponse {
  appId: string | null;
  appName: string;
  availableHosts?: Array<{ hostName?: string; displayName?: string }>;
  hasMultipleHosts?: boolean;
}

export class PermissionsPolicyPage {
  constructor(private readonly page: Page, private readonly cmsUrl: string) {}

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#permissions-policy`);
    await expect(this.page.getByRole('button', { name: 'Switch Context' })).toBeVisible();
  }

  private async openContextModal() {
    await this.page.getByRole('button', { name: 'Switch Context' }).click();
    const modal = this.page.locator('.modal.show', { hasText: 'Select Application Context' });
    await expect(modal).toBeVisible();
    return modal;
  }

  async switchToGlobal(): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: /^All Applications$/ }),
    }).first();
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    await row.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText('All Applications');
  }

  async switchToApplication(appDisplayName: string, expectedAppId: string): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: new RegExp(`^${escapeRegExp(appDisplayName)}$`) }),
    }).first();
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    await row.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(expectedAppId);
  }

  async switchToHost(hostDisplayName: string, expectedAppId: string, expectedHostName: string): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal
      .locator('.list-group-item', { hasText: hostDisplayName })
      .filter({ hasText: 'Host-level configuration' })
      .first();
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    await row.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(`${expectedAppId} - ${expectedHostName}`);
  }

  /**
   * If the current scope shows the "inherited" alert with a Create Override button, click it
   * so individual directive cards become editable. No-op when the scope already has an override.
   */
  async ensureOverrideExists(): Promise<void> {
    const createOverride = this.page.getByRole('button', { name: 'Create Override' });
    if (await createOverride.isVisible()) {
      await createOverride.click();
      await expect(this.page.getByRole('button', { name: 'Revert to Inherited' })).toBeVisible({ timeout: 10_000 });
    }
  }

  /**
   * If the current scope has its own override, click "Revert to Inherited" to delete it.
   * Returns true if a revert was performed.
   */
  async revertToInheritedIfPresent(): Promise<boolean> {
    const revert = this.page.getByRole('button', { name: 'Revert to Inherited' });
    if (await revert.isVisible()) {
      await revert.click();
      await expect(this.page.getByRole('button', { name: 'Create Override' })).toBeVisible({ timeout: 10_000 });
      return true;
    }
    return false;
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

  /**
   * Walk every application + host scope exposed by the live applications API
   * and revert any override that exists. Used by the beforeEach in each
   * Permissions Policy spec to start from a guaranteed clean slate.
   *
   * Driving the sweep from the API means we only visit scopes that genuinely
   * exist in the seeded sites — no risk of hanging on a hardcoded host that
   * the modal doesn't render.
   */
  async revertAllOverrides(): Promise<void> {
    const url = `${this.cmsUrl}/stott.security.optimizely/api/applications`;
    const response = await this.page.request.get(url, { ignoreHTTPSErrors: true });
    if (!response.ok()) {
      throw new Error(`Failed to fetch applications list (${response.status()}): ${url}`);
    }
    const apps: ApplicationApiResponse[] = await response.json();

    for (const app of apps) {
      if (!app.appId) continue; // skip the synthetic "All Applications" entry
      await this.switchToApplication(app.appName, app.appId);
      await this.revertToInheritedIfPresent();

      if (app.hasMultipleHosts && app.availableHosts) {
        for (const host of app.availableHosts) {
          if (!host.hostName || !host.displayName) continue; // skip the "Default" placeholder
          await this.switchToHost(host.displayName, app.appId, host.hostName);
          await this.revertToInheritedIfPresent();
        }
      }
    }

    await this.switchToGlobal();
  }
}
