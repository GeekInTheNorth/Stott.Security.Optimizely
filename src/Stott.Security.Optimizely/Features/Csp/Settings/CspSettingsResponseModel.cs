namespace Stott.Security.Optimizely.Features.Csp.Settings;

public sealed class CspSettingsResponseModel
{
    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    public bool UseInternalReporting { get; set; }

    public bool UseExternalReporting { get; set; }

    public string ExternalReportToUrl { get; set; } = string.Empty;

    public bool IsAllowListEnabled { get; set; }

    public string AllowListUrl { get; set; } = string.Empty;

    public bool IsUpgradeInsecureRequestsEnabled { get; set; }

    public bool IsInherited { get; set; }
}
