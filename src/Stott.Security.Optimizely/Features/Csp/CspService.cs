using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;
using Stott.Security.Optimizely.Features.Pages;

namespace Stott.Security.Optimizely.Features.Csp;

public sealed class CspService : ICspService
{
    private readonly ICspSettingsService _cspSettingsService;

    private readonly ICspPermissionService _cspPermissionService;

    private readonly ICspSandboxService _cspSandboxService;

    private readonly ICspReportUrlResolver _cspReportUrlResolver;

    public CspService(
        ICspSettingsService cspSettingsService,
        ICspPermissionService cspPermissionService,
        ICspSandboxService cspSandboxService,
        ICspReportUrlResolver cspReportUrlResolver)
    {
        _cspSettingsService = cspSettingsService;
        _cspPermissionService = cspPermissionService;
        _cspSandboxService = cspSandboxService;
        _cspReportUrlResolver = cspReportUrlResolver;
    }

    public async Task<IEnumerable<KeyValuePair<string, string>>> GetCompiledHeaders(IContentSecurityPolicyPage? currentPage)
    {
        var settings = await _cspSettingsService.GetAsync();
        if (settings is not { IsEnabled: true })
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }

        var cspSandbox = await _cspSandboxService.GetAsync() ?? new SandboxModel();
        var cspSources = await _cspPermissionService.GetAsync() ?? new List<CspSource>(0);
        var pageSources = currentPage?.ContentSecurityPolicySources ?? new List<PageCspSourceMapping>(0);

        return GetHeaders(settings, cspSandbox, cspSources, pageSources).ToList();
    }

    private IEnumerable<KeyValuePair<string, string>> GetHeaders(
        CspSettings settings,
        SandboxModel sandbox,
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

        var cspContent = BuildCspContent(settings, sandbox, globalSources, pageSources, false);
        if (string.IsNullOrWhiteSpace(cspContent))
        {
            yield break;
        }   

        if (cspContent.Length > CspConstants.MaxHeaderSize)
        {
            cspContent = BuildCspContent(settings, sandbox, globalSources, pageSources, true);
        }

        if (cspContent.Length > CspConstants.MaxHeaderSize)
        {
            yield break;
        }

        if (settings.IsReportOnly)
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy, cspContent);
        }
        else
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.ContentSecurityPolicy, cspContent);
        }

        var reportingEndPoints = GetReportingEndPoints(settings).ToList();
        if (reportingEndPoints is { Count: >0 })
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.ReportingEndpoints, string.Join(", ", reportingEndPoints));
        }
    }

    private string BuildCspContent(
        CspSettings settings, 
        SandboxModel sandbox,
        IList<CspSource>? globalSources, 
        IList<PageCspSourceMapping>? pageSources, 
        bool simplifyCsp)
    {
        var stringBuilder = new StringBuilder();

        var directives = GetDirectives(settings, globalSources, pageSources, simplifyCsp).ToList();
        foreach (var directive in directives)
        {
            stringBuilder.Append(directive);
        }

        if (settings is { IsUpgradeInsecureRequestsEnabled: true })
        {
            stringBuilder.Append($"{CspConstants.Directives.UpgradeInsecureRequests};");
        }

        var sandboxSettings = GetSandboxSettings(settings, sandbox).ToList();
        if (sandboxSettings is { Count: >0 })
        {
            stringBuilder.Append($"{string.Join(' ', sandboxSettings)}; ");
        }

        var reportToEndPoints = GetReportToEndPoints(settings).ToList();
        if (reportToEndPoints is { Count: > 0 })
        {
            stringBuilder.Append($"report-to {string.Join(' ', reportToEndPoints)};");
        }

        var reportUriAddresses = GetReportUriAddresses(settings).ToList();
        if (reportUriAddresses is { Count: > 0 })
        {
            stringBuilder.Append($"report-uri {string.Join(' ', reportUriAddresses)};");
        }

        return stringBuilder.ToString().Trim();
    }

    private IEnumerable<string> GetDirectives(CspSettings settings, IList<CspSource>? globalSources, IList<PageCspSourceMapping>? pageSources, bool simplifyCsp)
    {
        var cspSources = new List<CspSourceDto>();
        cspSources.AddRange(ConvertToDtos(globalSources, simplifyCsp));
        cspSources.AddRange(ConvertToDtos(pageSources, simplifyCsp));

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
                yield return $"{directive} {CspConstants.Sources.None}; ";
                continue;
            }

            var directiveSources = cspSources.Where(x => x.Directives.Contains(directive))
                                             .Select(x => x.Source.ToLower())
                                             .OrderBy(x => GetSortIndex(x))
                                             .ThenBy(x => x)
                                             .Distinct()
                                             .ToList();

            if (settings is { IsNonceEnabled: true } && CspConstants.NonceDirectives.Contains(directive))
            {
                directiveSources.Add(CspConstants.NoncePlaceholder);
            }

            yield return $"{directive} {string.Join(" ", directiveSources)}; ";
        }
    }

    private static IEnumerable<string> GetSandboxSettings(CspSettings settings, SandboxModel sandbox)
    {
        if (!sandbox.IsSandboxEnabled || !settings.IsEnabled || settings.IsReportOnly)
        {
            yield break;
        }

        if (sandbox.IsSandboxEnabled) { yield return CspConstants.Directives.Sandbox; }
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

        if (settings is { UseExternalReporting: true, ExternalReportUriUrl.Length: > 0 })
        {
            yield return "stott-security-external-endpoint";
        }
    }

    private IEnumerable<string> GetReportUriAddresses(CspSettings settings)
    {
        if (settings is { UseInternalReporting: true })
        {
            yield return _cspReportUrlResolver.GetReportUriPath();
        }

        if (settings is { UseExternalReporting: true, ExternalReportUriUrl.Length: > 0 })
        {
            yield return settings.ExternalReportUriUrl;
        }
    }

    private IEnumerable<string> GetReportingEndPoints(CspSettings settings)
    {
        if (settings is { IsEnabled: true, UseInternalReporting: true })
        {
            yield return $"stott-security-endpoint=\"{_cspReportUrlResolver.GetReportToPath()}\"";
        }

        if (settings is { IsEnabled: true, UseExternalReporting: true, ExternalReportToUrl.Length: > 0 })
        {
            yield return $"stott-security-external-endpoint=\"{settings.ExternalReportToUrl}\"";
        }
    }

    private static IEnumerable<CspSourceDto> ConvertToDtos(IEnumerable<ICspSourceMapping>? sources, bool simplifyCsp)
    {
        if (sources == null)
        {
            yield break;
        }

        foreach (var source in sources)
        {
            var directives = source.Directives ?? string.Empty;
            if (simplifyCsp)
            {
                directives = directives.Replace(CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSource)
                                       .Replace(CspConstants.Directives.ScriptSourceAttribute, CspConstants.Directives.ScriptSource)
                                       .Replace(CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSource)
                                       .Replace(CspConstants.Directives.StyleSourceAttribute, CspConstants.Directives.StyleSource)
                                       .Replace(CspConstants.Directives.ChildSource, CspConstants.Directives.FrameSource);
            }

            var dto = new CspSourceDto(source.Source, directives);
            if (!string.IsNullOrWhiteSpace(dto.Source) && dto.Directives.Any())
            {
                yield return dto;
            }
        }
    }

    private static int GetSortIndex(string source)
    {
        var index = CspConstants.AllSources.IndexOf(source);

        return index < 0 ? 100 : index;
    }

    private class CspSourceDto
    {
        public string Source { get; }

        public List<string> Directives { get; }

        public CspSourceDto(string? source, string? directives)
        {
            Source = source ?? string.Empty;
            Directives = directives?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct().ToList() ?? new List<string>(0);
        }
    }
}
