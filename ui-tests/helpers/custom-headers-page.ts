import { Page, Response, expect } from '@playwright/test';
import { ContextSwitcher, dialog, expectToast } from './axiom';

interface ScopeContext {
  appId: string | null;
  hostName: string | null;
  isInherited: boolean;
}

export class CustomHeadersPage {
  private context: ScopeContext = { appId: null, hostName: null, isInherited: false };
  private readonly contextSwitcher: ContextSwitcher;

  constructor(private readonly page: Page, private readonly cmsUrl: string) {
    this.contextSwitcher = new ContextSwitcher(page);
  }

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#response-headers`);
    await expect(this.contextSwitcher.switchButton).toBeVisible();
  }

  /**
   * Resolves once the override-status and header list requests for the given scope have completed.
   * Both fire after a context switch or an override change, and the UI only reflects the scope's
   * true inherited state once the status request has returned.
   */
  private waitForScopeLoad(appId: string, hostName: string | null): Promise<[Response, Response]> {
    const isFor = (response: Response, path: string): boolean => {
      const url = new URL(response.url());
      return url.pathname.includes(path)
        && url.searchParams.get('appId') === appId
        && url.searchParams.get('hostName') === hostName;
    };

    return Promise.all([
      this.page.waitForResponse((response) => isFor(response, '/customheader/override/exists')),
      this.page.waitForResponse((response) => isFor(response, '/customheader/list')),
    ]);
  }

  async switchToGlobal(): Promise<void> {
    await this.contextSwitcher.select('All Applications', 'All Applications');
    this.context = { appId: null, hostName: null, isInherited: false };
  }

  async switchToApplication(appDisplayName: string, expectedAppId: string): Promise<void> {
    const [status] = (await this.contextSwitcher.select(appDisplayName, expectedAppId, () => this.waitForScopeLoad(expectedAppId, null)))!;
    this.context = { appId: expectedAppId, hostName: null, isInherited: (await status.json()).isInherited === true };
  }

  async switchToHost(hostDisplayName: string, expectedAppId: string, expectedHostName: string): Promise<void> {
    const [status] = (await this.contextSwitcher.select(
      hostDisplayName,
      `${expectedAppId} - ${expectedHostName}`,
      () => this.waitForScopeLoad(expectedAppId, expectedHostName),
    ))!;
    this.context = { appId: expectedAppId, hostName: expectedHostName, isInherited: (await status.json()).isInherited === true };
  }

  /**
   * If the current scope is inherited, click Create Override so headers can be added.
   * No-op at global scope or when the scope already has an override.
   *
   * The decision is made from the status response captured by switchToApplication/switchToHost
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
    const [status] = await scopeReloaded;
    expect((await status.json()).isInherited, 'Create Override did not produce an override for the current scope').toBe(false);

    await expect(this.page.getByRole('button', { name: 'Revert to Inherited' })).toBeVisible({ timeout: 10_000 });
    this.context.isInherited = false;
  }

  /**
   * Open the Add Custom Header dialog, leave behavior as Add (the default), fill the
   * name and value fields, save, and wait for the success toast.
   */
  async addHeader(headerName: string, headerValue: string): Promise<void> {
    await this.page.getByRole('button', { name: 'Add Header' }).click();

    const modal = dialog(this.page, 'Add Custom Header');
    await expect(modal).toBeVisible();

    await modal.getByRole('textbox', { name: 'Header Name' }).fill(headerName);

    // Behavior defaults to Add for new headers, no change required. The
    // value input only renders while behavior is Add.
    await modal.getByRole('textbox', { name: 'Header Value' }).fill(headerValue);

    await modal.getByRole('button', { name: 'Save' }).click();
    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expectToast(this.page, 'Custom header has been successfully saved.');
  }
}
