import { test } from '@playwright/test';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { PermissionsPolicyPage } from '../helpers/permissions-policy-page';
import { expectDirectiveValue } from '../helpers/permissions-policy-headers';
import { resetSystem } from '../helpers/reset';

test.describe('Permissions Policy scoping (Application)', () => {
  test.beforeEach(async ({ page, request }) => {
    await resetSystem(request);
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
  });

  test('a directive overridden at application level applies to that application\'s hosts only; other applications keep the global value', async ({ page, request }) => {
    const pp = new PermissionsPolicyPage(page, env.appOneCmsUrl);
    await pp.open();

    await pp.switchToGlobal();
    await pp.ensureHeaderEnabled();
    await pp.setDirective('Camera', 'None');

    await pp.switchToApplication('Test Website 1', 'TestWebsite1');
    await pp.ensureOverrideExists();
    await pp.setDirective('Camera', 'ThisSite');

    await test.step('App One frontend (:5000) reflects the application-level override', async () => {
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'camera', '(self)', { label: 'App One Frontend (:5000)' });
    });

    await test.step('App One CMS host (:5001) reflects the application-level override', async () => {
      await expectDirectiveValue(request, env.appOneCmsUrl, 'camera', '(self)', { label: 'App One CMS (:5001)' });
    });

    await test.step('App Two (:5002) keeps the global value', async () => {
      await expectDirectiveValue(request, env.appTwoUrl, 'camera', '()', { label: 'App Two (:5002)' });
    });
  });
});
