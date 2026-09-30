import { Page, expect } from '@playwright/test';

function escapeRegExp(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

export type CspDirective =
  | 'base-uri'
  | 'default-src'
  | 'child-src'
  | 'connect-src'
  | 'font-src'
  | 'form-action'
  | 'frame-ancestors'
  | 'frame-src'
  | 'img-src'
  | 'manifest-src'
  | 'media-src'
  | 'object-src'
  | 'script-src'
  | 'script-src-attr'
  | 'script-src-elem'
  | 'style-src'
  | 'style-src-attr'
  | 'style-src-elem'
  | 'worker-src';

const directiveCheckboxId: Record<CspDirective, string> = {
  'base-uri': '#chkBaseUri',
  'default-src': '#chkDefaultSrc',
  'child-src': '#chkChildSrc',
  'connect-src': '#chkConnectSrc',
  'font-src': '#chkFontSrc',
  'form-action': '#chkFormAction',
  'frame-ancestors': '#chkFrameAncestors',
  'frame-src': '#chkFrameSrc',
  'img-src': '#chkImgSrc',
  'manifest-src': '#chkManifestSrc',
  'media-src': '#chkMediaSrc',
  'object-src': '#chkObjectSrc',
  'script-src': '#chkScriptSrc',
  'script-src-attr': '#chkScriptSrcAttr',
  'script-src-elem': '#chkScriptSrcElem',
  'style-src': '#chkStyleSrc',
  'style-src-attr': '#chkStyleSrcAttr',
  'style-src-elem': '#chkStyleSrcElem',
  'worker-src': '#chkWorkerSrc',
};

export class CspSourcePage {
  constructor(private readonly page: Page, private readonly cmsUrl: string) {}

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#csp-source`);
    await expect(this.page.getByRole('button', { name: 'Add Source' })).toBeVisible();
  }

  /**
   * Opens the "Switch Context" modal and selects "All Sites", putting the
   * page in global scope (no siteId, no hostName).
   */
  async switchToGlobal(): Promise<void> {
    await this.page.getByRole('button', { name: 'Switch Context' }).click();

    const modal = this.page.locator('.modal.show', { hasText: 'Select Site Context' });
    await expect(modal).toBeVisible();

    const row = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: /^All Sites$/ }),
    }).first();
    await row.click();

    await this.waitForContextModalClosed();
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText('All Sites');
  }

  /**
   * The `.modal.show` locator stops matching as soon as the "show" class drops,
   * but Bootstrap keeps the dialog in the DOM during its fade-out and it still
   * intercepts pointer events. Wait for the dialog itself to be hidden.
   */
  private async waitForContextModalClosed(): Promise<void> {
    await expect(this.page.locator('.modal', { hasText: 'Select Site Context' })).toBeHidden({ timeout: 10_000 });
  }

  /**
   * Opens the "Switch Context" modal and selects the top-level (site-scope)
   * row for `siteName` (e.g. "Test Website 1"). After this returns the page
   * is operating in siteId-scope with no host selected — i.e. "Site Level".
   * The context label shows the site name (CMS 12 keys sites by Guid, which
   * is never rendered).
   */
  async switchToSite(siteName: string): Promise<void> {
    await this.page.getByRole('button', { name: 'Switch Context' }).click();

    const modal = this.page.locator('.modal.show', { hasText: 'Select Site Context' });
    await expect(modal).toBeVisible();

    const siteRow = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: new RegExp(`^${escapeRegExp(siteName)}$`) }),
    }).first();
    await siteRow.click();

    await this.waitForContextModalClosed();
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(siteName);
  }

  /**
   * Opens the "Switch Context" modal and selects a specific host row under a
   * site. After this the page is operating in (siteId, hostName) scope —
   * i.e. "Host Level".
   *
   * @param hostName The host as configured on the site definition (e.g. "localhost:5000").
   *                 CMS 12 renders the host row and the context label with this same value.
   * @param siteName The site display name the context label should show (e.g. "Test Website 1").
   */
  async switchToHost(hostName: string, siteName: string): Promise<void> {
    await this.page.getByRole('button', { name: 'Switch Context' }).click();

    const modal = this.page.locator('.modal.show', { hasText: 'Select Site Context' });
    await expect(modal).toBeVisible();

    const hostRow = modal
      .locator('.list-group-item', { hasText: hostName })
      .filter({ hasText: 'Host-level configuration' })
      .first();
    await hostRow.click();

    await this.waitForContextModalClosed();
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(`${siteName} - ${hostName}`);
  }

  async addSource(source: string, directives: CspDirective[]): Promise<void> {
    await this.page.getByRole('button', { name: 'Add Source' }).click();

    const modal = this.page.locator('.modal.show');
    await expect(modal.getByText('Edit Source Directives')).toBeVisible();

    await modal.locator('#formSource').fill(source);

    // The modal debounces the valid-directives lookup by 1s; the directives
    // we tick remain visible for any standard https URL but we still need
    // the lookup to resolve before clicking Save so the directives array
    // it submits matches the validDirectives filter.
    await this.page.waitForTimeout(1500);

    for (const directive of directives) {
      await modal.locator(directiveCheckboxId[directive]).check();
    }

    await modal.getByRole('button', { name: 'Save' }).click();

    await expect(modal).toBeHidden({ timeout: 15_000 });
    await expect(this.page.getByText(`Successfully saved the source: ${source}`)).toBeVisible();
  }
}
