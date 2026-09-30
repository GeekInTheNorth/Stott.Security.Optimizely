import { Page, expect } from '@playwright/test';

export async function loginToCms(page: Page, cmsUrl: string, username: string, password: string): Promise<void> {
  await page.goto(`${cmsUrl}/util/Login?ReturnUrl=%2Fepiserver%2Fcms`);

  const usernameField = page.locator('input[name="Username"], input#Username, input[name="UserName"], input#UserName').first();
  const passwordField = page.locator('input[name="Password"], input#Password').first();

  await usernameField.fill(username);
  await passwordField.fill(password);

  await page.locator('button[type="submit"], input[type="submit"]').first().click();

  // Match on the path only: the login page's own URL carries the CMS path in
  // its returnUrl query string, so a plain regex on the full URL matches too early.
  await page.waitForURL(url => !/\/util\/login/i.test(url.pathname), { timeout: 30_000 });
  await expect(page).not.toHaveURL(/util\/Login/i);
}
