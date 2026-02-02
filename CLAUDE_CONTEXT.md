# Stott.Security.Optimizely - Claude Context Guide

## Project Overview

**Purpose**: Optimizely CMS addon for managing security response headers (Content Security Policy, Permissions Policy, CORS, etc.) with a non-technical user interface.

**Key Innovation**: Domain-based CSP management (users grant permissions to domains) with intelligent header optimization and splitting to prevent CDN issues at 8KB/16KB thresholds.

**Target Framework**: .NET 6.0, 8.0, 9.0, 10.0 | Optimizely CMS 12+

**Repository**: [GeekInTheNorth/Stott.Security.Optimizely](https://github.com/GeekInTheNorth/Stott.Security.Optimizely)

---

## Solution Structure

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
│   ├── Stott.Security.Optimizely.Test/     # NUnit test project (81 test files)
│   └── Stott.Security.UI/                   # React 19 + Vite frontend (39 .jsx files)
│       └── src/
│           ├── CSP/                         # CSP management UI (7 components)
│           ├── Security/                    # Security headers UI
│           ├── Cors/                        # CORS configuration UI
│           ├── PermissionsPolicy/           # Permissions-Policy editor
│           ├── Audit/                       # Audit history viewer
│           └── Context/                     # React Context state management
└── Sample/                                  # Demo CMS instance for development
```

---

## Architecture Patterns

### 1. Feature-Based Organization

Each security feature follows consistent layering:

```
Features/{FeatureName}/
├── Entities/          # Domain models (implements IAuditableEntity)
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

### 2. Audit-First Design

**Every entity modification is automatically audited** via DbContext interception.

**Pattern**: All domain entities implement `IAuditableEntity`:
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

**Database Tables**:
- `tbl_CspAuditHeader` - Header record per change operation
- `tbl_CspAuditProperty` - Detailed field changes (N:1 with header)

### 3. Middleware-Based Header Injection

**Flow**: Request → [SecurityHeaderMiddleware.cs](src/Stott.Security.Optimizely/Features/Middleware/SecurityHeaderMiddleware.cs) → [HeaderCompilationService.cs](src/Stott.Security.Optimizely/Features/Header/Service/HeaderCompilationService.cs) → Response

**Key Responsibilities**:
- **Middleware**: Intercepts every HTTP response
- **HeaderCompilationService**:
  - Caches compiled headers (cache key includes route type)
  - Generates nonce values per request
  - Substitutes `{NONCE}` placeholder in cached headers
  - Delegates to feature-specific services

**Performance**: Headers compiled once and cached; only nonce substitution per request.

---

## Critical Business Logic

### CSP Generation Pipeline

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
   - Exclusion paths configured in `SecurityRouteConfiguration`

3. **Directive Construction**:
   - Groups sources by directive
   - Inherits from `default-src` when specific directive not defined
   - Deduplicates and sorts sources for consistency
   - Handles special keywords: `'self'`, `'unsafe-inline'`, `'none'`

4. **Optimization & Splitting** → See below

### CSP Header Splitting Algorithm

**Problem**: CDNs break with headers > 8KB; critical failure at 16KB.

**Solution**: [CspOptimizer.cs](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs)

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
   - If group exceeds threshold → simplify (collapse to primary directive, e.g., all script directives → `script-src`)
   - Always preserve `default-src` in each header
   - Always include `report-to` directive for violation tracking

4. **Output**: Array of CSP header strings (typically 1, rarely 2-3)

**Key Method**: [CspOptimizer.GroupDirectives()](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs#L64-L93)

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

## Configuration & Extensibility

### Startup Configuration

**File**: [SecurityServiceExtensions.cs](src/Stott.Security.Optimizely/Features/Configuration/SecurityServiceExtensions.cs)

```csharp
// In Startup.cs or Program.cs
services.AddStottSecurity(
    cspSetupOptions => {
        cspSetupOptions.ConnectionStringName = "EPiServerDB";
        cspSetupOptions.NonceHashExclusionPaths.Add("/custom-admin-path");
    },
    authorizationOptions => {
        authorizationOptions.AddPolicy(CspConstants.AuthorizationPolicy,
            policy => policy.RequireRole("WebAdmins", "CustomRole"));
    }
);

app.UseStottSecurity(); // Adds middleware
```

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

---

## Testing

**Framework**: NUnit + Moq

**Test Organization**:
- **81 test files** in [Stott.Security.Optimizely.Test/](src/Stott.Security.Optimizely.Test/)
- **2,021 total test cases** (as of 2026-02-02)
- **TestCases pattern**: Separate classes for parameterized test data
  - Example: [CspServiceTestCases.cs](src/Stott.Security.Optimizely.Test/Csp/CspServiceTestCases.cs)
- **In-memory database**: `TestDataContextFactory` for integration tests

**Coverage Areas**:
- CSP generation and optimization
- CORS policy provider
- Security header compilation
- Permission policy mapping
- Audit record creation
- Validation rules
- Controller authorization

**CSP Optimizer Edge Case Tests** (3 unique tests added 2026-02-02):
- Very long domain names (60+ character URLs) - Tests realistic long CDN/analytics URLs
- Mixed domain length distributions - Tests size calculation with varied domain lengths
- Special CSP keywords only (no domains) - Tests keyword-only security policies

**Running Tests**:
```bash
dotnet test  # Run all 2,021 tests
dotnet test --filter "FullyQualifiedName~CspOptimizerTests"  # Run optimizer tests only
```

---

## Key Files Reference

### Core Business Logic
| File | Purpose |
|------|---------|
| [CspService.cs](src/Stott.Security.Optimizely/Features/Csp/Service/CspService.cs) | CSP generation orchestration |
| [CspOptimizer.cs](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs) | Header splitting algorithm |
| [HeaderCompilationService.cs](src/Stott.Security.Optimizely/Features/Header/Service/HeaderCompilationService.cs) | Main compilation + caching |
| [SecurityHeaderMiddleware.cs](src/Stott.Security.Optimizely/Features/Middleware/SecurityHeaderMiddleware.cs) | HTTP middleware hook |

### Data Layer
| File | Purpose |
|------|---------|
| [CspDataContext.cs](src/Stott.Security.Optimizely/Data/CspDataContext.cs) | EF Core DbContext with audit interception |
| [CspSettingsRepository.cs](src/Stott.Security.Optimizely/Features/Csp/Repository/CspSettingsRepository.cs) | Settings data access |
| [CspPermissionRepository.cs](src/Stott.Security.Optimizely/Features/Csp/Repository/CspPermissionRepository.cs) | Source/permission data access |

### API Controllers
| File | Purpose |
|------|---------|
| [CspPermissionsController.cs](src/Stott.Security.Optimizely/Features/Csp/CspPermissionsController.cs) | CSP source management API |
| [CspSettingsController.cs](src/Stott.Security.Optimizely/Features/Csp/CspSettingsController.cs) | Global CSP settings API |
| [SecurityHeaderController.cs](src/Stott.Security.Optimizely/Features/SecurityHeaders/SecurityHeaderController.cs) | Standard headers API |
| [AuditController.cs](src/Stott.Security.Optimizely/Features/Audit/AuditController.cs) | Audit history API |

### React UI
| File | Purpose |
|------|---------|
| [App.jsx](src/Stott.Security.UI/src/App.jsx) | Main component with tab navigation |
| [StottSecurityContext.jsx](src/Stott.Security.UI/src/Context/StottSecurityContext.jsx) | State management + HTTP service |
| [CspSettings.jsx](src/Stott.Security.UI/src/CSP/CspSettings.jsx) | Global CSP configuration UI |
| [CspSources.jsx](src/Stott.Security.UI/src/CSP/CspSources.jsx) | Domain source management UI |

---

## Common Workflows

### Adding a New Security Header Type

1. **Create entity** in [Entities/](src/Stott.Security.Optimizely/Entities/) implementing `IAuditableEntity`
2. **Add DbSet** to [CspDataContext.cs](src/Stott.Security.Optimizely/Data/CspDataContext.cs)
3. **Create migration**: `dotnet ef migrations add AddNewHeaderType`
4. **Implement repository** in `Features/{HeaderType}/Repository/`
5. **Implement service** in `Features/{HeaderType}/Service/`
6. **Create API controller** in `Features/{HeaderType}/`
7. **Add to HeaderCompilationService** compilation logic
8. **Create React component** in [src/Stott.Security.UI/src/{HeaderType}/](src/Stott.Security.UI/src/)
9. **Add tab** to [App.jsx](src/Stott.Security.UI/src/App.jsx)
10. **Write tests** in [Stott.Security.Optimizely.Test/](src/Stott.Security.Optimizely.Test/)

### Modifying CSP Optimization Logic

**When**: Adding new CSP directive types or changing splitting behavior

**Files to Update**:
1. [CspOptimizer.cs](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs) - Update `GroupDirectives()` method
2. [CspConstants.cs](src/Stott.Security.Optimizely/Common/CspConstants.cs) - Add new directive constants
3. [CspOptimizerTests.cs](src/Stott.Security.Optimizely.Test/Csp/CspOptimizerTests.cs) - Add test cases
4. Update directive grouping logic if new directive needs special handling

### Debugging CSP Issues

**Enable detailed logging**:
- CSP violations logged to configured `report-to` endpoint
- [SecurityHeaderMiddleware.cs](src/Stott.Security.Optimizely/Features/Middleware/SecurityHeaderMiddleware.cs) logs errors to Optimizely logger
- Check [CspOptimizer.cs](src/Stott.Security.Optimizely/Features/Csp/CspOptimizer.cs) for size threshold warnings

**Common Issues**:
- **CSP not applied**: Check `IsEnabled` in [CspSettings](src/Stott.Security.Optimizely/Entities/CspSettings.cs)
- **Too restrictive**: Check global sources + page-specific sources
- **Nonce not working**: Verify route not in exclusion paths
- **Header too large**: Review sources count, may need to simplify domains

---

## Important Constants

**File**: [CspConstants.cs](src/Stott.Security.Optimizely/Common/CspConstants.cs)

**Key Values**:
- `AuthorizationPolicy = "StottSecurityPolicy"` - Required role for API access
- CSP directive constants: `ScriptSrc`, `StyleSrc`, `DefaultSrc`, etc.
- CSP source keywords: `Self`, `None`, `UnsafeInline`, `UnsafeEval`, `StrictDynamic`
- Default nonce/hash exclusion paths: `/episerver`, `/ui`, `/util`

---

## Technical Decisions

### Why Domain-Based CSP (Not Raw Directives)?

**Rationale**: Non-technical users understand "allow scripts from google.com" better than "add `https://google.com` to `script-src`".

**Trade-off**: Less granular control, but significantly more accessible to content editors and marketers.

### Why Split Headers Instead of Simplifying?

**Rationale**: Maintains maximum security granularity while working within CDN constraints.

**Alternative Considered**: Always use simplified directives (fewer sources, broader permissions) - rejected as too permissive.

### Why Automatic Audit Trail?

**Rationale**: Security header changes impact site security posture; compliance requires knowing who changed what and when.

**Implementation**: DbContext interception ensures audit cannot be bypassed.

### Why Embedded React Assets?

**Rationale**: Simplifies deployment (single NuGet package), eliminates separate frontend hosting, reduces configuration complexity.

**Trade-off**: Increases DLL size, but acceptable for admin UI.

### Why ServiceLocator in HeaderCompilationService?

**Rationale**: Legacy Optimizely pattern for lazy-loaded dependencies; avoids circular dependency issues with middleware registration.

**Note**: Not ideal for testability, but pragmatic given Optimizely's DI container constraints.

---

## Known Limitations

1. **Audit Retention**: No automatic cleanup policy; requires manual DBA intervention for large deployments
2. **Path Exclusion**: Uses simple substring matching (could have false positives)
3. **CSP Complexity**: Very large source lists (100+ domains) may still exceed terminal threshold
4. **CORS Override**: Replaces default `ICorsPolicyProvider`; potential conflicts with custom CORS implementations

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

## Git Branch Strategy

**Current Branch**: `feature/cms13`
**Main Branch**: `main`
**Status**: Clean working tree

**Recent Work**: CMS 13 migration (attribute updates, API replacements, multi-target framework configuration)

---

## Quick Context Checklist

When starting work on this solution, verify understanding of:

- [ ] CSP generation flow: Source aggregation → Route filtering → Directive construction → Optimization
- [ ] Audit interception happens in `CspDataContext.SaveChangesAsync()`
- [ ] Header compilation is cached; nonce substituted per request
- [ ] CSP splitting uses greedy bin-packing with directive grouping
- [ ] React UI compiled to embedded resources via `npm run build-dotnet`
- [ ] All entities implement `IAuditableEntity` for automatic audit trail
- [ ] Feature-based organization: Each feature has Repository → Service → Controller
- [ ] Authorization required for all API endpoints (default: CmsAdmins role)
- [ ] Middleware applies headers to every HTTP response
- [ ] Tests use NUnit + Moq with in-memory database

---

## Getting Help

**Documentation**: Check [README.md](README.md) in repository root

**Issue Tracking**: GitHub Issues at repository URL

**Architecture Questions**: Refer to this context guide

**CSP Specification**: [MDN CSP Documentation](https://developer.mozilla.org/en-US/docs/Web/HTTP/CSP)

**Permissions-Policy**: [W3C Permissions Policy Spec](https://w3c.github.io/webappsec-permissions-policy/)

---

*Last Updated: 2026-02-02*
*Generated for: Optimizely CMS 13 / .NET 10 / React 19*
