namespace Stott.Security.Optimizely.Features.Settings;

public interface ICspSettings
{
    bool IsEnabled { get; }

    bool IsReportOnly { get; }

    bool IsWhitelistEnabled { get; }

    string? WhitelistUrl { get; }

    bool IsUpgradeInsecureRequestsEnabled { get; }
}