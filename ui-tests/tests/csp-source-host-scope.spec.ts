import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CspSourcePage } from '../helpers/csp-page';
import { expectCspHeader } from '../helpers/csp-headers';
import { resetSystem } from '../helpers/reset';

// Site One, Primary host. Seeded by SetupMigrationStep.cs — see
// SiteDefinitionExtensions.ToHostSummaries for how the host row text is derived
// (DisplayName and HostName are both HostDefinition.Name).
const SITE_ONE_NAME = 'Test Website 1';
const SITE_ONE_PRIMARY_HOST = 'localhost:5000';

function escapeRegex(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

test.describe('CSP source scoping (Host Level)', () => {
  test.beforeEach(async ({ request }) => {
    await resetSystem(request);
  });

  test('a source added at host level is visible only for that host and absent from other hosts and sites', async ({ page, request }) => {
    const guidHost = `https://www.${randomUUID()}.com`;
    const escaped = escapeRegex(guidHost);

    const includesScopedSource = (csp: string | undefined): boolean =>
      !!csp && new RegExp(`(?:^|;)\\s*frame-src[^;]*${escaped}`).test(csp);

    // Lenient: if a host has no CSP at all the source is definitionally absent.
    const excludesScopedSource = (csp: string | undefined): boolean => !csp?.includes(guidHost);

    const cspPage = new CspSourcePage(page, env.appOneCmsUrl);

    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    await cspPage.open();
    await cspPage.switchToHost(SITE_ONE_PRIMARY_HOST, SITE_ONE_NAME);
    await cspPage.addSource(guidHost, ['frame-src']);

    await test.step('Site One primary host (:5000) CSP includes the host-scoped source', async () => {
      await expectCspHeader(request, env.appOneFrontendUrl, includesScopedSource, {
        label: 'Site One Frontend (:5000)',
        message: `Site One primary host (${env.appOneFrontendUrl}) CSP did not contain ${guidHost} for frame-src`,
      });
    });

    await test.step('Site One CMS host (:5001) CSP excludes the host-scoped source', async () => {
      await expectCspHeader(request, env.appOneCmsUrl, excludesScopedSource, {
        timeout: 5_000,
        label: 'Site One CMS (:5001)',
        message: `Site One CMS host (${env.appOneCmsUrl}) CSP unexpectedly contained ${guidHost}`,
      });
    });

    await test.step('Site Two (:5002) CSP excludes the host-scoped source', async () => {
      await expectCspHeader(request, env.appTwoUrl, excludesScopedSource, {
        timeout: 5_000,
        label: 'Site Two (:5002)',
        message: `Site Two (${env.appTwoUrl}) CSP unexpectedly contained ${guidHost}`,
      });
    });
  });
});
