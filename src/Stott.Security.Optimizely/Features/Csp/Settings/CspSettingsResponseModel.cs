namespace Stott.Security.Optimizely.Features.Csp.Settings;

/// <summary>
/// API response model for the CSP settings endpoint. Mirrors <see cref="CspSettingsModel"/>
/// but adds <see cref="IsInherited"/> so the UI can indicate whether the returned settings
/// came from the current context or from an ancestor (Site -> Global).
/// </summary>
public sealed class CspSettingsResponseModel
{
    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    public bool UseInternalReporting { get; set; }

    public bool UseExternalReporting { get; set; }

    public string? ExternalReportToUrl { get; set; }

    public bool IsAllowListEnabled { get; set; }

    public string? AllowListUrl { get; set; }

    public bool IsUpgradeInsecureRequestsEnabled { get; set; }

    public bool IsNonceEnabled { get; set; }

    public bool IsStrictDynamicEnabled { get; set; }

    public bool IsInherited { get; set; }
}
