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

# Run all tests (2,031 tests as of 2026-02-02)
dotnet test

# Run specific test categories
dotnet test --filter "FullyQualifiedName~CspOptimizerTests"
dotnet test --filter "FullyQualifiedName~AuditRepositoryTests"

# Build React UI
cd src/Stott.Security.UI
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
│   ├── Stott.Security.Optimizely/          # Main Razor Class Library (321 .cs files)
│   │   ├── Entities/                        # Domain models + audit entities
│   │   ├── Features/                        # Feature-organized business logic
│   │   │   ├── Csp/                         # CSP generation, optimization, reporting
│   │   │   ├── Cors/                        # CORS policy management
│   │   │   ├── SecurityHeaders/             # Standard security headers
│   │   │   ├── PermissionPolicy/            # Permissions-Policy header
│   │   │   ├── Header/                      # HeaderCompilationService (orchestrator)
│   │   │   ├── Audit/                       # Audit trail
│   │   │   ├── Middleware/                  # SecurityHeaderMiddleware
│   │   │   └── Configuration/               # DI setup
│   │   ├── Migrations/                      # 11 EF Core migrations
│   │   └── Static/                          # Compiled React assets (embedded resources)
│   ├── Stott.Security.Optimizely.Test/     # NUnit test project (83 test files, 2,031 tests)
│   └── Stott.Security.UI/                   # React 19 + Vite frontend (39 .jsx files)
└── Sample/                                  # Demo CMS instance for development
```

### Git Information

- **Current Branch**: `feature/cms13`
- **Main Branch**: `main`
- **Status**: Clean working tree
- **Recent Work**: CMS 13 migration (API updates, multi-targeting)

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
- Standard security headers (HSTS, X-Frame-Options, etc.)
- Automatic audit trail for all changes
- React-based admin UI embedded in CMS

---

## Architecture

### 3-Tier Architecture

1. **Data Layer**: EF Core with SQL Server
   - [CspDataContext.cs](src/Stott.Security.Optimizely/Data/CspDataContext.cs) - DbContext with automatic audit interception
   - Repositories follow feature-based organization

2. **Business Logic**: Feature-organized services
   - Each feature has its own folder: `Features/{FeatureName}/`
   - Pattern: Repository → Service → Controller
   - Example: [CspService.cs](src/Stott.Security.Optimizely/Features/Csp/Service/CspService.cs)

3. **Presentation Layer**: React UI + REST APIs
   - React components in [Stott.Security.UI/](src/Stott.Security.UI/)
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
├─→ [CorsService] (generates CORS headers)
├─→ [SecurityHeaderService] (generates standard headers)
└─→ [PermissionPolicyService] (generates Permission-Policy)
    ↓
Headers added to HTTP Response
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
- [CspSettingsRepository.cs](src/Stott.Security.Optimizely/Features/Csp/Repository/CspSettingsRepository.cs) - Data access
- [CspService.cs](src/Stott.Security.Optimizely/Features/Csp/Service/CspService.cs) - Business logic
- [CspSettingsController.cs](src/Stott.Security.Optimizely/Features/Csp/CspSettingsController.cs) - API

---

## Key Patterns

### 1. Automatic Audit Trail

**ALL entity changes are automatically audited** via DbContext interception.

**How it works**:
- All domain entities implement [IAuditableEntity](src/Stott.Security.Optimizely/Common/IAuditableEntity.cs)
- [CspDataContext.SaveChangesAsync()](src/Stott.Security.Optimizely/Data/CspDataContext.cs) intercepts saves
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

**Implementation**: [CspDataContext.cs](src/Stott.Security.Optimizely/Data/CspDataContext.cs) overrides `SaveChangesAsync()` to:
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

**Entry Point**: [CspService.cs](src/Stott.Security.Optimizely/Features/Csp/Service/CspService.cs)

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

### 4. Header Compilation & Caching

- Headers compiled once per route type and cached
- [HeaderCompilationService](src/Stott.Security.Optimizely/Features/Header/Service/HeaderCompilationService.cs) handles orchestration
- Nonce values generated per request
- `{NONCE}` placeholder substituted in cached headers

**Flow**: Request → [SecurityHeaderMiddleware.cs](src/Stott.Security.Optimizely/Features/Middleware/SecurityHeaderMiddleware.cs) → [HeaderCompilationService.cs](src/Stott.Security.Optimizely/Features/Header/Service/HeaderCompilationService.cs) → Response

**Performance**: Headers compiled once and cached; only nonce substitution per request.

---

## Critical Files

### Most Important Files

| File | Purpose | When to Modify |
|------|---------|----------------|
| [CspService.cs](src/Stott.Security.Optimizely/Features/Csp/Service/CspService.cs) | CSP generation orchestration | Adding CSP features |
| [CspOptimizer.cs](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs) | Header splitting algorithm | CSP optimization changes |
| [HeaderCompilationService.cs](src/Stott.Security.Optimizely/Features/Header/Service/HeaderCompilationService.cs) | Main compilation + caching | Header generation flow |
| [SecurityHeaderMiddleware.cs](src/Stott.Security.Optimizely/Features/Middleware/SecurityHeaderMiddleware.cs) | HTTP middleware hook | Request interception |
| [CspDataContext.cs](src/Stott.Security.Optimizely/Data/CspDataContext.cs) | DbContext with audit interception | Database schema changes |
| [SecurityServiceExtensions.cs](src/Stott.Security.Optimizely/Features/Configuration/SecurityServiceExtensions.cs) | DI configuration | Startup configuration |

### Data Layer

| File | Purpose |
|------|---------|
| [CspDataContext.cs](src/Stott.Security.Optimizely/Data/CspDataContext.cs) | EF Core DbContext with audit interception |
| [CspSettingsRepository.cs](src/Stott.Security.Optimizely/Features/Csp/Repository/CspSettingsRepository.cs) | Settings data access |
| [CspPermissionRepository.cs](src/Stott.Security.Optimizely/Features/Csp/Repository/CspPermissionRepository.cs) | Source/permission data access |
| [AuditRepository.cs](src/Stott.Security.Optimizely/Features/Audit/AuditRepository.cs) | Audit data access |

### API Controllers

| File | Purpose |
|------|---------|
| [CspPermissionsController.cs](src/Stott.Security.Optimizely/Features/Csp/CspPermissionsController.cs) | CSP source management API |
| [CspSettingsController.cs](src/Stott.Security.Optimizely/Features/Csp/CspSettingsController.cs) | Global CSP settings API |
| [SecurityHeaderController.cs](src/Stott.Security.Optimizely/Features/SecurityHeaders/SecurityHeaderController.cs) | Standard headers API |
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
| [App.jsx](src/Stott.Security.UI/src/App.jsx) | Main component with tab navigation |
| [StottSecurityContext.jsx](src/Stott.Security.UI/src/Context/StottSecurityContext.jsx) | State management + HTTP service |
| [CspSettings.jsx](src/Stott.Security.UI/src/CSP/CspSettings.jsx) | Global CSP configuration UI |
| [CspSources.jsx](src/Stott.Security.UI/src/CSP/CspSources.jsx) | Domain source management UI |

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

### Adding a New Security Header Type

1. **Create entity** in [Entities/](src/Stott.Security.Optimizely/Entities/) implementing `IAuditableEntity`
2. **Add DbSet** to [CspDataContext.cs](src/Stott.Security.Optimizely/Data/CspDataContext.cs)
3. **Create migration**: `dotnet ef migrations add AddNewHeaderType`
4. **Implement repository** in `Features/{HeaderType}/Repository/`
5. **Implement service** in `Features/{HeaderType}/Service/`
6. **Create API controller** in `Features/{HeaderType}/`
7. **Add to HeaderCompilationService** compilation logic
8. **Create React component** in [Stott.Security.UI/src/{HeaderType}/](src/Stott.Security.UI/src/)
9. **Add tab** to [App.jsx](src/Stott.Security.UI/src/App.jsx)
10. **Write tests** in [Stott.Security.Optimizely.Test/](src/Stott.Security.Optimizely.Test/)

### Modifying React UI

1. Navigate to React project: `cd src/Stott.Security.UI`
2. Make changes to `.jsx` files
3. Build: `npm run build-dotnet` (compiles and copies to Static/)
4. Rebuild main project: `cd ../Stott.Security.Optimizely && dotnet build`
5. Test in sample site: `cd ../../Sample && dotnet run`

---

## Testing

### Framework & Organization

- **Framework**: NUnit + Moq
- **83 test files** with **2,031 test cases** (as of 2026-02-02)
- In-memory database via `TestDataContextFactory`
- Parameterized tests in separate `*TestCases.cs` files

### Coverage Areas

- CSP generation and optimization (43 tests in CspOptimizerTests)
- CORS policy provider
- Security header compilation
- Permission policy mapping
- Audit record creation and cleanup (18 tests)
- Validation rules
- Controller authorization

### Notable Test Additions

**CSP Optimizer Edge Cases** (3 tests added 2026-02-02):
- Very long domain names (60+ character URLs)
- Mixed domain length distributions
- Special CSP keywords only (no domains)

**Audit Cleanup Tests** (18 tests added 2026-02-02):
- Repository DeleteAsync method (5 tests)
- Scheduled job execution (10 tests)
- Data integrity verification (orphan prevention)

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

**Context**: [CspDataContext.cs](src/Stott.Security.Optimizely/Data/CspDataContext.cs)

**Key Tables**:

| Table | Purpose | Key Fields |
|-------|---------|-----------|
| `tbl_CspSettings` | Global CSP configuration | IsEnabled, IsReportOnly, IsAllowListEnabled, UseStrictDynamic |
| `tbl_CspSource` | CSP sources (domains) | Source (domain), Directives (CSV of allowed directives) |
| `tbl_CspSandbox` | CSP sandbox settings | 15 boolean flags (AllowScripts, AllowSameOrigin, etc.) |
| `tbl_CspSecurityHeaderSettings` | Standard headers | XContentTypeOptions, ReferrerPolicy, XFrameOptions, HSTS config |
| `tbl_CspCorsSettings` | CORS configuration | AllowedOrigins, AllowedMethods, AllowedHeaders |
| `tbl_CspPermissionPolicySettings` | Permissions-Policy | Per-directive configurations |
| `tbl_CspAuditHeader` | Audit headers | RecordType, OperationType, Identifier, ActionedBy, Actioned |
| `tbl_CspAuditProperty` | Audit details | AuditHeaderId, Field, OldValue, NewValue |

**Migrations**: 11 migrations track schema evolution from initial setup through CMS 13 upgrade.

---

## API Endpoints

**Base Path**: `/stott.security.optimizely/api/`

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/cspsettings/` | GET/POST | Global CSP settings |
| `/csppermissions/` | GET/POST/DELETE | CSP source management |
| `/csppermissions/append` | POST | Bulk add sources |
| `/cspreporting/` | GET | CSP violation reports |
| `/sandbox/` | GET/POST | Sandbox directives |
| `/cors/` | GET/POST | CORS configuration |
| `/securityheaders/` | GET/POST | Standard security headers |
| `/permissionpolicy/` | GET/POST | Permissions-Policy settings |
| `/audit/` | GET | Audit history (paginated) |
| `/compiled-headers/` | GET | Preview compiled headers (headless API) |

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
3. Check for React build errors: `cd src/Stott.Security.UI && npm run build-dotnet`
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
- **App.jsx**: Tab-based navigation with hash routing (`#csp-settings`, `#cors`, etc.)
- **StottSecurityContext.jsx**: Centralized state management, HTTP service, toast notifications
- **Feature Components**: CSP (7 components), CORS, Security Headers, Permissions Policy, Audit, Tools

**API Communication**: All via Axios to `/stott.security.optimizely/api/*` endpoints

**UI Patterns**:
- Modal dialogs for add/edit operations
- Client + server validation with visual feedback
- Debounced filtering (500ms delay)
- Async operations with loading states
- Toast notifications for success/failure

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
cd src/Stott.Security.UI
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

**CMS 13 Upgrade** (Recent):
- Replaced `IPageRouteHelper` → `IContentRouteHelper`
- Replaced `ISiteDefinitionRepository` → `IApplicationRepository`
- Updated target frameworks to .NET 6.0, 8.0, 9.0, 10.0
- Replaced plugin attributes with named attributes

**Version History** (via migrations):
1. Initial schema creation
2. CSP settings and sources
3. Security headers
4. CORS support
5. Permissions Policy
6. Audit trail
7. Sandbox directives
8. CSP reporting
9. Security.txt support (v4.0.0)
10. Route configuration
11. CMS 13 compatibility updates

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
- [ ] Middleware applies headers to every HTTP response
- [ ] Tests use NUnit + Moq with in-memory database

---

## Additional Resources

**User Documentation**: [README.md](README.md) - End-user setup and configuration guide

**Issue Tracking**: [GitHub Issues](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/issues)

**CSP Specification**: [MDN CSP Documentation](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP)

**Permissions-Policy**: [W3C Permissions Policy Spec](https://w3c.github.io/webappsec-permissions-policy/)

---

*Last Updated: 2026-02-02*
*Generated for: Optimizely CMS 13 / .NET 10 / React 19*
