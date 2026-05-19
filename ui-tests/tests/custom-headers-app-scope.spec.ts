import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CustomHeadersPage } from '../helpers/custom-headers-page';
import { expectResponseHeader } from '../helpers/response-headers';

test.describe('Custom Headers scoping (Application)', () => {
  test.beforeEach(async ({ page }) => {
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    const ch = new CustomHeadersPage(page, env.appOneCmsUrl);
    await ch.open();
    await ch.revertAllOverrides();
  });

  test('a custom header added at application scope is emitted only by hosts of that application', async ({ page, request }) => {
    const headerName = `X-Test-App-${randomUUID()}`;
    const headerValue = `enabled-${randomUUID()}`;
    const expectedValue = (v: string | undefined): boolean => v === headerValue;
    const headerAbsent = (v: string | undefined): boolean => v === undefined;

    const ch = new CustomHeadersPage(page, env.appOneCmsUrl);
    await ch.switchToApplication('Test Website 1', 'TestWebsite1');
    await ch.ensureOverrideExists();
    await ch.addHeader(headerName, headerValue);

    await test.step('App One frontend (:5000) emits the application-scoped header', async () => {
      await expectResponseHeader(request, env.appOneFrontendUrl, headerName, expectedValue, {
        label: 'App One Frontend (:5000)',
        message: `App One frontend did not emit ${headerName}=${headerValue}`,
      });
    });

    await test.step('App One CMS host (:5001) emits the application-scoped header', async () => {
      await expectResponseHeader(request, env.appOneCmsUrl, headerName, expectedValue, {
        label: 'App One CMS (:5001)',
        message: `App One CMS host did not emit ${headerName}=${headerValue}`,
      });
    });

    await test.step('App Two (:5002) does not emit the application-scoped header', async () => {
      await expectResponseHeader(request, env.appTwoUrl, headerName, headerAbsent, {
        timeout: 5_000,
        label: 'App Two (:5002)',
        message: `App Two unexpectedly emitted ${headerName}`,
      });
    });

    // Cleanup is handled by the next test's beforeEach revert sweep — the
    // header lives on the App One override which gets deleted in its entirety.
  });
});
