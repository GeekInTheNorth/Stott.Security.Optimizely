import { APIRequestContext, expect } from '@playwright/test';
import { env } from './env';

export async function resetSystem(request: APIRequestContext): Promise<void> {
  const response = await request.get(`${env.appOneCmsUrl}/ui-tests/reset`, { ignoreHTTPSErrors: true });
  expect(
    response.ok(),
    `Reset endpoint at ${env.appOneCmsUrl}/ui-tests/reset returned ${response.status()} ${response.statusText()}`,
  ).toBe(true);
}
