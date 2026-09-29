using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Applications;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.PermissionPolicy;

namespace OptimizelyTwelveTest.Features.UiTest;

public class ResetSystemController(
    IApplicationDefinitionService appService,
    IStottSecurityDataContext context,
    ICacheWrapper cache) : Controller
{
    /// <summary>
    /// Resets the security configuration to a known baseline for UI tests.
    /// </summary>
    /// <param name="includeDeprecatedDirectives">
    /// When true, seeds Permission Policy directives which have since been deprecated.  These cannot be
    /// created through the user interface, so they have to be seeded in order to test that pre-existing
    /// configuration is retained, flagged and still applied to the response.
    /// </param>
    [AllowAnonymous]
    [Route("/ui-tests/reset")]
    public async Task<IActionResult> Reset(bool includeDeprecatedDirectives = false)
    {
        // Remove all data from the database to reset the system to a clean state for UI tests.
        var cspSettings = await context.CspSettings.ToListAsync();
        var cspSources = await context.CspSources.ToListAsync();
        var cspSandbox = await context.CspSandboxes.ToListAsync();
        var ppSettings = await context.PermissionPolicySettings.ToListAsync();
        var ppPermissions = await context.PermissionPolicies.ToListAsync();
        var customHeaders = await context.CustomHeaders.ToListAsync();

        context.CspSettings.RemoveRange(cspSettings);
        context.CspSources.RemoveRange(cspSources);
        context.CspSandboxes.RemoveRange(cspSandbox);
        context.PermissionPolicySettings.RemoveRange(ppSettings);
        context.PermissionPolicies.RemoveRange(ppPermissions);
        context.CustomHeaders.RemoveRange(customHeaders);

        await context.SaveChangesAsync();

        var allApps = await appService.GetAllApplicationsAsync();

        const string testUser = "ui-tests";
        var now = DateTime.UtcNow;

        // Setup baseline data
        context.CspSettings.Add(new CspSettings
        {
            IsEnabled = true,
            IsReportOnly = false,
            ModifiedBy = testUser,
            Modified = now
        });

        var selfRequirements = new List<string>
        {
            CspConstants.Directives.DefaultSource,
            CspConstants.Directives.ChildSource,
            CspConstants.Directives.ConnectSource,
            CspConstants.Directives.FontSource,
            CspConstants.Directives.FrameSource,
            CspConstants.Directives.ImageSource,
            CspConstants.Directives.ScriptSource,
            CspConstants.Directives.ScriptSourceElement,
            CspConstants.Directives.StyleSource,
            CspConstants.Directives.StyleSourceElement
        };
        
        context.CspSources.Add(new CspSource
        {
            Source = CspConstants.Sources.Self,
            Directives = string.Join(",", selfRequirements),
            ModifiedBy = testUser,
            Modified = now
        });

        context.CspSources.Add(new CspSource
        {
            Source = CspConstants.Sources.SchemeData,
            Directives = CspConstants.Directives.ImageSource,
            ModifiedBy = testUser,
            Modified = now
        });

        context.PermissionPolicySettings.Add(new PermissionPolicySettings
        {
            IsEnabled = true,
            ModifiedBy = testUser,
            Modified = now
        });

        context.PermissionPolicies.Add(new PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Geolocation,
            EnabledState = PermissionPolicyEnabledState.ThisSite.ToString(),
            ModifiedBy = testUser,
            Modified = now
        });

        if (includeDeprecatedDirectives)
        {
            var deprecatedDirectives = new Dictionary<string, PermissionPolicyEnabledState>
            {
                { PermissionPolicyConstants.AttributionReporting, PermissionPolicyEnabledState.All },
                { PermissionPolicyConstants.BrowsingTopics, PermissionPolicyEnabledState.ThisSite },
                { PermissionPolicyConstants.DocumentDomain, PermissionPolicyEnabledState.None }
            };

            foreach (var deprecatedDirective in deprecatedDirectives)
            {
                context.PermissionPolicies.Add(new PermissionPolicy
                {
                    Directive = deprecatedDirective.Key,
                    EnabledState = deprecatedDirective.Value.ToString(),
                    ModifiedBy = testUser,
                    Modified = now
                });
            }
        }

        // CustomHeaderRepository.CreateOverrideAsync no-ops when the parent context has no
        // header rows to copy. Seed a global baseline so app/host overrides can be created.
        context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = CspConstants.HeaderNames.FrameOptions,
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "DENY",
            AppId = null,
            HostName = null,
            ModifiedBy = testUser,
            Modified = now
        });

        foreach(var app in allApps)
        {
            var appId = app.AppId;

            foreach (var host in app.AvailableHosts)
            {
                var hostName = host.HostName.GetSanitizedHostDomain();

                if (string.Equals(host.HostType, "Primary", StringComparison.OrdinalIgnoreCase))
                {
                    context.CspSources.Add(new CspSource
                    {
                        Source = CspConstants.Sources.Nonce,
                        Directives = string.Join(",", new[]
                        {
                            CspConstants.Directives.ScriptSource,
                            CspConstants.Directives.ScriptSourceElement,
                            CspConstants.Directives.StyleSource,
                            CspConstants.Directives.StyleSourceElement
                        }),
                        ModifiedBy = testUser,
                        Modified = now,
                        AppId = appId,
                        HostName = hostName
                    });
                }

                if (string.Equals(host.HostType, "Edit", StringComparison.OrdinalIgnoreCase))
                {
                    context.CspSources.Add(new CspSource
                    {
                        Source = CspConstants.Sources.UnsafeEval,
                        Directives = string.Join(",", new[]
                        {
                            CspConstants.Directives.ScriptSource,
                            CspConstants.Directives.ScriptSourceElement
                        }),
                        ModifiedBy = testUser,
                        Modified = now,
                        AppId = appId,
                        HostName = hostName
                    });

                    context.CspSources.Add(new CspSource
                    {
                        Source = CspConstants.Sources.UnsafeInline,
                        Directives = string.Join(",", new[]
                        {
                            CspConstants.Directives.ScriptSource,
                            CspConstants.Directives.ScriptSourceElement,
                            CspConstants.Directives.StyleSource,
                            CspConstants.Directives.StyleSourceElement
                        }),
                        ModifiedBy = testUser,
                        Modified = now,
                        AppId = appId,
                        HostName = hostName
                    });

                    context.CspSources.Add(new CspSource
                    {
                        Source = "https://*.optimizely.com",
                        Directives = string.Join(",", new[]
                        {
                            CspConstants.Directives.ConnectSource,
                            CspConstants.Directives.ScriptSource,
                            CspConstants.Directives.ScriptSourceElement,
                            CspConstants.Directives.StyleSource,
                            CspConstants.Directives.StyleSourceElement
                        }),
                        ModifiedBy = testUser,
                        Modified = now,
                        AppId = appId,
                        HostName = hostName
                    });
                }
            }
        }

        await context.SaveChangesAsync();

        cache.RemoveAll();

        return Ok();
    }
}