import { Page, expect } from '@playwright/test';
import { ContextSwitcher, dialog, expectToast } from './axiom';

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
  private readonly contextSwitcher: ContextSwitcher;

  constructor(private readonly page: Page, private readonly cmsUrl: string) {
    this.contextSwitcher = new ContextSwitcher(page);
  }

  async open(): Promise<void> {
    await this.page.goto(`${this.cmsUrl}/stott.security.optimizely/administration/#csp-source`);
    await expect(this.page.getByRole('button', { name: 'Add Source' })).toBeVisible();
  }

  /**
   * Select "All Applications" in the context switcher, putting the page in global
   * scope (no appId, no hostName).
   */
  async switchToGlobal(): Promise<void> {
    await this.contextSwitcher.select('All Applications', 'All Applications');
  }

  /**
   * Select the application-scope option for `appDisplayName` (e.g. "Test Website 1").
   * After this returns the page is operating in appId-scope with no host selected,
   * i.e. "Application Level".
   */
  async switchToApplication(appDisplayName: string, expectedAppId: string): Promise<void> {
    await this.contextSwitcher.select(appDisplayName, expectedAppId);
  }

  /**
   * Select a specific host option under an application. After this the page is
   * operating in (appId, hostName) scope, i.e. "Host Level".
   *
   * @param hostDisplayName  The visible host text in the dialog (e.g. "https://localhost:5000/")
   * @param expectedAppId    The app identifier the context label should show (e.g. "TestWebsite1")
   * @param expectedHostName The host identifier the context label should show (e.g. "localhost:5000")
   */
  async switchToHost(hostDisplayName: string, expectedAppId: string, expectedHostName: string): Promise<void> {
    await this.contextSwitcher.select(hostDisplayName, `${expectedAppId} - ${expectedHostName}`);
  }

  async addSource(source: string, directives: CspDirective[]): Promise<void> {
    await this.page.getByRole('button', { name: 'Add Source' }).click();

    const modal = dialog(this.page, 'Edit Source Directives');
    await expect(modal).toBeVisible();

    await modal.locator('#formSource').fill(source);

    // The modal debounces the valid-directives lookup by 1s; the directives
    // we tick remain visible for any standard https URL but we still need
    // the lookup to resolve before clicking Save so the directives array
    // it submits matches the validDirectives filter.
    await this.page.waitForTimeout(1500);

    for (const directive of directives) {
      // Axiom visually hides the native input (1px clipped), so click its label instead.
      const checkbox = modal.locator(directiveCheckboxId[directive]);
      if (!(await checkbox.isChecked())) {
        await checkbox.locator('xpath=ancestor::label[1]').click();
      }
      await expect(checkbox).toBeChecked();
    }

    await modal.getByRole('button', { name: 'Save' }).click();

    await expect(modal).toBeHidden({ timeout: 15_000 });
    await expectToast(this.page, `Successfully saved the source: ${source}`);
  }
}
