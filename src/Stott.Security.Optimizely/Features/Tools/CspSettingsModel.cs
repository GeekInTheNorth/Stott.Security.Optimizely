using System.Collections.Generic;

using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Features.Settings;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class CspSettingsModel : ICspSettings
{
    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    public bool IsAllowListEnabled { get; set; }

    public string? AllowListUrl { get; set; }

    public bool IsUpgradeInsecureRequestsEnabled { get; set; }

    public bool IsNonceEnabled { get; set; }

    public bool IsStrictDynamicEnabled { get; set; }

    public bool UseInternalReporting { get; set; }

    public bool UseExternalReporting { get; set; }

    public string? ExternalReportToUrl { get; set; }

    public string? ExternalReportUriUrl { get; set; }

    public SandboxModel? Sandbox { get; set; }

    public List<CspSourceModel>? Sources { get; set; }
}