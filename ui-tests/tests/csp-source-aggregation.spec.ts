import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CspSourcePage } from '../helpers/csp-page';
import { expectCspHeader } from '../helpers/csp-headers';
import { resetSystem } from '../helpers/reset';

const APP_ONE_DISPLAY = 'Test Website 1';
const APP_ONE_ID = 'TestWebsite1';
const APP_ONE_PRIMARY_HOST_DISPLAY = 'https://localhost:5000/';
const APP_ONE_PRIMARY_HOST_NAME = 'localhost:5000';

test.describe('CSP source aggregation across scopes', () => {
  test.beforeEach(async ({ request }) => {
    await resetSystem(request);
  });

  test('a host receives global, application and host-scoped sources combined in its CSP', async ({ page, request }) => {
    const globalSource = `https://www.${randomUUID()}.com`;
    const appSource = `https://www.${randomUUID()}.com`;
    const hostSource = `https://www.${randomUUID()}.com`;

    const cspPage = new CspSourcePage(page, env.appOneCmsUrl);

    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    await cspPage.open();
    // Page opens in "All Applications" (global) context by default.
    await cspPage.addSource(globalSource, ['script-src']);

    await cspPage.switchToApplication(APP_ONE_DISPLAY, APP_ONE_ID);
    await cspPage.addSource(appSource, ['script-src']);

    await cspPage.switchToHost(APP_ONE_PRIMARY_HOST_DISPLAY, APP_ONE_ID, APP_ONE_PRIMARY_HOST_NAME);
    await cspPage.addSource(hostSource, ['script-src']);

    const includesAllThree = (csp: string | undefined): boolean => {
      if (!csp) return false;
      // Find the segment for the exact `script-src` directive (not `script-src-elem` /
      // `script-src-attr`). Splitting on `;` and matching the directive name verbatim
      // avoids prefix collisions a regex like /script-src[^;]*/ would otherwise hit.
      const scriptSrc = csp
        .split(';')
        .map(segment => segment.trim())
        .find(segment => segment === 'script-src' || segment.startsWith('script-src '));
      if (!scriptSrc) return false;
      return [globalSource, appSource, hostSource].every(source => scriptSrc.includes(source));
    };

    await test.step('App One primary host (:5000) script-src includes global, app and host sources', async () => {
      await expectCspHeader(request, env.appOneFrontendUrl, includesAllThree, {
        label: 'App One Frontend (:5000)',
        message: `App One primary host (${env.appOneFrontendUrl}) script-src did not contain all three sources (global=${globalSource}, app=${appSource}, host=${hostSource})`,
      });
    });
  });
});
