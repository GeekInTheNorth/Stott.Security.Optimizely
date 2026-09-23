import { Page, Response, expect } from '@playwright/test';

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

interface ScopeContext {
  appId: string | null;
  hostName: string | null;
  isInherited: boolean;
}

export type PermissionPolicyEnabledState =
  | 'Disabled'
  | 'None'
  | 'All'
  | 'ThisSite'
  | 'ThisAndSpecificSites'
  | 'SpecificSites';

export class PermissionsPolicyPage {
  private context: ScopeContext = { appId: null, hostName: null, isInherited: false };

  constructor(private readonly page: Page, private readonly cmsUrl: string) {}

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#permissions-policy`);
    await expect(this.page.getByRole('button', { name: 'Switch Context' })).toBeVisible();
  }

  /**
   * Resolves once the settings and directive list requests for the given scope have completed.
   * Both fire after a context switch or an override change, and the UI only reflects the scope's
   * true inherited state once the settings request has returned.
   */
  private waitForScopeLoad(appId: string, hostName: string | null): Promise<[Response, Response]> {
    const isFor = (response: Response, path: string): boolean => {
      const url = new URL(response.url());
      return url.pathname.includes(path)
        && url.searchParams.get('appId') === appId
        && url.searchParams.get('hostName') === hostName;
    };

    return Promise.all([
      this.page.waitForResponse((response) => isFor(response, '/permission-policy/settings/get')),
      this.page.waitForResponse((response) => isFor(response, '/permission-policy/source/list')),
    ]);
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
    this.context = { appId: null, hostName: null, isInherited: false };
  }

  async switchToApplication(appDisplayName: string, expectedAppId: string): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: new RegExp(`^${escapeRegExp(appDisplayName)}$`) }),
    }).first();
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    const scopeLoaded = this.waitForScopeLoad(expectedAppId, null);
    await row.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(expectedAppId);
    const [settings] = await scopeLoaded;
    this.context = { appId: expectedAppId, hostName: null, isInherited: (await settings.json()).isInherited === true };
  }

  async switchToHost(hostDisplayName: string, expectedAppId: string, expectedHostName: string): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal
      .locator('.list-group-item', { hasText: hostDisplayName })
      .filter({ hasText: 'Host-level configuration' })
      .first();
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    const scopeLoaded = this.waitForScopeLoad(expectedAppId, expectedHostName);
    await row.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(`${expectedAppId} - ${expectedHostName}`);
    const [settings] = await scopeLoaded;
    this.context = { appId: expectedAppId, hostName: expectedHostName, isInherited: (await settings.json()).isInherited === true };
  }

  /**
   * If the current scope is inherited, click Create Override so individual directive cards
   * become editable. No-op at global scope or when the scope already has an override.
   *
   * The decision is made from the settings response captured by switchToApplication/switchToHost
   * rather than from the DOM: the UI's inherited flag defaults to false until that response lands,
   * so "Revert to Inherited" can briefly show for a scope which is in fact inherited.
   */
  async ensureOverrideExists(): Promise<void> {
    if (!this.context.isInherited || this.context.appId === null) {
      return;
    }

    const createOverride = this.page.getByRole('button', { name: 'Create Override' });
    await expect(createOverride).toBeVisible({ timeout: 10_000 });

    const scopeReloaded = this.waitForScopeLoad(this.context.appId, this.context.hostName);
    await createOverride.click();
    const [settings] = await scopeReloaded;
    expect((await settings.json()).isInherited, 'Create Override did not produce an override for the current scope').toBe(false);

    await expect(this.page.getByRole('button', { name: 'Revert to Inherited' })).toBeVisible({ timeout: 10_000 });
    this.context.isInherited = false;
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
   * Locate the directive card whose header title matches `directiveTitle` exactly.
   * The card header also holds the Deprecated badge, so the title is matched against
   * its own element rather than the header's full text.
   */
  private directiveCard(directiveTitle: string) {
    return this.page.locator('.card', {
      has: this.page.locator('.card-header span', { hasText: new RegExp(`^${escapeRegExp(directiveTitle)}$`) }),
    }).first();
  }

  /**
   * Assert the directive is offered in the list. Applies the "All Directives" filter first
   * so the assertion is not confused by the enabled-state filter.
   */
  async expectDirectiveListed(directiveTitle: string): Promise<void> {
    await this.ensureAllDirectivesFilter();
    await expect(this.directiveCard(directiveTitle)).toBeVisible();
  }

  /**
   * Assert the directive is not offered in the list at all. Used for deprecated directives,
   * which are only surfaced when they already hold a stored configuration.
   */
  async expectDirectiveNotListed(directiveTitle: string): Promise<void> {
    await this.ensureAllDirectivesFilter();
    // Wait for the list to render before asserting an absence, otherwise the assertion
    // can pass against an empty list.
    await expect(this.directiveCard('Geolocation')).toBeVisible();
    await expect(this.directiveCard(directiveTitle)).toHaveCount(0);
  }

  /**
   * Assert the directive is listed and carries the Deprecated badge.
   */
  async expectDirectiveDeprecated(directiveTitle: string): Promise<void> {
    await this.ensureAllDirectivesFilter();
    const card = this.directiveCard(directiveTitle);
    await expect(card).toBeVisible();
    await expect(card.locator('.card-header .badge', { hasText: 'Deprecated' })).toBeVisible();
  }

  /**
   * Assert the directive is listed and does not carry the Deprecated badge.
   */
  async expectDirectiveNotDeprecated(directiveTitle: string): Promise<void> {
    await this.ensureAllDirectivesFilter();
    const card = this.directiveCard(directiveTitle);
    await expect(card).toBeVisible();
    await expect(card.locator('.card-header .badge')).toHaveCount(0);
  }

  /**
   * Open the Edit modal for the directive whose card title matches `directiveTitle`,
   * set the enabled-state dropdown to `state`, fill specific-source rows where applicable,
   * Save, and wait for the success toast.
   */
  async setDirective(directiveTitle: string, state: PermissionPolicyEnabledState, sources: string[] = []): Promise<void> {
    await this.ensureAllDirectivesFilter();

    const card = this.directiveCard(directiveTitle);
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

    // Saving triggers a debounced refresh of the directive list, which re-orders the cards.
    // Wait for that refresh to land before returning so the next edit starts from a settled list.
    const listRefreshed = this.page.waitForResponse((response) => response.url().includes('/permission-policy/source/list'));
    await modal.getByRole('button', { name: 'Save' }).click();
    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.getByText('Permission Policy Settings have been successfully saved.', { exact: false })).toBeVisible({ timeout: 10_000 });
    await listRefreshed;
  }
}
