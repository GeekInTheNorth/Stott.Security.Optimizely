import { APIRequestContext, expect } from '@playwright/test';

/**
 * Fetch a single response header by name. Playwright normalises header names
 * to lowercase, so callers can pass any casing.
 *
 * Uses maxRedirects: 0 / failOnStatusCode: false so the helper works equally
 * for a public homepage and a CMS host that redirects to /util/Login — the
 * Stott middleware writes headers on every response, redirects included.
 */
export async function fetchResponseHeader(request: APIRequestContext, url: string, headerName: string): Promise<string | undefined> {
  const response = await request.get(url, {
    ignoreHTTPSErrors: true,
    maxRedirects: 0,
    failOnStatusCode: false,
  });
  return response.headers()[headerName.toLowerCase()];
}

/**
 * Poll a URL until the named header's value satisfies the predicate, then
 * console.log the captured value. The log runs in `finally` so the captured
 * header is visible on both pass and fail.
 */
export async function expectResponseHeader(
  request: APIRequestContext,
  url: string,
  headerName: string,
  predicate: (value: string | undefined) => boolean,
  options: { timeout?: number; label?: string; message?: string } = {},
): Promise<string | undefined> {
  let lastValue: string | undefined;
  try {
    await expect
      .poll(
        async () => {
          lastValue = await fetchResponseHeader(request, url, headerName);
          return predicate(lastValue);
        },
        {
          timeout: options.timeout ?? 10_000,
          message: options.message ?? `Header ${headerName} at ${url} did not satisfy predicate (last seen: ${lastValue ?? '(missing)'})`,
        },
      )
      .toBe(true);
    return lastValue;
  } finally {
    console.log(`\n[Header] ${options.label ?? url}\n  ${headerName}: ${lastValue ?? '(missing)'}\n`);
  }
}
