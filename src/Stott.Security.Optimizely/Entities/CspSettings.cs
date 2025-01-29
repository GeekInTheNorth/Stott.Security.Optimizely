#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.Csp.Settings;

[Table("tbl_CspSettings")]
public class CspSettings : IAuditableEntity, ICspSettings
{
    public Guid Id { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    [Column("IsWhitelistEnabled")]
    public bool IsAllowListEnabled { get; set; }

    [Column("WhitelistUrl")]
    public string AllowListUrl { get; set; }

    public bool IsUpgradeInsecureRequestsEnabled { get; set; }

    public bool IsNonceEnabled { get; set; }

    public bool IsStrictDynamicEnabled { get; set; }

    public bool UseInternalReporting { get; set; }

    public bool UseExternalReporting { get; set; }

    public string ExternalReportToUrl { get; set; }

    public string ExternalReportUriUrl { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}