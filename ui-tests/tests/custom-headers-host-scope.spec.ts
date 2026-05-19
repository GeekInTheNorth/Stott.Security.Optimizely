import { test } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CustomHeadersPage } from '../helpers/custom-headers-page';
import { expectResponseHeader } from '../helpers/response-headers';

test.describe('Custom Headers scoping (Host)', () => {
  test.beforeEach(async ({ page }) => {
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    const ch = new CustomHeadersPage(page, env.appOneCmsUrl);
    await ch.open();
    await ch.revertAllOverrides();
  });

  test('a custom header added at host scope is emitted only by that specific host', async ({ page, request }) => {
    const headerName = `X-Test-Host-${randomUUID()}`;
    const headerValue = `enabled-${randomUUID()}`;
    const expectedValue = (v: string | undefined): boolean => v === headerValue;
    const headerAbsent = (v: string | undefined): boolean => v === undefined;

    const ch = new CustomHeadersPage(page, env.appOneCmsUrl);
    await ch.switchToHost('https://localhost:5000/', 'TestWebsite1', 'localhost:5000');
    await ch.ensureOverrideExists();
    await ch.addHeader(headerName, headerValue);

    await test.step('App One primary host (:5000) emits the host-scoped header', async () => {
      await expectResponseHeader(request, env.appOneFrontendUrl, headerName, expectedValue, {
        label: 'App One Frontend (:5000)',
        message: `App One primary host did not emit ${headerName}=${headerValue}`,
      });
    });

    await test.step('App One CMS host (:5001) does not emit the host-scoped header', async () => {
      await expectResponseHeader(request, env.appOneCmsUrl, headerName, headerAbsent, {
        timeout: 5_000,
        label: 'App One CMS (:5001)',
        message: `App One CMS host unexpectedly emitted ${headerName}`,
      });
    });

    await test.step('App Two (:5002) does not emit the host-scoped header', async () => {
      await expectResponseHeader(request, env.appTwoUrl, headerName, headerAbsent, {
        timeout: 5_000,
        label: 'App Two (:5002)',
        message: `App Two unexpectedly emitted ${headerName}`,
      });
    });

    // Cleanup is handled by the next test's beforeEach revert sweep — the
    // header lives on the host override which gets deleted in its entirety.
  });
});
