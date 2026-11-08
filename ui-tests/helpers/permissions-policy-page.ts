import { Locator, Page, Response, expect } from '@playwright/test';
import { ContextSwitcher, combobox, comboboxValue, dialog, expectToast, selectOption } from './axiom';

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

/** Visible option labels for each enabled state in the directive edit dialog. */
const enabledStateLabels: Record<PermissionPolicyEnabledState, string> = {
  Disabled: 'Disabled',
  None: 'Allow None',
  All: 'Allow all websites',
  ThisSite: 'Allow just this website',
  ThisAndSpecificSites: 'Allow this website and specific third party websites',
  SpecificSites: 'Allow specific third party websites',
};

export class PermissionsPolicyPage {
  private context: ScopeContext = { appId: null, hostName: null, isInherited: false };
  private readonly contextSwitcher: ContextSwitcher;

  constructor(private readonly page: Page, private readonly cmsUrl: string) {
    this.contextSwitcher = new ContextSwitcher(page);
  }

  /** The page body below the context switcher. Dialogs render in a portal outside of it. */
  private get container(): Locator {
    return this.page.locator('.security-page-container');
  }

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#permissions-policy`);
    await expect(this.contextSwitcher.switchButton).toBeVisible();
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

  async switchToGlobal(): Promise<void> {
    await this.contextSwitcher.select('All Applications', 'All Applications');
    this.context = { appId: null, hostName: null, isInherited: false };
  }

  async switchToApplication(appDisplayName: string, expectedAppId: string): Promise<void> {
    const [settings] = (await this.contextSwitcher.select(appDisplayName, expectedAppId, () => this.waitForScopeLoad(expectedAppId, null)))!;
    this.context = { appId: expectedAppId, hostName: null, isInherited: (await settings.json()).isInherited === true };
  }

  async switchToHost(hostDisplayName: string, expectedAppId: string, expectedHostName: string): Promise<void> {
    const [settings] = (await this.contextSwitcher.select(
      hostDisplayName,
      `${expectedAppId} - ${expectedHostName}`,
      () => this.waitForScopeLoad(expectedAppId, expectedHostName),
    ))!;
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
    const select = combobox(this.container, 'Permission Policy Header');
    await expect(select).toBeEnabled();
    if (await comboboxValue(select) === 'Enabled') {
      return;
    }
    await selectOption(this.page, select, 'Enabled');
    await this.page.locator('#btnSave').click();
    await expectToast(this.page, 'Permissions Policy Settings have been successfully saved.');
  }

  /**
   * Set the "Configuration" directive filter to "All Directives". The default filter
   * hides directives in some enabled states, which would prevent setDirective from
   * locating cards for directives currently in those states. Idempotent.
   */
  async ensureAllDirectivesFilter(): Promise<void> {
    const filter = combobox(this.container, 'Configuration');
    await expect(filter).toBeVisible();
    await selectOption(this.page, filter, 'All Directives');
  }

  /**
   * Locate the directive card whose heading matches `directiveTitle` exactly. The card is
   * the nearest ancestor of the heading that holds the description paragraph; the header
   * row wrapping the heading (and the Deprecated badge) has no paragraph of its own.
   */
  private directiveCard(directiveTitle: string): Locator {
    return this.container
      .getByRole('heading', { name: directiveTitle, exact: true })
      .locator('xpath=ancestor::div[p][1]');
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
    await expect(card.getByText('Deprecated', { exact: true })).toBeVisible();
  }

  /**
   * Assert the directive is listed and does not carry the Deprecated badge.
   */
  async expectDirectiveNotDeprecated(directiveTitle: string): Promise<void> {
    await this.ensureAllDirectivesFilter();
    const card = this.directiveCard(directiveTitle);
    await expect(card).toBeVisible();
    await expect(card.getByText('Deprecated', { exact: true })).toHaveCount(0);
  }

  /**
   * Open the Edit dialog for the directive whose card title matches `directiveTitle`,
   * set the enabled state to `state`, fill specific-source rows where applicable,
   * Save, and wait for the success toast.
   */
  async setDirective(directiveTitle: string, state: PermissionPolicyEnabledState, sources: string[] = []): Promise<void> {
    await this.ensureAllDirectivesFilter();

    const card = this.directiveCard(directiveTitle);
    await expect(card).toBeVisible();
    await card.getByRole('button', { name: 'Edit' }).click();

    const modal = dialog(this.page, directiveTitle);
    await expect(modal).toBeVisible();

    await selectOption(this.page, combobox(modal, 'Enabled State'), enabledStateLabels[state]);

    if (state === 'ThisAndSpecificSites' || state === 'SpecificSites') {
      // Selecting either state auto-adds an empty source row when none exists.
      // Fill any existing inputs, then click "Add Source" for further sources.
      const sourceInputs = modal.getByPlaceholder('Enter a URL');
      for (let i = 0; i < sources.length; i++) {
        if (i >= await sourceInputs.count()) {
          await modal.getByRole('button', { name: 'Add Source' }).click();
        }
        await sourceInputs.nth(i).fill(sources[i]);
      }
    }

    // Saving triggers a refresh of the directive list, which re-orders the cards.
    // Wait for that refresh to land before returning so the next edit starts from a settled list.
    const listRefreshed = this.page.waitForResponse((response) => response.url().includes('/permission-policy/source/list'));
    await modal.getByRole('button', { name: 'Save' }).click();
    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expectToast(this.page, 'Permission Policy Settings have been successfully saved.');
    await listRefreshed;
  }
}
