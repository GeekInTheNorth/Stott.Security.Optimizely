import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CspSourcePage } from '../helpers/csp-page';
import { expectCspHeader } from '../helpers/csp-headers';
import { resetSystem } from '../helpers/reset';

// Application One, Primary host. Seeded by SetupMigrationStep.cs — see
// SecurityTxtHelpers.CreateHostSummaries for how display/host names are derived.
const APP_ONE_ID = 'TestWebsite1';
const APP_ONE_PRIMARY_HOST_DISPLAY = 'https://localhost:5000/';
const APP_ONE_PRIMARY_HOST_NAME = 'localhost:5000';

function escapeRegex(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

test.describe('CSP source scoping (Host Level)', () => {
  test.beforeEach(async ({ request }) => {
    await resetSystem(request);
  });

  test('a source added at host level is visible only for that host and absent from other hosts and applications', async ({ page, request }) => {
    const guidHost = `https://www.${randomUUID()}.com`;
    const escaped = escapeRegex(guidHost);

    const includesScopedSource = (csp: string | undefined): boolean =>
      !!csp && new RegExp(`(?:^|;)\\s*frame-src[^;]*${escaped}`).test(csp);

    // Lenient: if a host has no CSP at all the source is definitionally absent.
    const excludesScopedSource = (csp: string | undefined): boolean => !csp?.includes(guidHost);

    const cspPage = new CspSourcePage(page, env.appOneCmsUrl);

    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    await cspPage.open();
    await cspPage.switchToHost(APP_ONE_PRIMARY_HOST_DISPLAY, APP_ONE_ID, APP_ONE_PRIMARY_HOST_NAME);
    await cspPage.addSource(guidHost, ['frame-src']);

    await test.step('App One primary host (:5000) CSP includes the host-scoped source', async () => {
      await expectCspHeader(request, env.appOneFrontendUrl, includesScopedSource, {
        label: 'App One Frontend (:5000)',
        message: `App One primary host (${env.appOneFrontendUrl}) CSP did not contain ${guidHost} for frame-src`,
      });
    });

    await test.step('App One CMS host (:5001) CSP excludes the host-scoped source', async () => {
      await expectCspHeader(request, env.appOneCmsUrl, excludesScopedSource, {
        timeout: 5_000,
        label: 'App One CMS (:5001)',
        message: `App One CMS host (${env.appOneCmsUrl}) CSP unexpectedly contained ${guidHost}`,
      });
    });

    await test.step('App Two (:5002) CSP excludes the host-scoped source', async () => {
      await expectCspHeader(request, env.appTwoUrl, excludesScopedSource, {
        timeout: 5_000,
        label: 'App Two (:5002)',
        message: `App Two (${env.appTwoUrl}) CSP unexpectedly contained ${guidHost}`,
      });
    });
  });
});
