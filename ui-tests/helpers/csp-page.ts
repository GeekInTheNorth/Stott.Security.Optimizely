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
   * Opens the "Switch Context" modal and selects the top-level (application-scope)
   * row for `appDisplayName` (e.g. "Test Website 1"). After this returns the page
   * is operating in appId-scope with no host selected — i.e. "Application Level".
   */
  async switchToApplication(appDisplayName: string, expectedAppId: string): Promise<void> {
    await this.page.getByRole('button', { name: 'Switch Context' }).click();

    const modal = this.page.locator('.modal.show', { hasText: 'Select Application Context' });
    await expect(modal).toBeVisible();

    const appRow = modal.locator('.list-group-item', {
      has: this.page.locator('strong', { hasText: new RegExp(`^${escapeRegExp(appDisplayName)}$`) }),
    }).first();
    await appRow.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(expectedAppId);
  }

  /**
   * Opens the "Switch Context" modal and selects a specific host row under an
   * application. After this the page is operating in (appId, hostName) scope —
   * i.e. "Host Level".
   *
   * @param hostDisplayName The visible host text in the modal (e.g. "https://localhost:5000/")
   * @param expectedAppId   The app identifier the context label should show (e.g. "TestWebsite1")
   * @param expectedHostName The host identifier the context label should show (e.g. "localhost:5000")
   */
  async switchToHost(hostDisplayName: string, expectedAppId: string, expectedHostName: string): Promise<void> {
    await this.page.getByRole('button', { name: 'Switch Context' }).click();

    const modal = this.page.locator('.modal.show', { hasText: 'Select Application Context' });
    await expect(modal).toBeVisible();

    const hostRow = modal
      .locator('.list-group-item', { hasText: hostDisplayName })
      .filter({ hasText: 'Host-level configuration' })
      .first();
    await hostRow.click();

    await expect(modal).toBeHidden({ timeout: 10_000 });
    await expect(this.page.locator('strong:has-text("Context:") + span')).toHaveText(`${expectedAppId} - ${expectedHostName}`);
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

  async deleteSource(source: string): Promise<void> {
    const row = this.page.locator('tr', { hasText: source }).first();
    await expect(row).toBeVisible();
    await row.getByRole('button', { name: 'Delete' }).click();

    const confirmModal = this.page.locator('.modal.show', { hasText: 'Delete Source' });
    await expect(confirmModal).toBeVisible();
    await confirmModal.getByRole('button', { name: 'Delete' }).click();

    await expect(confirmModal).toBeHidden({ timeout: 15_000 });
    await expect(this.page.locator('tr', { hasText: source })).toHaveCount(0, { timeout: 15_000 });
  }
}
