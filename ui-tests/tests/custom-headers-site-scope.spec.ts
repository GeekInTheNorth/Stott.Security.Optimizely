import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CustomHeadersPage } from '../helpers/custom-headers-page';
import { expectResponseHeader } from '../helpers/response-headers';
import { resetSystem } from '../helpers/reset';

test.describe('Custom Headers scoping (Site)', () => {
  test.beforeEach(async ({ page, request }) => {
    await resetSystem(request);
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
  });

  test('a custom header added at site scope is emitted only by hosts of that site', async ({ page, request }) => {
    const headerName = `X-Test-Site-${randomUUID()}`;
    const headerValue = `enabled-${randomUUID()}`;
    const expectedValue = (v: string | undefined): boolean => v === headerValue;
    const headerAbsent = (v: string | undefined): boolean => v === undefined;

    const ch = new CustomHeadersPage(page, env.appOneCmsUrl);
    await ch.open();
    await ch.switchToSite('Test Website 1');
    await ch.ensureOverrideExists();
    await ch.addHeader(headerName, headerValue);

    await test.step('Site One frontend (:5000) emits the site-scoped header', async () => {
      await expectResponseHeader(request, env.appOneFrontendUrl, headerName, expectedValue, {
        label: 'Site One Frontend (:5000)',
        message: `Site One frontend did not emit ${headerName}=${headerValue}`,
      });
    });

    await test.step('Site One CMS host (:5001) emits the site-scoped header', async () => {
      await expectResponseHeader(request, env.appOneCmsUrl, headerName, expectedValue, {
        label: 'Site One CMS (:5001)',
        message: `Site One CMS host did not emit ${headerName}=${headerValue}`,
      });
    });

    await test.step('Site Two (:5002) does not emit the site-scoped header', async () => {
      await expectResponseHeader(request, env.appTwoUrl, headerName, headerAbsent, {
        timeout: 5_000,
        label: 'Site Two (:5002)',
        message: `Site Two unexpectedly emitted ${headerName}`,
      });
    });
  });
});
