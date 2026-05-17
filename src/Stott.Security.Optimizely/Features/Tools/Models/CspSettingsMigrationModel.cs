using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Stott.Security.Optimizely.Features.Tools.Models;

public sealed class CspSettingsMigrationModel
{
    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    public bool IsAllowListEnabled { get; set; }

    public string? AllowListUrl { get; set; }

    public bool IsUpgradeInsecureRequestsEnabled { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsNonceEnabled { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsStrictDynamicEnabled { get; set; }

    public bool UseInternalReporting { get; set; }

    public bool UseExternalReporting { get; set; }

    public string? ExternalReportToUrl { get; set; }

    public CspSandboxMigrationModel? Sandbox { get; set; }

    public List<CspSourceMigrationModel>? Sources { get; set; }
}
