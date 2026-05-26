import { test } from '@playwright/test';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { PermissionsPolicyPage } from '../helpers/permissions-policy-page';
import { expectDirectiveValue } from '../helpers/permissions-policy-headers';
import { resetSystem } from '../helpers/reset';

test.describe('Permissions Policy scoping (Host)', () => {
  test.beforeEach(async ({ page, request }) => {
    await resetSystem(request);
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
  });

  test('host, application and global directives each apply to the correct scope only', async ({ page, request }) => {
    const pp = new PermissionsPolicyPage(page, env.appOneCmsUrl);
    await pp.open();

    await pp.switchToGlobal();
    await pp.ensureHeaderEnabled();
    await pp.setDirective('Camera', 'None');

    await pp.switchToApplication('Test Website 1', 'TestWebsite1');
    await pp.ensureOverrideExists();
    await pp.setDirective('Camera', 'ThisSite');

    await pp.switchToHost('https://localhost:5000/', 'TestWebsite1', 'localhost:5000');
    await pp.ensureOverrideExists();
    await pp.setDirective('Camera', 'All');

    await test.step('App One primary host (:5000) reflects the host-level override', async () => {
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'camera', '*', { label: 'App One Frontend (:5000)' });
    });

    await test.step('App One CMS host (:5001) reflects the application-level override', async () => {
      await expectDirectiveValue(request, env.appOneCmsUrl, 'camera', '(self)', { label: 'App One CMS (:5001)' });
    });

    await test.step('App Two (:5002) keeps the global value', async () => {
      await expectDirectiveValue(request, env.appTwoUrl, 'camera', '()', { label: 'App Two (:5002)' });
    });
  });
});
