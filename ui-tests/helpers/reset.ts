import { APIRequestContext, expect } from '@playwright/test';
import { env } from './env';

export async function resetSystem(request: APIRequestContext): Promise<void> {
  await reset(request);
}

/**
 * Reset, additionally seeding Permission Policy directives which have since been deprecated.
 * These can no longer be created through the UI, so they have to be seeded in order to test
 * that pre-existing configuration is retained, flagged and still applied to the response.
 */
export async function resetSystemWithDeprecatedDirectives(request: APIRequestContext): Promise<void> {
  await reset(request, '?includeDeprecatedDirectives=true');
}

async function reset(request: APIRequestContext, query = ''): Promise<void> {
  const url = `${env.appOneCmsUrl}/ui-tests/reset${query}`;
  const response = await request.get(url, { ignoreHTTPSErrors: true });
  expect(
    response.ok(),
    `Reset endpoint at ${url} returned ${response.status()} ${response.statusText()}`,
  ).toBe(true);
}
