using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Dtos;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Pages;

namespace Stott.Security.Optimizely.Features.Csp;

public sealed class CspService : ICspService
{
    private readonly ICspSettingsService _cspSettingsService;

    private readonly ICspPermissionService _cspPermissionService;

    private readonly ICspSandboxService _cspSandboxService;

    public CspService(
        ICspSettingsService cspSettingsService,
        ICspPermissionService cspPermissionService,
        ICspSandboxService cspSandboxService)
    {
        _cspSettingsService = cspSettingsService;
        _cspPermissionService = cspPermissionService;
        _cspSandboxService = cspSandboxService;
    }

    public async Task<IEnumerable<HeaderDto>> GetCompiledHeaders(IContentSecurityPolicyPage? currentPage)
    {
        var settings = await _cspSettingsService.GetAsync();
        if (settings is not { IsEnabled: true })
        {
            return Enumerable.Empty<HeaderDto>();
        }

        var cspSandbox = await _cspSandboxService.GetAsync();
        var cspSources = await _cspPermissionService.GetAsync();
        var pageSources = currentPage?.ContentSecurityPolicySources;

        var cspHeaders = GetCspHeaders(settings, cspSandbox, cspSources, pageSources).ToList();
        if (cspHeaders.Count > 0)
        {
            var reportingEndPoints = GetReportingEndPoints(settings).ToList();
            if (reportingEndPoints is { Count: > 0 })
            {
                cspHeaders.Add(new HeaderDto { Key = CspConstants.HeaderNames.ReportingEndpoints, Value = string.Join(", ", reportingEndPoints) });
            }
        }

        return cspHeaders;
    }

    private IEnumerable<HeaderDto> GetCspHeaders(
        CspSettings settings,
        SandboxModel? sandbox,
        IList<CspSource>? globalSources,
        IList<PageCspSourceMapping>? pageSources)
    {
        if (globalSources is not { Count: > 0 } && 
            pageSources is not { Count: > 0 } && 
            settings is not { IsUpgradeInsecureRequestsEnabled: true } &&
            sandbox is not { IsSandboxEnabled: true })
        {
            yield break;
        }

        var headerName = settings.IsReportOnly ? CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy : CspConstants.HeaderNames.ContentSecurityPolicy;
        var cspContents = BuildCspContent(settings, sandbox, globalSources, pageSources);

        foreach (var cspContent in cspContents)
        {
            yield return new HeaderDto { Key = headerName, Value = cspContent };
        }
    }

    private List<string> BuildCspContent(
        CspSettings settings, 
        SandboxModel? sandbox,
        IList<CspSource>? globalSources, 
        IList<PageCspSourceMapping>? pageSources)
    {
        var cspContents = new List<string>();
        var stringBuilder = new StringBuilder();
        var directives = GetAllDirectives(settings, sandbox, globalSources, pageSources);
        var directiveGroups = CspOptimizer.GroupDirectives(settings, directives);
        foreach (var directiveGroup in directiveGroups)
        {
            stringBuilder.Clear();
            foreach (var directive in directiveGroup)
            {
                stringBuilder.Append(directive.ToString());
            }
            cspContents.Add(stringBuilder.ToString().Trim());
        }

        return cspContents;
    }

    private List<CspDirectiveDto> GetAllDirectives(
        CspSettings settings, 
        SandboxModel? sandbox,
        IList<CspSource>? globalSources, 
        IList<PageCspSourceMapping>? pageSources)
    {
        var directives = GetFetchDirectives(settings, globalSources, pageSources).ToList();

        if (settings is { IsUpgradeInsecureRequestsEnabled: true })
        {
            directives.Add(new CspDirectiveDto(CspConstants.Directives.UpgradeInsecureRequests, new List<string>(0)));
        }

        var sandboxDirective = GetSandboxDirective(settings, sandbox);
        if (sandboxDirective is not null)
        {
            directives.Add(sandboxDirective);
        }

        var reportToEndPoints = GetReportToEndPoints(settings).ToList();
        if (reportToEndPoints is { Count: > 0 })
        {
            directives.Add(new CspDirectiveDto(CspConstants.Directives.ReportTo, reportToEndPoints));
        }

        return directives;
    } 

    private static IEnumerable<CspDirectiveDto> GetFetchDirectives(CspSettings settings, IList<CspSource>? globalSources, IList<PageCspSourceMapping>? pageSources)
    {
        var cspSources = new List<CspSourceDto>();
        cspSources.AddRange(ConvertToDtos(globalSources));
        cspSources.AddRange(ConvertToDtos(pageSources));

        if (cspSources is not { Count: >0 })
        {
            yield break;
        }

        var stringBuilder = new StringBuilder();
        var distinctDirectives = cspSources.SelectMany(x => x.Directives).Distinct().ToList();
        var noneDirectives = cspSources.Where(x => x.Source.Equals(CspConstants.Sources.None))
                                            .SelectMany(x => x.Directives)
                                            .Distinct()
                                            .ToList();

        foreach (var directive in distinctDirectives)
        {
            if (noneDirectives.Contains(directive))
            {
                yield return new CspDirectiveDto(directive, CspConstants.Sources.None);
                continue;
            }

            var directiveSources = cspSources.Where(x => x.Directives.Contains(directive))
                                             .Select(x => x.Source.ToLower())
                                             .OrderBy(x => GetSortIndex(x))
                                             .ThenBy(x => x)
                                             .Distinct()
                                             .ToList();

            yield return new CspDirectiveDto(directive, directiveSources);
        }
    }

    private static CspDirectiveDto? GetSandboxDirective(CspSettings settings, SandboxModel? sandbox)
    {
        if (sandbox is not { IsSandboxEnabled: true } || !settings.IsEnabled || settings.IsReportOnly)
        {
            return null;
        }

        var sandboxSettings = GetSandboxSettings(settings, sandbox).ToList();

        return new CspDirectiveDto(CspConstants.Directives.Sandbox, sandboxSettings);
    }

    private static IEnumerable<string> GetSandboxSettings(CspSettings settings, SandboxModel? sandbox)
    {
        if (sandbox is not { IsSandboxEnabled: true } || !settings.IsEnabled || settings.IsReportOnly)
        {
            yield break;
        }

        if (sandbox.IsAllowDownloadsEnabled) { yield return "allow-downloads"; }
        if (sandbox.IsAllowDownloadsWithoutGestureEnabled) { yield return "allow-downloads-without-user-activation"; }
        if (sandbox.IsAllowFormsEnabled) { yield return "allow-forms"; }
        if (sandbox.IsAllowModalsEnabled) { yield return "allow-modals"; }
        if (sandbox.IsAllowOrientationLockEnabled) { yield return "allow-orientation-lock"; }
        if (sandbox.IsAllowPointerLockEnabled) { yield return "allow-pointer-lock"; }
        if (sandbox.IsAllowPopupsEnabled) { yield return "allow-popups"; }
        if (sandbox.IsAllowPopupsToEscapeTheSandboxEnabled) { yield return "allow-popups-to-escape-sandbox"; }
        if (sandbox.IsAllowPresentationEnabled) { yield return "allow-presentation"; }
        if (sandbox.IsAllowSameOriginEnabled) { yield return "allow-same-origin"; }
        if (sandbox.IsAllowScriptsEnabled) { yield return "allow-scripts"; }
        if (sandbox.IsAllowStorageAccessByUserEnabled) { yield return "allow-storage-access-by-user-activation"; }
        if (sandbox.IsAllowTopNavigationEnabled) { yield return "allow-top-navigation"; }
        if (sandbox.IsAllowTopNavigationByUserEnabled) { yield return "allow-top-navigation-by-user-activation"; }
        if (sandbox.IsAllowTopNavigationToCustomProtocolEnabled) { yield return "allow-top-navigation-to-custom-protocols"; }
    }

    private static IEnumerable<string> GetReportToEndPoints(CspSettings settings)
    {
        if (settings is { UseInternalReporting: true })
        {
            yield return "stott-security-endpoint";
        }

        if (settings is { UseExternalReporting: true, ExternalReportToUrl.Length: > 0 })
        {
            yield return "stott-security-external-endpoint";
        }
    }

    private static IEnumerable<string> GetReportingEndPoints(CspSettings settings)
    {
        if (settings is { IsEnabled: true, UseInternalReporting: true })
        {
            yield return $"stott-security-endpoint=\"{CspConstants.InternalReportingPlaceholder}\"";
        }

        if (settings is { IsEnabled: true, UseExternalReporting: true, ExternalReportToUrl.Length: > 0 })
        {
            yield return $"stott-security-external-endpoint=\"{settings.ExternalReportToUrl}\"";
        }
    }

    private static IEnumerable<CspSourceDto> ConvertToDtos(IEnumerable<ICspSourceMapping>? sources)
    {
        if (sources is null)
        {
            yield break;
        }

        foreach (var source in sources)
        {
            if (!string.IsNullOrWhiteSpace(source.Source) && !string.IsNullOrWhiteSpace(source.Directives))
            {
                yield return new CspSourceDto(source.Source, source.Directives);
            }
        }
    }

    private static int GetSortIndex(string source)
    {
        var index = CspConstants.AllSources.IndexOf(source);

        return index < 0 ? 100 : index;
    }
}
