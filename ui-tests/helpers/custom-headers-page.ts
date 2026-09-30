import { Locator, Page, expect } from '@playwright/test';

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

const OVERRIDE_STATUS_PATH = '/stott.security.optimizely/api/customheader/override/exists';

export class CustomHeadersPage {
  constructor(private readonly page: Page, private readonly cmsUrl: string) {}

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#response-headers`);
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
   *  - for site/host scopes, the override-status request has returned and the
   *    matching banner button is rendered. Until then the container still shows
   *    the previous scope's inherited state, so checking "Create Override" too
   *    early races the request and "Add Header" can unmount mid-click.
   */
  private async selectContextRow(row: Locator, expectedLabel: string, contextSpecific: boolean): Promise<void> {
    await expect(row).toBeVisible();
    await row.scrollIntoViewIfNeeded();

    const overrideStatus = contextSpecific
      ? this.page.waitForResponse(r => r.request().method() === 'GET' && r.url().includes(OVERRIDE_STATUS_PATH))
      : undefined;

    await row.click();

    await expect(this.page.locator('.modal', { hasText: 'Select Site Context' })).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(expectedLabel);

    if (overrideStatus) {
      const { isInherited } = await (await overrideStatus).json() as { isInherited: boolean };
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
   * Clicks "Create Override" when the inherited alert is showing. No-op if the
   * current scope is already overridden (or is the global scope, which has no
   * override concept). Call after switchToSite/switchToHost, which guarantee the
   * banner reflects the current scope.
   */
  async ensureOverrideExists(): Promise<void> {
    const createOverride = this.page.getByRole('button', { name: 'Create Override' });
    if (await createOverride.isVisible()) {
      await createOverride.click();
      await expect(this.page.getByRole('button', { name: 'Revert to Inherited' })).toBeVisible({ timeout: 10_000 });
      await expect(this.page.getByRole('button', { name: 'Add Header' })).toBeVisible({ timeout: 10_000 });
    }
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
