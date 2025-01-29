namespace Stott.Security.Optimizely.Features.Header;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Settings;

internal sealed class CspContentBuilder : ICspContentBuilder
{
    private readonly ICspReportUrlResolver _cspReportUrlResolver;

    private List<CspSourceDto>? _cspSources;

    private ICspSettings? _cspSettings;

    private SandboxModel? _cspSandbox;

    public CspContentBuilder(ICspReportUrlResolver cspReportUrlResolver)
    {
        _cspReportUrlResolver = cspReportUrlResolver;
    }

    public ICspContentBuilder WithSettings(ICspSettings cspSettings)
    {
        _cspSettings = cspSettings;

        return this;
    }

    public ICspContentBuilder WithSandbox(SandboxModel cspSandbox)
    {
        _cspSandbox = cspSandbox;

        return this;
    }

    public ICspContentBuilder WithSources(IEnumerable<ICspSourceMapping> sources)
    {
        _cspSources = ConvertToDtos(sources).ToList();

        return this;
    }

    public string BuildAsync()
    {
        var stringBuilder = new StringBuilder();

        var directives = GetDirectives().ToList();
        foreach (var directive in directives)
        {
            stringBuilder.Append(directive);
        }

        if (_cspSettings is { IsUpgradeInsecureRequestsEnabled: true })
        {
            stringBuilder.Append($"{CspConstants.Directives.UpgradeInsecureRequests};");
        }

        var sandboxSettings = GetSandboxSettings().ToList();
        if (sandboxSettings.Any())
        {
            stringBuilder.Append($"{string.Join(' ', sandboxSettings)}; ");
        }

        var reportToEndPoints = GetReportToEndPoints().ToList();
        if (reportToEndPoints.Any())
        {
            stringBuilder.Append($"report-to {string.Join(' ', reportToEndPoints)};");
        }
        
        var reportUriAddresses = GetReportUriAddresses().ToList();
        if (reportUriAddresses.Any())
        {
            stringBuilder.Append($"report-uri {string.Join(' ', reportUriAddresses)};");
        }

        return stringBuilder.ToString().Trim();
    }

    private IEnumerable<string> GetDirectives()
    {
        if (_cspSources == null || !_cspSources.Any())
        {
            yield break;
        }

        var stringBuilder = new StringBuilder();
        var distinctDirectives = _cspSources.SelectMany(x => x.Directives)
                                            .Distinct()
                                            .ToList();
        var noneDirectives = _cspSources.Where(x => x.Source.Equals(CspConstants.Sources.None))
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

            var directiveSources = _cspSources.Where(x => x.Directives.Contains(directive))
                                              .Select(x => x.Source.ToLower())
                                              .OrderBy(x => GetSortIndex(x))
                                              .ThenBy(x => x)
                                              .Distinct()
                                              .ToList();

            if (_cspSettings is { IsNonceEnabled: true } && CspConstants.NonceDirectives.Contains(directive))
            {
                directiveSources.Add(CspConstants.NoncePlaceholder);
            }

            yield return $"{directive} {string.Join(" ", directiveSources)}; ";
        }
    }

    private static IEnumerable<CspSourceDto> ConvertToDtos(IEnumerable<ICspSourceMapping> sources)
    {
        if (sources == null)
        {
            yield break;
        }

        foreach (var source in sources)
        {
            var dto = new CspSourceDto(source.Source, source.Directives);
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

    private IEnumerable<string> GetSandboxSettings()
    {
        if (_cspSettings == null || _cspSandbox == null)
        {
            yield break;
        }

        if (!_cspSandbox.IsSandboxEnabled || !_cspSettings.IsEnabled || _cspSettings.IsReportOnly)
        {
            yield break;
        }

        if (_cspSandbox.IsSandboxEnabled) { yield return CspConstants.Directives.Sandbox; }
        if (_cspSandbox.IsAllowDownloadsEnabled) { yield return "allow-downloads"; }
        if (_cspSandbox.IsAllowDownloadsWithoutGestureEnabled) { yield return "allow-downloads-without-user-activation"; }
        if (_cspSandbox.IsAllowFormsEnabled) { yield return "allow-forms"; }
        if (_cspSandbox.IsAllowModalsEnabled) { yield return "allow-modals"; }
        if (_cspSandbox.IsAllowOrientationLockEnabled) { yield return "allow-orientation-lock"; }
        if (_cspSandbox.IsAllowPointerLockEnabled) { yield return "allow-pointer-lock"; }
        if (_cspSandbox.IsAllowPopupsEnabled) { yield return "allow-popups"; }
        if (_cspSandbox.IsAllowPopupsToEscapeTheSandboxEnabled) { yield return "allow-popups-to-escape-sandbox"; }
        if (_cspSandbox.IsAllowPresentationEnabled) { yield return "allow-presentation"; }
        if (_cspSandbox.IsAllowSameOriginEnabled) { yield return "allow-same-origin"; }
        if (_cspSandbox.IsAllowScriptsEnabled) { yield return "allow-scripts"; }
        if (_cspSandbox.IsAllowStorageAccessByUserEnabled) { yield return "allow-storage-access-by-user-activation"; }
        if (_cspSandbox.IsAllowTopNavigationEnabled) { yield return "allow-top-navigation"; }
        if (_cspSandbox.IsAllowTopNavigationByUserEnabled) { yield return "allow-top-navigation-by-user-activation"; }
        if (_cspSandbox.IsAllowTopNavigationToCustomProtocolEnabled) { yield return "allow-top-navigation-to-custom-protocols"; }
    }

    private class CspSourceDto
    {
        public string Source { get; }

        public List<string> Directives { get; }

        public CspSourceDto(string? source, string? directives)
        {
            Source = source ?? string.Empty;
            Directives = directives?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList() ?? new List<string>(0);
        }
    }

    private IEnumerable<string> GetReportUriAddresses()
    {
        if (_cspSettings is { UseInternalReporting: true })
        {
            yield return _cspReportUrlResolver.GetReportUriPath();
        }

        if (_cspSettings is { UseExternalReporting: true, ExternalReportUriUrl.Length: >0 })
        {
            yield return _cspSettings.ExternalReportUriUrl;
        }
    }

    private IEnumerable<string> GetReportToEndPoints()
    {
        if (_cspSettings is { UseInternalReporting: true })
        {
            yield return "stott-security-endpoint";
        }

        if (_cspSettings is { UseExternalReporting: true, ExternalReportUriUrl.Length: > 0 })
        {
            yield return "stott-security-external-endpoint";
        }
    }
}