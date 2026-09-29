# Stott.Security.Optimizely — UI / Integration Tests

End-to-end tests that drive the **OptimizelyTwelveTest** sample CMS in a real browser, persist settings via the Stott Security admin UI, and then assert the compiled HTTP response headers on the public site match. These complement the 1,800+ NUnit unit tests by covering the React UI, the API, the middleware pipeline, and the SQL DB as a single system.

## Stack

- **Playwright** (Node + TypeScript) — single browser project (Chromium) by default.
- Auto-starts the Sample CMS via Playwright's `webServer` config (`dotnet run` against `Sample/OptimizelyTwelveTest`). If the app is already running locally, Playwright will reuse it.

## Prerequisites

- Node.js 18+
- .NET SDK 10 (matches the Sample app's target framework)
- SQL Server LocalDB (the Sample project's default `EPiServerDB` connection)
- A registered admin user in the Sample CMS (see **First-run setup** below)

## First-run setup

1. **Register an admin user once.** The Sample app uses `RegisterAdminUserBehaviors.Enabled`, so the first time you hit the CMS it presents a registration screen.

   ```powershell
   dotnet run --project ..\Sample\OptimizelyTwelveTest\OptimizelyTwelveTest.csproj
   ```

   Open `https://localhost:5001/episerver/cms` (you may need to trust the dev cert), complete the admin registration form, then stop the app. The credentials are now persisted in the LocalDB instance.

2. **Install Node deps and browsers.**

   ```powershell
   cd ui-tests
   npm install
   npm run install-browsers
   ```

3. **Provide credentials.** Copy `.env.example` to `.env` and fill in the admin username/password you just registered:

   ```powershell
   Copy-Item .env.example .env
   notepad .env
   ```

   `.env` is gitignored.

## Running the tests

```powershell
# Headless run (Playwright will start the Sample app if it isn't already)
npm test

# Watch a run in a real browser window
npm run test:headed

# Inspect failures
npm run report

# Step through a single spec interactively
npm run test:debug
```

If you already have the Sample app running, Playwright reuses it (`reuseExistingServer: true` outside CI).

## Layout

```
ui-tests/
├── playwright.config.ts   # webServer, baseURL, ignoreHTTPSErrors, single chromium project
├── tests/
│   └── csp-source.spec.ts # Test 1 — CSP source round-trip
└── helpers/
    ├── env.ts             # Typed env-var access
    ├── auth.ts            # CMS login helper
    └── csp-page.ts        # Page object for the CSP sources screen
```

## Test 1: CSP source round-trip

`tests/csp-source.spec.ts` does the following against Application One (frontend `:5000`, CMS `:5001`):

1. Logs into the CMS as the configured admin user.
2. Navigates to `/stott.security.optimizely/administration/#csp-source`.
3. Adds a CSP source with a unique URL (`https://www.<guid>.com`) granting `script-src` and `script-src-elem`.
4. Issues an HTTP `GET` against `https://localhost:5000` and asserts the `Content-Security-Policy` (or `…-Report-Only`) header contains the new URL inside both directives.
5. **Cleanup:** opens the source list and deletes the row it added (runs in a `finally` block so cleanup happens even if assertions fail).

The URL contains a fresh GUID per run, so repeated runs don't collide if cleanup doesn't complete.

## Adding more tests

To extend the suite (Permissions Policy, Custom Headers, multi-domain checks across `:5001` and `:5002`):

- Add a new spec under `tests/`.
- Add a new page object under `helpers/` if it's a new admin screen.
- Reuse `loginToCms()` from `helpers/auth.ts`.

If login becomes a bottleneck (it currently runs per test), a future improvement is to switch to a global setup that captures `storageState` once and shares it across tests.

## Troubleshooting

- **"Missing required environment variable"** — copy `.env.example` to `.env` and fill it in.
- **Login fails** — confirm the admin user was registered in the Sample CMS and the password is correct in `.env`. Try logging in manually at `https://localhost:5001/util/Login`.
- **`webServer` times out** — the cold start can be slow on first run (DB migrations seed three Optimizely sites). Bump `timeout` in `playwright.config.ts` if needed, or pre-start the app yourself.
- **Cert warnings** — Playwright already passes `ignoreHTTPSErrors`. If browsers still prompt, trust the dev cert: `dotnet dev-certs https --trust`.
- **CSP header missing entirely** — check `tbl_CspSettings.IsEnabled = 1` in the LocalDB. The diagnostic assertion in the test dumps the full header set when neither CSP header is present.
