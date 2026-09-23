import { Page, Response, expect } from '@playwright/test';

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

interface ScopeContext {
  appId: string | null;
  hostName: string | null;
  isInherited: boolean;
}

export class CustomHeadersPage {
  private context: ScopeContext = { appId: null, hostName: null, isInherited: false };

  constructor(private readonly page: Page, private readonly cmsUrl: string) {}

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#response-headers`);
    await expect(this.page.getByRole('button', { name: 'Switch Context' })).toBeVisible();
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
    const [status] = await scopeLoaded;
    this.context = { appId: expectedAppId, hostName: null, isInherited: (await status.json()).isInherited === true };
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
    const [status] = await scopeLoaded;
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
   * Open the Add Custom Header modal, set behavior=Add (the default), fill the
   * name and value fields, save, and wait for the success toast.
   */
  async addHeader(headerName: string, headerValue: string): Promise<void> {
    await this.page.getByRole('button', { name: 'Add Header' }).click();

    const modal = this.page.locator('.modal.show', { hasText: 'Add Custom Header' });
    await expect(modal).toBeVisible();

    // Header Name input — placeholder identifies it uniquely within the modal.
    await modal.locator('input[placeholder*="X-Permitted"]').fill(headerName);

    // Behavior defaults to 1 (Add) for new headers, no change required. The
    // value input only renders while behavior === 1.
    await modal.locator('input[placeholder="e.g., none"]').fill(headerValue);

    await modal.getByRole('button', { name: 'Save' }).click();
    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.getByText('Custom header has been successfully saved.', { exact: false })).toBeVisible({ timeout: 10_000 });
  }

  /**
   * Click Delete on the card whose header name matches, then confirm in the
   * generic ConfirmationModal.
   */
}
