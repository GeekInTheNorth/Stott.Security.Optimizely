import { Page, expect } from '@playwright/test';

export async function loginToCms(page: Page, cmsUrl: string, username: string, password: string): Promise<void> {
  await page.goto(`${cmsUrl}/util/Login?ReturnUrl=%2Foptimizely%2Fcms`);

  const usernameField = page.locator('input[name="Username"], input#Username, input[name="UserName"], input#UserName').first();
  const passwordField = page.locator('input[name="Password"], input#Password').first();

  await usernameField.fill(username);
  await passwordField.fill(password);

  await page.locator('button[type="submit"], input[type="submit"]').first().click();

  await page.waitForURL(/\/(optimizely|episerver)\//i, { timeout: 30_000 });
  await expect(page).not.toHaveURL(/util\/Login/i);
}
