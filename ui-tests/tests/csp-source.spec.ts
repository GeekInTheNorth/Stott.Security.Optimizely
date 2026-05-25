import { test, expect } from '@playwright/test';
import { randomUUID } from 'crypto';
import { env } from '../helpers/env';
import { loginToCms } from '../helpers/auth';
import { CspSourcePage } from '../helpers/csp-page';
import { resetSystem } from '../helpers/reset';

function escapeRegex(value: string): string {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

test.describe('CSP source round-trip (Application One)', () => {
  test.beforeEach(async ({ request }) => {
    await resetSystem(request);
  });

  test('a source added in the CMS appears in the front-end Content-Security-Policy header', async ({ page, request }) => {
    const guidHost = `https://www.${randomUUID()}.com`;
    const cspPage = new CspSourcePage(page, env.appOneCmsUrl);

    await loginToCms(page, env.appOneCmsUrl, env.cmsUsername, env.cmsPassword);
    await cspPage.open();
    await cspPage.addSource(guidHost, ['script-src', 'script-src-elem']);

    const response = await request.get(env.appOneFrontendUrl, { ignoreHTTPSErrors: true });
    const headers = response.headers();
    const cspHeader = headers['content-security-policy'] ?? headers['content-security-policy-report-only'];

    expect(cspHeader, `Neither Content-Security-Policy nor Content-Security-Policy-Report-Only present. Headers: ${JSON.stringify(headers)}`).toBeDefined();
    expect(cspHeader).toMatch(new RegExp(`(?:^|;)\\s*script-src[^;]*${escapeRegex(guidHost)}`));
    expect(cspHeader).toMatch(new RegExp(`(?:^|;)\\s*script-src-elem[^;]*${escapeRegex(guidHost)}`));
  });
});
