#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Optimizely.Features.Audit;

[Table("tbl_CorsSettings")]
public class CorsSettings : IAuditableEntity
{
    public Guid Id { get; set; }

    public bool IsEnabled { get; set; }

    public bool AllowCredentials { get; set; }

    public string AllowHeaders { get; set; }

    public string AllowMethods { get; set; }

    public string AllowOrigins { get; set; }

    public int MaxAge { get; set; }

    public string ExposeHeaders { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}