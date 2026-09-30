import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CspSourcePage } from '../helpers/csp-page';
import { expectCspHeader } from '../helpers/csp-headers';
import { resetSystem } from '../helpers/reset';

// Seeded by Sample/OptimizelyTwelveTest/Features/Configuration/SetupMigrationStep.cs
// as SiteDefinition.Name. CMS 12 keys sites by Guid, so the UI only ever shows the name.
const SITE_ONE_NAME = 'Test Website 1';

function escapeRegex(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

test.describe('CSP source scoping (Site Level)', () => {
  test.beforeEach(async ({ request }) => {
    await resetSystem(request);
  });

  test('a source added at site level is visible for every host in that site and absent from other sites', async ({ page, request }) => {
    const guidHost = `https://www.${randomUUID()}.com`;
    const escaped = escapeRegex(guidHost);

    const includesScopedSource = (csp: string | undefined): boolean =>
      !!csp
      && new RegExp(`(?:^|;)\\s*script-src[^;]*${escaped}`).test(csp)
      && new RegExp(`(?:^|;)\\s*script-src-elem[^;]*${escaped}`).test(csp);

    // Lenient: if the other site has no CSP at all, the source is definitionally absent.
    const excludesScopedSource = (csp: string | undefined): boolean => !csp?.includes(guidHost);

    const cspPage = new CspSourcePage(page, env.appOneCmsUrl);

    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    await cspPage.open();
    await cspPage.switchToSite(SITE_ONE_NAME);
    await cspPage.addSource(guidHost, ['script-src', 'script-src-elem']);

    await test.step('Site One frontend (:5000) CSP includes the scoped source', async () => {
      await expectCspHeader(request, env.appOneFrontendUrl, includesScopedSource, {
        label: 'Site One Frontend (:5000)',
        message: `Site One frontend (${env.appOneFrontendUrl}) CSP did not contain ${guidHost} for script-src/script-src-elem`,
      });
    });

    await test.step('Site One CMS host (:5001) CSP includes the scoped source', async () => {
      await expectCspHeader(request, env.appOneCmsUrl, includesScopedSource, {
        label: 'Site One CMS (:5001)',
        message: `Site One CMS host (${env.appOneCmsUrl}) CSP did not contain ${guidHost} for script-src/script-src-elem`,
      });
    });

    await test.step('Site Two (:5002) CSP excludes the scoped source', async () => {
      await expectCspHeader(request, env.appTwoUrl, excludesScopedSource, {
        timeout: 5_000,
        label: 'Site Two (:5002)',
        message: `Site Two (${env.appTwoUrl}) CSP unexpectedly contained ${guidHost}`,
      });
    });
  });
});
