#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Optimizely.Features.Audit;

[Table("tbl_CspSettings")]
public class CspSettings : IAuditableEntity
{
    public Guid Id { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    public bool IsWhitelistEnabled { get; set; }

    public string WhitelistUrl { get; set; }

    public bool IsUpgradeInsecureRequestsEnabled { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}