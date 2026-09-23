import { test } from '@playwright/test';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { PermissionsPolicyPage } from '../helpers/permissions-policy-page';
import { expectDirectiveAbsent, expectDirectiveValue } from '../helpers/permissions-policy-headers';
import { resetSystem, resetSystemWithDeprecatedDirectives } from '../helpers/reset';

test.describe('Permissions Policy directives (current)', () => {
  test.beforeEach(async ({ page, request }) => {
    await resetSystem(request);
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
  });

  test('directives added or renamed in line with the current specification can be configured and are emitted', async ({ page, request }) => {
    const pp = new PermissionsPolicyPage(page, env.appOneCmsUrl);
    await pp.open();

    await pp.switchToGlobal();
    await pp.ensureHeaderEnabled();

    // The card titles below must resolve to the directive names asserted against the header.
    await pp.setDirective('Local Network Access', 'ThisSite');
    await pp.setDirective('Summarizer', 'SpecificSites', ['https://www.example.com']);

    // Both of these were renamed: 'opt-credentials' was a transposition of 'otp-credentials',
    // and 'identity-credentials' became 'identity-credentials-get'.
    await pp.setDirective('OTP Credentials', 'All');
    await pp.setDirective('Identity Credentials', 'None');

    await test.step('newly supported directives reach the response', async () => {
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'local-network-access', '(self)', { label: 'App One Frontend (:5000)' });
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'summarizer', '("https://www.example.com")', { label: 'App One Frontend (:5000)' });
    });

    await test.step('renamed directives reach the response under their current names', async () => {
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'otp-credentials', '*', { label: 'App One Frontend (:5000)' });
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'identity-credentials-get', '()', { label: 'App One Frontend (:5000)' });
    });

    await test.step('the names they replaced are not emitted', async () => {
      await expectDirectiveAbsent(request, env.appOneFrontendUrl, 'opt-credentials', { label: 'App One Frontend (:5000)' });
      await expectDirectiveAbsent(request, env.appOneFrontendUrl, 'identity-credentials', { label: 'App One Frontend (:5000)' });
    });
  });

  test('deprecated directives are not offered when they have no stored configuration', async ({ page }) => {
    const pp = new PermissionsPolicyPage(page, env.appOneCmsUrl);
    await pp.open();

    await pp.switchToGlobal();

    await pp.expectDirectiveNotListed('Attribution Reporting');
    await pp.expectDirectiveNotListed('Browsing Topics');
    await pp.expectDirectiveNotListed('Document Domain');

    // Control: a current directive is still offered, so the assertions above are meaningful.
    await pp.expectDirectiveListed('Camera');
    await pp.expectDirectiveNotDeprecated('Camera');
  });
});

test.describe('Permissions Policy directives (deprecated)', () => {
  test.beforeEach(async ({ page, request }) => {
    await resetSystemWithDeprecatedDirectives(request);
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
  });

  test('existing deprecated configuration is surfaced with a warning and still applied', async ({ page, request }) => {
    const pp = new PermissionsPolicyPage(page, env.appOneCmsUrl);
    await pp.open();

    await pp.switchToGlobal();
    await pp.ensureHeaderEnabled();

    await test.step('each deprecated directive is listed and flagged', async () => {
      await pp.expectDirectiveDeprecated('Attribution Reporting');
      await pp.expectDirectiveDeprecated('Browsing Topics');
      await pp.expectDirectiveDeprecated('Document Domain');
    });

    await test.step('the stored configuration is still applied to the response', async () => {
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'attribution-reporting', '*', { label: 'App One Frontend (:5000)' });
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'browsing-topics', '(self)', { label: 'App One Frontend (:5000)' });
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'document-domain', '()', { label: 'App One Frontend (:5000)' });
    });
  });

  test('a deprecated directive can still be switched off through the user interface', async ({ page, request }) => {
    const pp = new PermissionsPolicyPage(page, env.appOneCmsUrl);
    await pp.open();

    await pp.switchToGlobal();
    await pp.ensureHeaderEnabled();

    await pp.setDirective('Document Domain', 'Disabled');

    await expectDirectiveAbsent(request, env.appOneFrontendUrl, 'document-domain', { label: 'App One Frontend (:5000)' });

    // The card remains so the configuration can be seen and reversed; it just no longer contributes.
    await pp.expectDirectiveDeprecated('Document Domain');
  });
});
