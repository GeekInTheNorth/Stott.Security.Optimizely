import { test } from '@playwright/test';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { PermissionsPolicyPage } from '../helpers/permissions-policy-page';
import { expectDirectiveValue } from '../helpers/permissions-policy-headers';

test.describe('Permissions Policy scoping (Global)', () => {
  test.beforeEach(async ({ page }) => {
    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    const pp = new PermissionsPolicyPage(page, env.appOneCmsUrl);
    await pp.open();
    await pp.revertAllOverrides();
  });

  test('directives configured at the global level are applied to every host across applications', async ({ page, request }) => {
    const pp = new PermissionsPolicyPage(page, env.appOneCmsUrl);

    await pp.switchToGlobal();
    await pp.ensureHeaderEnabled();
    await pp.setDirective('Camera', 'ThisSite');
    await pp.setDirective('Geolocation', 'ThisSite');

    await test.step('App One frontend (:5000) inherits the global Permissions Policy', async () => {
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'camera', '(self)', { label: 'App One Frontend (:5000)' });
      await expectDirectiveValue(request, env.appOneFrontendUrl, 'geolocation', '(self)', { label: 'App One Frontend (:5000)' });
    });

    await test.step('App One CMS host (:5001) inherits the global Permissions Policy', async () => {
      await expectDirectiveValue(request, env.appOneCmsUrl, 'camera', '(self)', { label: 'App One CMS (:5001)' });
      await expectDirectiveValue(request, env.appOneCmsUrl, 'geolocation', '(self)', { label: 'App One CMS (:5001)' });
    });

    await test.step('App Two (:5002) inherits the global Permissions Policy', async () => {
      await expectDirectiveValue(request, env.appTwoUrl, 'camera', '(self)', { label: 'App Two (:5002)' });
      await expectDirectiveValue(request, env.appTwoUrl, 'geolocation', '(self)', { label: 'App Two (:5002)' });
    });
  });
});
