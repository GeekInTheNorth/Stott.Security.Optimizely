# Stott.Security.Optimizely - Claude Code Guide

A comprehensive guide for Claude Code instances working in this codebase.

## Table of Contents

- [Quick Start](#quick-start)
- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Key Patterns](#key-patterns)
- [Critical Files](#critical-files)
- [Common Workflows](#common-workflows)
- [Testing](#testing)
- [Configuration](#configuration)
- [Database Schema](#database-schema)
- [API Endpoints](#api-endpoints)
- [Debugging](#debugging)
- [Technical Decisions](#technical-decisions)

---

## Quick Start

### Essential Commands

```bash
# Build the solution
dotnet build

# Run all tests (1,573 tests as of 2026-02-06)
dotnet test

# Run specific test categories
dotnet test --filter "FullyQualifiedName~CspOptimizerTests"
dotnet test --filter "FullyQualifiedName~AuditRepositoryTests"

# Build React UI
cd src/Stott.Security.Ui
npm install
npm run build-dotnet  # Compiles and copies to Static/ folder

# Run sample site
cd Sample
dotnet run
```

### Project Structure

```
d:\Projects\Stott\Stott.Security.Optimizely\
├── src/
│   ├── Stott.Security.Optimizely/          # Main Razor Class Library
│   │   ├── Entities/                        # Domain models + audit entities
│   │   ├── Features/                        # Feature-organized business logic
│   │   │   ├── Csp/                         # CSP generation, optimization, reporting
│   │   │   ├── Cors/                        # CORS policy management
│   │   │   ├── CustomHeaders/               # Response headers (standard + custom)
│   │   │   ├── PermissionPolicy/            # Permissions-Policy header
│   │   │   ├── Header/                      # HeaderCompilationService (orchestrator)
│   │   │   ├── Audit/                       # Audit trail
│   │   │   ├── Middleware/                  # SecurityHeaderMiddleware
│   │   │   └── Configuration/               # DI setup
│   │   ├── Migrations/                      # 13 EF Core migrations
│   │   └── Static/                          # Compiled React assets (embedded resources)
│   ├── Stott.Security.Optimizely.Test/     # NUnit test project (49 test files, 1,573 tests)
│   └── Stott.Security.Ui/                   # React 19 + Vite frontend (39 .jsx files)
└── Sample/                                  # Demo CMS instance for development
```

### Git Information

- **Current Branch**: `feature/custom_headers_with_claude`
- **Main Branch**: `main`
- **Recent Work**: Custom Headers feature (replacing deprecated Security Headers), CMS 13 migration

---

## Project Overview

**Purpose**: Optimizely CMS 12+ addon for managing security response headers with a user-friendly interface.

**Key Innovation**: Domain-based CSP management (users grant permissions to domains) with intelligent header optimization and splitting to prevent CDN issues at 8KB/16KB thresholds.

**Target Framework**: Multi-targets .NET 6.0, 8.0, 9.0, 10.0

**Repository**: [GeekInTheNorth/Stott.Security.Optimizely](https://github.com/GeekInTheNorth/Stott.Security.Optimizely)

**Key Features**:
- Content Security Policy (CSP) management with automatic header splitting
- CORS configuration
- Permission Policy management
- Response Headers management (standard security headers + custom headers with add/remove behavior)
- Automatic audit trail for all changes
- React-based admin UI embedded in CMS

---

## Architecture

### 3-Tier Architecture

1. **Data Layer**: EF Core with SQL Server
   - [CspDataContext.cs](src/Stott.Security.Optimizely/Entities/CspDataContext.cs) - DbContext with automatic audit interception
   - Repositories follow feature-based organization

2. **Business Logic**: Feature-organized services
   - Each feature has its own folder: `Features/{FeatureName}/`
   - Pattern: Repository → Service → Controller
   - Example: [CspService.cs](src/Stott.Security.Optimizely/Features/Csp/CspService.cs)

3. **Presentation Layer**: React UI + REST APIs
   - React components in [Stott.Security.Ui/](src/Stott.Security.Ui/)
   - API controllers in each feature folder
   - Compiled React assets embedded as .NET resources

### Request Flow

```
HTTP Request
    ↓
[SecurityHeaderMiddleware] (intercepts response)
    ↓
[HeaderCompilationService] (orchestrates header generation)
    ↓
├─→ [CspService] (generates CSP headers)
│   └─→ [CspOptimizer] (splits headers if too large)
├─→ [PermissionPolicyService] (generates Permission-Policy)
└─→ [CustomHeaderService] (generates response headers: standard + custom)
    ↓
Headers applied to HTTP Response (add or remove based on behavior)
```

### Feature-Based Organization

Each security feature follows consistent structure:

```
Features/{FeatureName}/
├── Repository/        # Data access (EF Core)
├── Service/           # Business logic
├── Controller/        # REST API endpoints
└── Models/            # DTOs and view models
```

**Example**: CSP feature
- [CspSettings.cs](src/Stott.Security.Optimizely/Entities/CspSettings.cs) - Entity
- [CspSettingsRepository.cs](src/Stott.Security.Optimizely/Features/Csp/Settings/Repository/CspSettingsRepository.cs) - Data access
- [CspService.cs](src/Stott.Security.Optimizely/Features/Csp/CspService.cs) - Business logic
- [CspSettingsController.cs](src/Stott.Security.Optimizely/Features/Csp/Settings/CspSettingsController.cs) - API

---

## Key Patterns

### 1. Automatic Audit Trail

**ALL entity changes are automatically audited** via DbContext interception.

**How it works**:
- All domain entities implement [IAuditableEntity](src/Stott.Security.Optimizely/Features/Audit/IAuditableEntity.cs)
- [CspDataContext.SaveChangesAsync()](src/Stott.Security.Optimizely/Entities/CspDataContext.cs) intercepts saves
- Creates `AuditHeader` + `AuditProperty` records for every change
- Tracks: who, what, when, old value → new value

**Pattern**:
```csharp
public interface IAuditableEntity
{
    Guid Id { get; set; }
    DateTime Modified { get; set; }
    string ModifiedBy { get; set; }
}
```

**Implementation**: [CspDataContext.cs](src/Stott.Security.Optimizely/Entities/CspDataContext.cs) overrides `SaveChangesAsync()` to:
1. Enumerate `ChangeTracker.Entries<IAuditableEntity>()`
2. Create `AuditHeader` records with operation type (Create/Update/Delete)
3. Create `AuditProperty` records capturing field-level changes (OldValue → NewValue)
4. Attach user identity from `ModifiedBy` property

**Audit Cleanup**:
- [AuditCleanupScheduledJob](src/Stott.Security.Optimizely/Features/Audit/AuditCleanupScheduledJob.cs) runs weekly
- Deletes records older than retention period (default: 2 years)
- Batch deletion: 1000 records per run (prevents long-running transactions)
- Configured via [SecurityConfiguration.AuditRetentionPeriod](src/Stott.Security.Optimizely/Features/Configuration/SecurityConfiguration.cs)
- Orphan prevention: [AuditRepository.DeleteAsync()](src/Stott.Security.Optimizely/Features/Audit/AuditRepository.cs) explicitly deletes child `AuditProperty` records before parent `AuditHeader` records
- Oldest records deleted first (ordered by `Actioned` date)

### 2. CSP Header Splitting

**Problem**: CDNs break with headers > 8KB; critical failure at 16KB.

**Solution**: [CspOptimizer.cs](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs) automatically splits large CSP headers.

**Thresholds**:
```csharp
SplitThreshold = 8100 bytes      // Start splitting
SimplifyThreshold = 12000 bytes  // Force directive collapse
TerminalThreshold = 15500 bytes  // Give up (log error)
```

**Algorithm** (Greedy Bin-Packing):

1. **Pre-Calculate Sizes**: Each directive's predicted size = `Directive.Length + 3 + Σ(Source.Length + 1)`
   - Accounts for separators (`;` between directives, spaces between sources)
   - Reduces threshold by ~38 bytes per nonce directive

2. **Directive Grouping** (priority order):
   - Frame-source group: `fenced-frame-src`, `frame-src`, `worker-src`, `child-src`
   - Script-source group: `script-src-elem`, `script-src-attr`, `script-src`
   - Style-source group: `style-src-elem`, `style-src-attr`, `style-src`
   - Fetch directives: `connect-src`, `font-src`, `img-src`, etc.
   - Standalone: `base-uri`, `form-action`, `frame-ancestors`, `sandbox`

3. **Splitting Strategy**:
   - If group fits → add to current CSP header
   - If group exceeds threshold → simplify (collapse to primary directive)
   - Always preserve `default-src` in each header
   - Always include `report-to` directive for violation tracking

4. **Output**: Array of CSP header strings (typically 1, rarely 2-3)

**Key Method**: [CspOptimizer.GroupDirectives()](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs#L64-L93)

### 3. CSP Generation Pipeline

**Entry Point**: [CspService.cs](src/Stott.Security.Optimizely/Features/Csp/CspService.cs)

**Generation Steps**:
1. **Source Aggregation**:
   - Global CSP settings (IsEnabled, IsReportOnly, etc.)
   - Global CSP sources (`CspSource` table: domain + allowed directives)
   - Page-specific sources (via `IContentSecurityPolicyPage` interface)
   - Sandbox directives (`CspSandbox` table)

2. **Route-Based Filtering**:
   - Automatically removes `'unsafe-inline'`, nonces, hashes for admin paths
   - Route types: `NoNonceOrHash` (admin), `ContentSpecific` (pages)
   - Exclusion paths configured in `SecurityConfiguration`

3. **Directive Construction**:
   - Groups sources by directive
   - Inherits from `default-src` when specific directive not defined
   - Deduplicates and sorts sources for consistency
   - Handles special keywords: `'self'`, `'unsafe-inline'`, `'none'`

4. **Optimization & Splitting** (see CSP Header Splitting above)

### 4. Custom Headers (Response Headers)

**Problem**: The old `SecurityHeaders` feature used enum-based configuration with separate controllers for different header types, making it inflexible and unable to handle custom headers.

**Solution**: The `CustomHeaders` feature provides a unified system where all headers (standard and custom) are managed via a single entity with three behaviors.

**CustomHeaderBehavior Enum**:
- `Disabled` (0) - Header not processed (default for standard headers)
- `Add` (1) - Include header in HTTP response
- `Remove` (2) - Actively remove header from HTTP response

**8 Standard Headers** are automatically provided as defaults via `CustomHeaderMapper.FixedHeaders`:
- X-XSS-Protection, X-Frame-Options, X-Content-Type-Options, Referrer-Policy
- Cross-Origin-Embedder-Policy, Cross-Origin-Opener-Policy, Cross-Origin-Resource-Policy
- Strict-Transport-Security (with specialized HSTS editor in UI)

**Architecture**:
- [CustomHeader.cs](src/Stott.Security.Optimizely/Entities/CustomHeader.cs) - Entity (table: `tbl_CspCustomHeader`)
- [CustomHeaderRepository.cs](src/Stott.Security.Optimizely/Features/CustomHeaders/Repository/CustomHeaderRepository.cs) - Data access with upsert logic
- [CustomHeaderMapper.cs](src/Stott.Security.Optimizely/Features/CustomHeaders/Repository/CustomHeaderMapper.cs) - Enriches entities with UI metadata (descriptions, allowed values, property types)
- [CustomHeaderService.cs](src/Stott.Security.Optimizely/Features/CustomHeaders/Service/CustomHeaderService.cs) - Business logic with caching
- [CustomHeaderController.cs](src/Stott.Security.Optimizely/Features/CustomHeaders/CustomHeaderController.cs) - REST API

**Caching**: Compiled headers cached with key `"stott.security.customheaders"`, invalidated on any save/delete.

**UI Property Types**: The mapper returns a `PropertyType` hint for each header:
- `"string"` - Free-text input (custom headers)
- `"select"` - Dropdown with predefined values (standard headers)
- `"hsts"` - Specialized HSTS editor with max-age slider and checkboxes

### 5. Header Compilation & Caching

- Headers compiled once per route type and cached
- [HeaderCompilationService.cs](src/Stott.Security.Optimizely/Features/Header/HeaderCompilationService.cs) handles orchestration
- Nonce values generated per request
- `{NONCE}` placeholder substituted in cached headers
- HSTS header only sent over HTTPS connections

**Flow**: Request → [SecurityHeaderMiddleware.cs](src/Stott.Security.Optimizely/Features/Middleware/SecurityHeaderMiddleware.cs) → [HeaderCompilationService.cs](src/Stott.Security.Optimizely/Features/Header/HeaderCompilationService.cs) → Response

**HeaderDto**: Each compiled header carries `Key`, `Value`, and `IsRemoval` flag. The middleware either appends or removes headers based on `IsRemoval`.

**Performance**: Headers compiled once and cached; only nonce substitution per request.

---

## Critical Files

### Most Important Files

| File | Purpose | When to Modify |
|------|---------|----------------|
| [CspService.cs](src/Stott.Security.Optimizely/Features/Csp/CspService.cs) | CSP generation orchestration | Adding CSP features |
| [CspOptimizer.cs](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs) | Header splitting algorithm | CSP optimization changes |
| [HeaderCompilationService.cs](src/Stott.Security.Optimizely/Features/Header/HeaderCompilationService.cs) | Main compilation + caching | Header generation flow |
| [SecurityHeaderMiddleware.cs](src/Stott.Security.Optimizely/Features/Middleware/SecurityHeaderMiddleware.cs) | HTTP middleware hook | Request interception |
| [CspDataContext.cs](src/Stott.Security.Optimizely/Entities/CspDataContext.cs) | DbContext with audit interception | Database schema changes |
| [SecurityServiceExtensions.cs](src/Stott.Security.Optimizely/Features/Configuration/SecurityServiceExtensions.cs) | DI configuration | Startup configuration |
| [CustomHeaderService.cs](src/Stott.Security.Optimizely/Features/CustomHeaders/Service/CustomHeaderService.cs) | Response headers business logic | Custom/standard header changes |
| [CustomHeaderMapper.cs](src/Stott.Security.Optimizely/Features/CustomHeaders/Repository/CustomHeaderMapper.cs) | Standard header metadata | Adding standard header types |

### Data Layer

| File | Purpose |
|------|---------|
| [CspDataContext.cs](src/Stott.Security.Optimizely/Entities/CspDataContext.cs) | EF Core DbContext with audit interception |
| [CspSettingsRepository.cs](src/Stott.Security.Optimizely/Features/Csp/Settings/Repository/CspSettingsRepository.cs) | Settings data access |
| [CspPermissionRepository.cs](src/Stott.Security.Optimizely/Features/Csp/Permissions/Repository/CspPermissionRepository.cs) | Source/permission data access |
| [AuditRepository.cs](src/Stott.Security.Optimizely/Features/Audit/AuditRepository.cs) | Audit data access |

### API Controllers

| File | Purpose |
|------|---------|
| [CspPermissionsController.cs](src/Stott.Security.Optimizely/Features/Csp/Permissions/CspPermissionsController.cs) | CSP source management API |
| [CustomHeaderController.cs](src/Stott.Security.Optimizely/Features/CustomHeaders/CustomHeaderController.cs) | Response headers API (standard + custom) |
| [AuditController.cs](src/Stott.Security.Optimizely/Features/Audit/AuditController.cs) | Audit history API |

### Configuration Files

| File | Purpose |
|------|---------|
| [SecuritySetupOptions.cs](src/Stott.Security.Optimizely/Features/Configuration/SecuritySetupOptions.cs) | User-provided setup options at startup |
| [SecurityConfiguration.cs](src/Stott.Security.Optimizely/Features/Configuration/SecurityConfiguration.cs) | Runtime configuration singleton |
| [CspConstants.cs](src/Stott.Security.Optimizely/Common/CspConstants.cs) | Constants (auth policy, directives, thresholds) |

### React UI Entry Points

| File | Purpose |
|------|---------|
| [App.jsx](src/Stott.Security.Ui/src/App.jsx) | Main component with hash-based navigation |
| [StottSecurityContext.jsx](src/Stott.Security.Ui/src/Context/StottSecurityContext.jsx) | State management + HTTP service |
| [CustomHeadersContainer.jsx](src/Stott.Security.Ui/src/CustomHeaders/CustomHeadersContainer.jsx) | Response headers management UI |
| [PermissionList.jsx](src/Stott.Security.Ui/src/CSP/PermissionList.jsx) | CSP domain source management UI |

---

## Common Workflows

### Adding a New Test

1. Create test file in [src/Stott.Security.Optimizely.Test/](src/Stott.Security.Optimizely.Test/)
2. Follow NUnit + Moq pattern
3. Use `TestDataContextFactory` for in-memory database
4. Run tests: `dotnet test`

**Example test structure**:
```csharp
[TestFixture]
public class MyFeatureTests
{
    private Mock<IDependency> _mockDependency;
    private MyFeatureService _service;

    [SetUp]
    public void SetUp()
    {
        _mockDependency = new Mock<IDependency>();
        _service = new MyFeatureService(_mockDependency.Object);
    }

    [Test]
    public void MethodName_GivenCondition_ThenExpectedOutcome()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

### Modifying CSP Logic

**When**: Adding new CSP directives or changing splitting behavior

**Steps**:
1. Update [CspOptimizer.cs](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs) - `GroupDirectives()` method
2. Add constants to [CspConstants.cs](src/Stott.Security.Optimizely/Common/CspConstants.cs)
3. Update [CspOptimizerTests.cs](src/Stott.Security.Optimizely.Test/Csp/CspOptimizerTests.cs)
4. Run tests: `dotnet test --filter "FullyQualifiedName~CspOptimizerTests"`

### Adding a New Standard Response Header

Standard security headers are now managed through the Custom Headers feature. To add a new standard header with predefined values:

1. **Add header name** to `CspConstants.HeaderNames`
2. **Add to FixedHeaders array** in [CustomHeaderMapper.cs](src/Stott.Security.Optimizely/Features/CustomHeaders/Repository/CustomHeaderMapper.cs)
3. **Add allowed values** in `CustomHeaderMapper.GetAllowedValues()`
4. **Add description** in `CustomHeaderMapper.GetDescriptionForHeaderName()`
5. **Set property type** in `CustomHeaderMapper.GetPropertyType()` (`"select"`, `"string"`, or `"hsts"`)
6. **Write tests** in [Stott.Security.Optimizely.Test/](src/Stott.Security.Optimizely.Test/)

No database changes, new entities, or new API endpoints are needed - the existing Custom Headers infrastructure handles everything.

### Modifying React UI

1. Navigate to React project: `cd src/Stott.Security.Ui`
2. Make changes to `.jsx` files
3. Build: `npm run build-dotnet` (compiles and copies to Static/)
4. Rebuild main project: `cd ../Stott.Security.Optimizely && dotnet build`
5. Test in sample site: `cd ../../Sample && dotnet run`

---

## Testing

### Framework & Organization

- **Framework**: NUnit + Moq
- **49 test files** with **1,573 test cases** (as of 2026-02-06)
- In-memory database via `TestDataContextFactory`
- Parameterized tests in separate `*TestCases.cs` files

### Coverage Areas

- CSP generation and optimization (43 tests in CspOptimizerTests)
- CORS policy provider and configuration
- Header compilation service
- Permission policy mapping and services
- Audit record creation and cleanup (18 tests)
- Validation rules (source rules, model validation)
- Controller authorization standards

### Running Tests

```bash
# All tests
dotnet test

# Specific test class
dotnet test --filter "FullyQualifiedName~CspOptimizerTests"

# Multiple test classes
dotnet test --filter "FullyQualifiedName~AuditRepositoryTests|FullyQualifiedName~AuditCleanupScheduledJobTests"

# Tests in specific namespace
dotnet test --filter "FullyQualifiedName~Stott.Security.Optimizely.Test.Features.Csp"
```

---

## Configuration

### Startup Configuration

**File**: [SecurityServiceExtensions.cs](src/Stott.Security.Optimizely/Features/Configuration/SecurityServiceExtensions.cs)

```csharp
// In Startup.cs or Program.cs
services.AddStottSecurity(
    cspSetupOptions => {
        cspSetupOptions.ConnectionStringName = "EPiServerDB";
        cspSetupOptions.NonceHashExclusionPaths.Add("/custom-admin-path");
        cspSetupOptions.AuditRetentionPeriod = TimeSpan.FromDays(90); // Default: 730 days
    },
    authorizationOptions => {
        authorizationOptions.AddPolicy(CspConstants.AuthorizationPolicy,
            policy => policy.RequireRole("WebAdmins", "CustomRole"));
    }
);

app.UseStottSecurity(); // Adds middleware (must be before UseEndpoints)
```

### Default Settings

| Setting | Default | Notes |
|---------|---------|-------|
| Connection String Name | EPiServerDB | SQL Server connection |
| Allowed Roles | WebAdmins, CmsAdmins, Administrator | API access roles |
| Audit Retention Period | 730 days (2 years) | Configurable via `AuditRetentionPeriod` |
| Audit Batch Size | 1000 records | Defined in `CspConstants.AuditDeletionBatchSize` |
| Nonce Exclusion Paths | `/episerver`, `/ui`, `/util` | CMS admin paths |

### Extensibility Points

1. **Per-Page CSP Override**:
   ```csharp
   public class MyPage : PageData, IContentSecurityPolicyPage {
       [EditorDescriptor(typeof(CspSourceMappingEditorDescriptor))]
       public virtual IList<PageCspSourceMapping> ContentSecurityPolicySources { get; set; }
   }
   ```

2. **Custom Nonce Provider**: Implement `INonceProvider` interface

3. **Custom CSP Report URL Resolver**: Implement `ICspReportUrlResolver`

4. **Agency Allow List**: Configure remote URL for centralized CSP source updates

5. **Custom CORS Policies**: Add via standard .NET CORS configuration

### Important Constants

**File**: [CspConstants.cs](src/Stott.Security.Optimizely/Common/CspConstants.cs)

**Key Values**:
- `AuthorizationPolicy = "StottSecurityPolicy"` - Required role for API access
- `AuditDeletionBatchSize = 1000` - Batch size for audit cleanup
- CSP directive constants: `ScriptSrc`, `StyleSrc`, `DefaultSrc`, etc.
- CSP source keywords: `Self`, `None`, `UnsafeInline`, `UnsafeEval`, `StrictDynamic`
- Default nonce/hash exclusion paths: `/episerver`, `/ui`, `/util`

---

## Database Schema

**Context**: [CspDataContext.cs](src/Stott.Security.Optimizely/Entities/CspDataContext.cs)

**Key Tables**:

| Table | Purpose | Key Fields |
|-------|---------|-----------|
| `tbl_CspSettings` | Global CSP configuration | IsEnabled, IsReportOnly, IsAllowListEnabled, UseStrictDynamic |
| `tbl_CspSource` | CSP sources (domains) | Source (domain), Directives (CSV of allowed directives) |
| `tbl_CspSandbox` | CSP sandbox settings | 15 boolean flags (AllowScripts, AllowSameOrigin, etc.) |
| `tbl_CspCustomHeader` | Response headers (standard + custom) | HeaderName, Behavior (Disabled/Add/Remove), HeaderValue |
| `tbl_CspSecurityHeaderSettings` | Legacy standard headers (deprecated, preserved) | No longer actively used by services |
| `tbl_CspCorsSettings` | CORS configuration | AllowedOrigins, AllowedMethods, AllowedHeaders |
| `tbl_CspPermissionPolicySettings` | Permissions-Policy | Per-directive configurations |
| `tbl_CspAuditHeader` | Audit headers | RecordType, OperationType, Identifier, ActionedBy, Actioned |
| `tbl_CspAuditProperty` | Audit details | AuditHeaderId, Field, OldValue, NewValue |

**Migrations**: 13 migrations track schema evolution from initial setup through Custom Headers.

---

## API Endpoints

**Base Path**: `/stott.security.optimizely/api/`

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/cspsettings/` | GET/POST | Global CSP settings |
| `/csppermissions/` | GET/POST/DELETE | CSP source management |
| `/csppermissions/append` | POST | Bulk add sources |
| `/cspreporting/` | GET | CSP violation reports |
| `/cspsandbox/` | GET/POST | Sandbox directives |
| `/corsconfiguration/` | GET/POST | CORS configuration |
| `/customheader/list` | GET | List response headers (filterable by name/behavior) |
| `/customheader/save` | POST | Create or update a response header |
| `/customheader/delete` | DELETE | Delete a response header |
| `/permission-policy/source/` | GET/POST | Permissions-Policy directive sources |
| `/permission-policy/settings/` | GET/POST | Permissions-Policy enable/disable |
| `/audit/` | GET | Audit history (paginated) |
| `/compiled-headers/` | GET | Preview compiled headers (headless API) |
| `/securitytxt/` | GET/POST/DELETE | Security.txt file management |
| `/migration/` | GET/POST | Export/import settings (tools) |

**Authorization**: All endpoints require authenticated user with role in `CspConstants.AuthorizationPolicy` (default: CmsAdmins, Administrator, WebAdmins).

---

## Debugging

### CSP Issues

**Enable detailed logging**:
- Check [SecurityHeaderMiddleware.cs](src/Stott.Security.Optimizely/Features/Middleware/SecurityHeaderMiddleware.cs) for errors
- Review [CspOptimizer.cs](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs) for size threshold warnings
- CSP violations logged to configured `report-to` endpoint

**Common Issues**:
- **CSP not applied**: Check `IsEnabled` in CspSettings table
- **Too restrictive**: Review global sources + page-specific sources
- **Nonce not working**: Verify route not in exclusion paths
- **Header too large**: Review source count, simplify domains

### Test Failures

1. Check test output for specific failure
2. Run single test: `dotnet test --filter "FullyQualifiedName~TestName"`
3. Verify in-memory database setup in test `SetUp` method
4. Check for async/await issues in repository tests

### Build Issues

1. Clean solution: `dotnet clean`
2. Rebuild: `dotnet build`
3. Check for React build errors: `cd src/Stott.Security.Ui && npm run build-dotnet`
4. Verify multi-targeting (builds for .NET 6.0, 8.0, 9.0, 10.0)

---

## Technical Decisions

### Why Domain-Based CSP (Not Raw Directives)?

**Rationale**: Non-technical users understand "allow scripts from google.com" better than "add `https://google.com` to `script-src`".

**Trade-off**: Less granular control, but significantly more accessible to content editors and marketers.

### Why Split Headers Instead of Simplifying?

**Rationale**: Maintains maximum security granularity while working within CDN constraints.

**Alternative Considered**: Always use simplified directives - rejected as too permissive.

### Why Automatic Audit Trail?

**Rationale**: Security header changes impact site security posture; compliance requires knowing who changed what and when.

**Implementation**: DbContext interception ensures audit cannot be bypassed.

### Why Embedded React Assets?

**Rationale**: Simplifies deployment (single NuGet package), eliminates separate frontend hosting.

**Trade-off**: Increases DLL size, but acceptable for admin UI.

### Why Consolidate Security Headers into Custom Headers?

**Rationale**: The old `SecurityHeaders` feature used 7 separate enums and 3 separate API endpoints for 8 standard headers. Custom Headers unifies management into a single table with a behavior enum (Disabled/Add/Remove), enabling:
- Any number of custom headers (not just the 8 standard ones)
- Header removal capability (e.g., suppress `Server` or `X-Powered-By`)
- Single UI with card-based layout instead of scattered forms
- Consistent audit trail for all header changes

**Trade-off**: Lost compile-time type safety of enums, but gained flexibility and extensibility. Standard headers still have validated dropdown values via `CustomHeaderMapper`.

### Why ServiceLocator in HeaderCompilationService?

**Rationale**: Legacy Optimizely pattern for lazy-loaded dependencies; avoids circular dependency issues with middleware registration.

**Note**: Not ideal for testability, but pragmatic given Optimizely's DI container constraints.

---

## React Frontend

**Build Pipeline**: Vite → `npm run build-dotnet` → Compiled to [Static/](src/Stott.Security.Optimizely/Static/) → Embedded as .NET resources

**Output Files**:
- `index-{hash}.css` (~254KB minified)
- `index-{hash}.js` (~551KB minified)

**Architecture**:
- **App.jsx**: Hash-based navigation (`#csp-settings`, `#response-headers`, `#cors-settings`, etc.)
- **StottSecurityContext.jsx**: Centralized state management, HTTP service, toast notifications
- **Feature Components**: CSP (9 components), CORS (5), Response Headers (4), Permissions Policy (4), Security.txt (3), Audit (1), Tools (3), Preview (1)

**Navigation Keys** (hash routes):
`csp-settings`, `csp-sandbox`, `csp-source`, `csp-violations`, `cors-settings`, `response-headers`, `permissions-policy`, `audit-history`, `header-preview`, `tools`, `security-txt`

**API Communication**: All via Axios to `/stott.security.optimizely/api/*` endpoints

**UI Patterns**:
- Card-based layouts for header and policy management
- Modal dialogs for add/edit operations with shared [ConfirmationModal.jsx](src/Stott.Security.Ui/src/Common/ConfirmationModal.jsx) for delete confirmation
- Client + server validation with visual feedback
- Debounced filtering (500ms delay)
- Toast notifications for success/failure (auto-dismiss 5s)

**Response Headers UI Components**:
- [CustomHeadersContainer.jsx](src/Stott.Security.Ui/src/CustomHeaders/CustomHeadersContainer.jsx) - Main container with name/behavior filters
- [CustomHeaderCard.jsx](src/Stott.Security.Ui/src/CustomHeaders/CustomHeaderCard.jsx) - Card display per header
- [CustomHeaderModal.jsx](src/Stott.Security.Ui/src/CustomHeaders/CustomHeaderModal.jsx) - Add/edit form with dynamic value editor
- [HstsHeaderValue.jsx](src/Stott.Security.Ui/src/CustomHeaders/HstsHeaderValue.jsx) - Specialized HSTS editor (max-age slider, includeSubDomains, preload)

---

## Development Environment Setup

### Prerequisites
- .NET SDK 6.0+ (supports up to .NET 10.0)
- Node.js 18+ (for React UI development)
- SQL Server (LocalDB sufficient for development)
- Optimizely CMS 12+ license (for Sample project)

### Build Steps

**Backend**:
```bash
cd src/Stott.Security.Optimizely
dotnet build
dotnet test ../Stott.Security.Optimizely.Test
```

**Frontend**:
```bash
cd src/Stott.Security.Ui
npm install
npm run build-dotnet  # Compiles and copies to Static/ folder
```

**Sample Site**:
```bash
cd Sample
dotnet run
# Navigate to http://localhost:5000
```

---

## Known Limitations

1. **Path Exclusion**: Uses simple substring matching (could have false positives)
2. **CSP Complexity**: Very large source lists (100+ domains) may still exceed terminal threshold
3. **CORS Override**: Replaces default `ICorsPolicyProvider`; potential conflicts with custom CORS implementations

---

## Migration History

**Custom Headers Consolidation** (2026-02-02 to 2026-02-05):
- Added `tbl_CspCustomHeader` table for unified header management
- Deprecated and removed `Features/SecurityHeaders/` (enums, service, repository, controller, tests)
- Renamed UI tab from "Security Headers" to "Response Headers"
- Standard security headers now managed via Custom Headers with predefined allowed values

**CMS 13 Upgrade** (2026-01):
- Replaced `IPageRouteHelper` → `IContentRouteHelper`
- Replaced `ISiteDefinitionRepository` → `IApplicationRepository`
- Updated target frameworks to .NET 6.0, 8.0, 9.0, 10.0
- Replaced plugin attributes with named attributes

**Database Migrations** (13 total):
1. `20220531` - InitialDbSetup (CSP settings, sources, security headers)
2. `20220710` - AddAdditionalHeaders
3. `20221010` - AddWhitelistAddressSettings
4. `20221026` - AddAuditTables
5. `20221031` - AddModifiedFieldsToAuditableTables
6. `20221113` - AddSandboxSupport
7. `20230421` - AddUpgradeInsecureRequests
8. `20230731` - AddCors
9. `20231204` - AllowListRename
10. `20240131` - AddNonceSettings
11. `20240225` - AddExternalReportingUrls
12. `20250311` - AddPermissionPolicy
13. `20260202` - AddCustomHeaders

---

## Quick Context Checklist

Before working on this codebase, verify understanding of:

- [ ] CSP generation flow: Source aggregation → Route filtering → Directive construction → Optimization
- [ ] Audit interception happens automatically in `CspDataContext.SaveChangesAsync()`
- [ ] Header compilation is cached; nonce substituted per request
- [ ] CSP splitting uses greedy bin-packing with directive grouping
- [ ] React UI compiled to embedded resources via `npm run build-dotnet`
- [ ] All entities implement `IAuditableEntity` for automatic audit trail
- [ ] Feature-based organization: Each feature has Repository → Service → Controller
- [ ] Authorization required for all API endpoints
- [ ] Middleware applies headers to every HTTP response (add or remove via `IsRemoval` flag)
- [ ] Custom Headers replaces the old Security Headers feature; manages standard + custom headers
- [ ] Standard headers have predefined allowed values via `CustomHeaderMapper`
- [ ] Tests use NUnit + Moq with in-memory database

---

## Additional Resources

**User Documentation**: [README.md](README.md) - End-user setup and configuration guide

**Issue Tracking**: [GitHub Issues](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/issues)

**CSP Specification**: [MDN CSP Documentation](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP)

**Permissions-Policy**: [W3C Permissions Policy Spec](https://w3c.github.io/webappsec-permissions-policy/)

---

*Last Updated: 2026-02-06*
*Generated for: Optimizely CMS 13 / .NET 10 / React 19*
