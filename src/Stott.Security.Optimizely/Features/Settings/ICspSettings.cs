﻿namespace Stott.Security.Optimizely.Features.Settings;

public interface ICspSettings
{
    bool IsEnabled { get; }

    bool IsReportOnly { get; }

    bool IsAllowListEnabled { get; }

    string? AllowListUrl { get; }

    bool IsUpgradeInsecureRequestsEnabled { get; }

    bool IsNonceEnabled { get; }

    bool IsStrictDynamicEnabled { get; }

    bool UseInternalReporting { get; }

    bool UseExternalReporting { get; }

    string? ExternalReportToUrl { get; }

    string? ExternalReportUriUrl { get; }
}