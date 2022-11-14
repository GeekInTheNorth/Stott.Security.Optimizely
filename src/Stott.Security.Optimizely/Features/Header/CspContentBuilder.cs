namespace Stott.Security.Optimizely.Features.Header;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Sandbox;

public class CspContentBuilder : ICspContentBuilder
{
    private bool _sendViolationReport;

    private string _violationReportUrl;

    private List<CspSourceDto> _cspSources;

    private CspSettings _cspSettings;

    private SandboxModel _cspSandbox;

    public ICspContentBuilder WithSettings(CspSettings cspSettings)
    {
        _cspSettings = cspSettings;

        return this;
    }

    public ICspContentBuilder WithSandbox(SandboxModel cspSandbox)
    {
        _cspSandbox = cspSandbox;

        return this;
    }

    public ICspContentBuilder WithSources(IEnumerable<CspSource> sources)
    {
        _cspSources = ConvertToDtos(sources).ToList();

        return this;
    }

    public ICspContentBuilder WithReporting(bool sendViolationReport, string violationReportUrl)
    {
        _sendViolationReport = sendViolationReport;
        _violationReportUrl = violationReportUrl;

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

        var sandboxSettings = GetSandboxSettings().ToList();
        if (sandboxSettings.Any())
        {
            stringBuilder.Append($"{string.Join(' ', sandboxSettings)}; ");
        }

        if (_sendViolationReport && !string.IsNullOrWhiteSpace(_violationReportUrl))
        {
            stringBuilder.Append($"report-to {_violationReportUrl};");
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

        foreach (var directive in distinctDirectives)
        {
            var directiveSources = _cspSources.Where(x => x.Directives.Contains(directive))
                                              .Select(x => x.Source?.ToLower())
                                              .OrderBy(x => GetSortIndex(x))
                                              .ThenBy(x => x)
                                              .Distinct();

            yield return $"{directive} {string.Join(" ", directiveSources)}; ";
        }
    }

    private static IEnumerable<CspSourceDto> ConvertToDtos(IEnumerable<CspSource> sources)
    {
        if (sources == null)
        {
            yield break;
        }

        foreach (var source in sources)
        {
            var dto = new CspSourceDto
            {
                Source = source.Source,
                Directives = source.Directives?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList()
            };

            if (dto.Directives != null && dto.Directives.Any())
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

        if (_cspSandbox.IsSandboxEnabled) { yield return "sandbox"; }
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
        public string Source { get; set; }

        public List<string> Directives { get; set; }
    }
}