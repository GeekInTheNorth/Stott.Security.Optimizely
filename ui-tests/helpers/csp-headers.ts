import { APIRequestContext, expect } from '@playwright/test';

/**
 * Fetch the Content-Security-Policy header from a host without following redirects.
 * The Stott middleware adds the header to every response (including 3xx), so it
 * works equally for a public homepage and a CMS host that redirects to /util/Login.
 *
 * Returns the raw CSP header value, or undefined if neither the enforcing nor
 * the report-only variant is present.
 */
export async function fetchCspHeader(request: APIRequestContext, url: string): Promise<string | undefined> {
  const response = await request.get(url, {
    ignoreHTTPSErrors: true,
    maxRedirects: 0,
    failOnStatusCode: false,
  });
  const headers = response.headers();
  return headers['content-security-policy'] ?? headers['content-security-policy-report-only'];
}

/**
 * Polls the given URL until the CSP header satisfies the predicate or times out.
 * Useful right after a settings change, because the addon's header cache
 * invalidates asynchronously.
 */
export async function expectCspHeader(
  request: APIRequestContext,
  url: string,
  predicate: (csp: string | undefined) => boolean,
  options: { timeout?: number; message?: string; label?: string } = {},
): Promise<string> {
  let lastSeen: string | undefined;
  await expect
    .poll(
      async () => {
        lastSeen = await fetchCspHeader(request, url);
        return predicate(lastSeen);
      },
      { timeout: options.timeout ?? 10_000, message: options.message ?? `CSP header at ${url} did not satisfy predicate` },
    )
    .toBe(true);

  console.log(`\n[CSP] ${options.label ?? url}\n  ${lastSeen ?? '(no CSP header)'}\n`);
  return lastSeen ?? '';
}
