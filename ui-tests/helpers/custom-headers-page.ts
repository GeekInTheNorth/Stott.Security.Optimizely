import { Locator, Page, Response, expect } from '@playwright/test';

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

/**
 * CMS 12 keys scopes by the site's Guid, which the UI never renders. The Guid is
 * captured from the requests the UI issues after a context switch so later waits
 * can be pinned to the exact scope.
 */
interface ScopeContext {
  siteId: string | null;
  hostName: string | null;
  isInherited: boolean;
}

export class CustomHeadersPage {
  private context: ScopeContext = { siteId: null, hostName: null, isInherited: false };

  constructor(private readonly page: Page, private readonly cmsUrl: string) {}

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#response-headers`);
    await expect(this.page.getByRole('button', { name: 'Switch Context' })).toBeVisible();
  }

  /**
   * Resolves once the override-status and header list requests for the given scope have completed.
   * Both fire after a context switch or an override change, and the UI only reflects the scope's
   * true inherited state once the status request has returned.
   *
   * `siteId` is undefined when switching context (the Guid is not yet known); any site-scoped
   * request then matches. Pass the captured Guid to pin the wait to a known scope.
   */
  private waitForScopeLoad(hostName: string | null, siteId?: string): Promise<[Response, Response]> {
    const isFor = (response: Response, path: string): boolean => {
      const url = new URL(response.url());
      const requestSiteId = url.searchParams.get('siteId');
      return url.pathname.includes(path)
        && requestSiteId !== null
        && (siteId === undefined || requestSiteId === siteId)
        && url.searchParams.get('hostName') === hostName;
    };

    return Promise.all([
      this.page.waitForResponse((response) => isFor(response, '/customheader/override/exists')),
      this.page.waitForResponse((response) => isFor(response, '/customheader/list')),
    ]);
  }

  private async openContextModal(): Promise<Locator> {
    await this.page.getByRole('button', { name: 'Switch Context' }).click();
    const modal = this.page.locator('.modal.show', { hasText: 'Select Site Context' });
    await expect(modal).toBeVisible();
    return modal;
  }

  /**
   * The `.modal.show` locator stops matching as soon as the "show" class drops, but Bootstrap
   * keeps the dialog in the DOM during its fade-out and it still intercepts pointer events.
   */
  private async waitForContextModalClosed(): Promise<void> {
    await expect(this.page.locator('.modal', { hasText: 'Select Site Context' })).toBeHidden({ timeout: 10_000 });
  }

  private async selectScopedRow(row: Locator, expectedLabel: string, hostName: string | null): Promise<void> {
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    const scopeLoaded = this.waitForScopeLoad(hostName);
    await row.click();

    await this.waitForContextModalClosed();
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(expectedLabel);
    const [status] = await scopeLoaded;
    const siteId = new URL(status.url()).searchParams.get('siteId');
    this.context = { siteId, hostName, isInherited: (await status.json()).isInherited === true };
  }

  async switchToGlobal(): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: /^All Sites$/ }),
    }).first();
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();
    await row.click();

    await this.waitForContextModalClosed();
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText('All Sites');
    this.context = { siteId: null, hostName: null, isInherited: false };
  }

  /** Selects the site-level row for `siteName`; the context label shows the site name. */
  async switchToSite(siteName: string): Promise<void> {
    const modal = await this.openContextModal();
    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: new RegExp(`^${escapeRegExp(siteName)}$`) }),
    }).first();
    await this.selectScopedRow(row, siteName, null);
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
    await this.selectScopedRow(row, `${siteName} - ${hostName}`, hostName);
  }

  /**
   * If the current scope is inherited, click Create Override so headers can be added.
   * No-op at global scope or when the scope already has an override.
   *
   * The decision is made from the status response captured by switchToSite/switchToHost
   * rather than from the DOM: the UI's inherited flag defaults to false until that response lands,
   * so "Revert to Inherited" can briefly show for a scope which is in fact inherited.
   */
  async ensureOverrideExists(): Promise<void> {
    if (!this.context.isInherited || this.context.siteId === null) {
      return;
    }

    const createOverride = this.page.getByRole('button', { name: 'Create Override' });
    await expect(createOverride).toBeVisible({ timeout: 10_000 });

    const scopeReloaded = this.waitForScopeLoad(this.context.hostName, this.context.siteId);
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
}
