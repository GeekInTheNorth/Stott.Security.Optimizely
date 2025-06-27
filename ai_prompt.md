# AI Prompt for Stott.Security.Optimizely

## Project Overview
This solution provides advanced security headers, CORS, and CSP management for Optimizely CMS sites...

## Architecture
- `src/Stott.Security.Optimizely/`: Contains server side business logic and feature modules such as Audit, Caching, Cors, Csp, etc.
- `Entities/`: Data models for security settings and audit logs.
- `ServiceExtensions/`: Startup and DI configuration.
- The back end in `src/Stott.Security.Optimizely` is built to compile for .NET 6, .NET 8 and .NET 9 which means conventions must match .NET 6 for maximum compatibility
- `src/Stott.Security.React/` contains the old UI that is currently being migrated which is built using Create React App
- `src/Stott.Security.UI/` contains the new UI which is being built using React and Vite
- `samples/` contains demo sites for testing the logic within `src/Stott.Security.Optimizely/` and should not be considered as production code.

## Conventions
- Use PascalCase for class names, camelCase for variables.
- All controllers inherit from `BaseController`.
- Tests are in `Stott.Security.Optimizely.Test`.

## Key Workflows
- To add a new security feature, create a module in `Features/` and register it in `StartupExtensions.cs`.
- To run tests: `dotnet test src/Stott.Security.Optimizely.Test/`

## Security
- All HTTP headers must be validated.
- CSP and CORS settings are managed via the admin UI and persisted in the database.

## AI Usage
- Always suggest unit tests for new code.
- Prioritize security best practices.
- Do not auto-modify migration files.

## Gotchas
- The Csp module uses custom serialization logic.