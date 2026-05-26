import { APIRequestContext, expect } from '@playwright/test';

/**
 * Fetch the Content-Security-Policy header(s) from a host without following redirects.
 * The Stott middleware adds the header to every response (including 3xx), so it
 * works equally for a public homepage and a CMS host that redirects to /util/Login.
 *
 * CspOptimizer can emit multiple Content-Security-Policy headers when the value
 * crosses the ~8KB split threshold. Each directive (e.g. script-src) only ever
 * appears in one of those headers, so we concatenate every CSP header value with
 * "; " separators — the regex form `(?:^|;)\s*directive` callers use continues
 * to work because that join character is also the directive separator within a
 * single header.
 *
 * Enforcing headers are preferred over report-only when both are present.
 * Returns undefined if neither variant is present.
 */
export async function fetchCspHeader(request: APIRequestContext, url: string): Promise<string | undefined> {
  const response = await request.get(url, {
    ignoreHTTPSErrors: true,
    maxRedirects: 0,
    failOnStatusCode: false,
  });

  const all = response.headersArray();
  const enforcing = all.filter(h => h.name.toLowerCase() === 'content-security-policy');
  const reportOnly = all.filter(h => h.name.toLowerCase() === 'content-security-policy-report-only');
  const chosen = enforcing.length > 0 ? enforcing : reportOnly;
  if (chosen.length === 0) return undefined;

  return chosen
    .map(h => h.value.replace(/;\s*$/, ''))
    .join('; ');
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
  try {
    await expect
      .poll(
        async () => {
          lastSeen = await fetchCspHeader(request, url);
          return predicate(lastSeen);
        },
        { timeout: options.timeout ?? 10_000, message: options.message ?? `CSP header at ${url} did not satisfy predicate` },
      )
      .toBe(true);
    return lastSeen ?? '';
  } finally {
    console.log(`\n[CSP] ${options.label ?? url}\n  ${lastSeen ?? '(no CSP header)'}\n`);
  }
}
