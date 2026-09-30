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

  test('host, site and global directives each apply to the correct scope only', async ({ page, request }) => {
    const pp = new PermissionsPolicyPage(page, env.appOneCmsUrl);
    await pp.open();

    await pp.switchToGlobal();
    await pp.ensureHeaderEnabled();
    await pp.setDirective('Camera', 'None');

    await pp.switchToSite('Test Website 1');
    await pp.ensureOverrideExists();
    await pp.setDirective('Camera', 'ThisSite');

    await pp.switchToHost('localhost:5000', 'Test Website 1');
    await pp.ensureOverrideExists();
    await pp.setDirective('Camera', 'All');

    await test.step('Site One primary host (:5000) reflects the host-level override', async () => {
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'camera', '*', { label: 'Site One Frontend (:5000)' });
    });

    await test.step('Site One CMS host (:5001) reflects the site-level override', async () => {
      await expectDirectiveValue(request, env.appOneCmsUrl, 'camera', '(self)', { label: 'Site One CMS (:5001)' });
    });

    await test.step('Site Two (:5002) keeps the global value', async () => {
      await expectDirectiveValue(request, env.appTwoUrl, 'camera', '()', { label: 'Site Two (:5002)' });
    });
  });
});
