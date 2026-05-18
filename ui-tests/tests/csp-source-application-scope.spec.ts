import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CspSourcePage } from '../helpers/csp-page';
import { expectCspHeader } from '../helpers/csp-headers';

// Seeded by Sample/OptimizelyTwelveTest/Features/Configuration/SetupMigrationStep.cs
// AppId = InProcessWebsite.Name, AppName = InProcessWebsite.DisplayName.
const APP_ONE_DISPLAY = 'Test Website 1';
const APP_ONE_ID = 'TestWebsite1';

function escapeRegex(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

test.describe('CSP source scoping (Application Level)', () => {
  test('a source added at application level is visible for every host in that application and absent from other applications', async ({ page, request }) => {
    const guidHost = `https://www.${randomUUID()}.com`;
    const escaped = escapeRegex(guidHost);

    const includesScopedSource = (csp: string | undefined): boolean =>
      !!csp
      && new RegExp(`(?:^|;)\\s*script-src[^;]*${escaped}`).test(csp)
      && new RegExp(`(?:^|;)\\s*script-src-elem[^;]*${escaped}`).test(csp);

    // Lenient: if the other application has no CSP at all, the source is definitionally absent.
    const excludesScopedSource = (csp: string | undefined): boolean => !csp?.includes(guidHost);

    const cspPage = new CspSourcePage(page, env.appOneCmsUrl);

    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    await cspPage.open();
    await cspPage.switchToApplication(APP_ONE_DISPLAY, APP_ONE_ID);
    await cspPage.addSource(guidHost, ['script-src', 'script-src-elem']);

    try {
      await test.step('App One frontend (:5000) CSP includes the scoped source', async () => {
        await expectCspHeader(request, env.appOneFrontendUrl, includesScopedSource, {
          label: 'App One Frontend (:5000)',
          message: `App One frontend (${env.appOneFrontendUrl}) CSP did not contain ${guidHost} for script-src/script-src-elem`,
        });
      });

      await test.step('App One CMS host (:5001) CSP includes the scoped source', async () => {
        await expectCspHeader(request, env.appOneCmsUrl, includesScopedSource, {
          label: 'App One CMS (:5001)',
          message: `App One CMS host (${env.appOneCmsUrl}) CSP did not contain ${guidHost} for script-src/script-src-elem`,
        });
      });

      await test.step('App Two (:5002) CSP excludes the scoped source', async () => {
        await expectCspHeader(request, env.appTwoUrl, excludesScopedSource, {
          timeout: 5_000,
          label: 'App Two (:5002)',
          message: `App Two (${env.appTwoUrl}) CSP unexpectedly contained ${guidHost}`,
        });
      });
    } finally {
      await cspPage.open();
      await cspPage.switchToApplication(APP_ONE_DISPLAY, APP_ONE_ID);
      await cspPage.deleteSource(guidHost);
    }
  });
});
