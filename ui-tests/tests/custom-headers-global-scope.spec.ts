import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CustomHeadersPage } from '../helpers/custom-headers-page';
import { expectResponseHeader } from '../helpers/response-headers';

test.describe('Custom Headers scoping (Global)', () => {
  test.beforeEach(async ({ page }) => {
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    const ch = new CustomHeadersPage(page, env.appOneCmsUrl);
    await ch.open();
    await ch.revertAllOverrides();
  });

  test('a custom header added at the global level is emitted by every host across applications', async ({ page, request }) => {
    const headerName = `X-Test-Global-${randomUUID()}`;
    const headerValue = `enabled-${randomUUID()}`;
    const expectedValue = (v: string | undefined): boolean => v === headerValue;

    const ch = new CustomHeadersPage(page, env.appOneCmsUrl);
    await ch.switchToGlobal();
    await ch.addHeader(headerName, headerValue);

    try {
      await test.step('App One frontend (:5000) emits the global custom header', async () => {
        await expectResponseHeader(request, env.appOneFrontendUrl, headerName, expectedValue, {
          label: 'App One Frontend (:5000)',
          message: `App One frontend did not emit ${headerName}=${headerValue}`,
        });
      });

      await test.step('App One CMS host (:5001) emits the global custom header', async () => {
        await expectResponseHeader(request, env.appOneCmsUrl, headerName, expectedValue, {
          label: 'App One CMS (:5001)',
          message: `App One CMS host did not emit ${headerName}=${headerValue}`,
        });
      });

      await test.step('App Two (:5002) emits the global custom header', async () => {
        await expectResponseHeader(request, env.appTwoUrl, headerName, expectedValue, {
          label: 'App Two (:5002)',
          message: `App Two did not emit ${headerName}=${headerValue}`,
        });
      });
    } finally {
      // Global headers don't get cleaned up by revertAllOverrides (it only
      // touches app + host overrides), so delete this test's header explicitly.
      await ch.open();
      await ch.switchToGlobal();
      await ch.deleteHeader(headerName);
    }
  });
});
