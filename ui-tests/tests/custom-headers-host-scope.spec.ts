import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CustomHeadersPage } from '../helpers/custom-headers-page';
import { expectResponseHeader } from '../helpers/response-headers';
import { resetSystem } from '../helpers/reset';

test.describe('Custom Headers scoping (Host)', () => {
  test.beforeEach(async ({ page, request }) => {
    await resetSystem(request);
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
  });

  test('a custom header added at host scope is emitted only by that specific host', async ({ page, request }) => {
    const headerName = `X-Test-Host-${randomUUID()}`;
    const headerValue = `enabled-${randomUUID()}`;
    const expectedValue = (v: string | undefined): boolean => v === headerValue;
    const headerAbsent = (v: string | undefined): boolean => v === undefined;

    const ch = new CustomHeadersPage(page, env.appOneCmsUrl);
    await ch.open();
    await ch.switchToHost('localhost:5000', 'Test Website 1');
    await ch.ensureOverrideExists();
    await ch.addHeader(headerName, headerValue);

    await test.step('Site One primary host (:5000) emits the host-scoped header', async () => {
      await expectResponseHeader(request, env.appOneFrontendUrl, headerName, expectedValue, {
        label: 'Site One Frontend (:5000)',
        message: `Site One primary host did not emit ${headerName}=${headerValue}`,
      });
    });

    await test.step('Site One CMS host (:5001) does not emit the host-scoped header', async () => {
      await expectResponseHeader(request, env.appOneCmsUrl, headerName, headerAbsent, {
        timeout: 5_000,
        label: 'Site One CMS (:5001)',
        message: `Site One CMS host unexpectedly emitted ${headerName}`,
      });
    });

    await test.step('Site Two (:5002) does not emit the host-scoped header', async () => {
      await expectResponseHeader(request, env.appTwoUrl, headerName, headerAbsent, {
        timeout: 5_000,
        label: 'Site Two (:5002)',
        message: `Site Two unexpectedly emitted ${headerName}`,
      });
    });
  });
});
