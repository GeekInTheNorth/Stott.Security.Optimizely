import { APIRequestContext, expect } from '@playwright/test';

/**
 * Fetch the Permissions-Policy header from a URL without following redirects.
 * The Stott middleware adds the header to every response (including 3xx), so
 * it works for both the public homepage and a CMS host that redirects to login.
 */
export async function fetchPermissionsPolicyHeader(request: APIRequestContext, url: string): Promise<string | undefined> {
  const response = await request.get(url, {
    ignoreHTTPSErrors: true,
    maxRedirects: 0,
    failOnStatusCode: false,
  });
  return response.headers()['permissions-policy'];
}

/**
 * Extract the fragment for a single directive from a Permissions-Policy header.
 * The header is a comma-separated list of `name=value` items. The value is one
 * of `*`, `(self)`, `(self "url1" "url2")`, `("url1" "url2")`, or `()`.
 *
 * Returns the raw value portion (everything after the `=`), or undefined when
 * the directive is not present.
 */
export function getDirectiveValue(header: string | undefined, directive: string): string | undefined {
  if (!header) return undefined;

  // Walk top-level commas (ignore commas inside parentheses).
  const items: string[] = [];
  let depth = 0;
  let start = 0;
  for (let i = 0; i < header.length; i++) {
    const ch = header[i];
    if (ch === '(') depth++;
    else if (ch === ')') depth = Math.max(0, depth - 1);
    else if (ch === ',' && depth === 0) {
      items.push(header.slice(start, i).trim());
      start = i + 1;
    }
  }
  items.push(header.slice(start).trim());

  for (const item of items) {
    const eq = item.indexOf('=');
    if (eq < 0) continue;
    const name = item.slice(0, eq).trim();
    if (name === directive) {
      return item.slice(eq + 1).trim();
    }
  }
  return undefined;
}

/**
 * Poll the URL until the named directive equals the expected fragment, then
 * console.log the captured header. Mirrors expectCspHeader's UX.
 */
export async function expectDirectiveValue(
  request: APIRequestContext,
  url: string,
  directive: string,
  expected: string,
  options: { timeout?: number; label?: string; message?: string } = {},
): Promise<void> {
  let lastHeader: string | undefined;
  let lastValue: string | undefined;
  try {
    await expect
      .poll(
        async () => {
          lastHeader = await fetchPermissionsPolicyHeader(request, url);
          lastValue = getDirectiveValue(lastHeader, directive);
          return lastValue;
        },
        {
          timeout: options.timeout ?? 10_000,
          message:
            options.message
            ?? `Permissions-Policy at ${url}: expected ${directive}=${expected} (full header: ${lastHeader ?? '(none)'})`,
        },
      )
      .toBe(expected);
  } finally {
    console.log(`\n[Permissions-Policy] ${options.label ?? url}\n  ${directive}=${lastValue ?? '(missing)'}\n  full: ${lastHeader ?? '(none)'}\n`);
  }
}
