#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.Csp;

[Table("tbl_CspSource")]
[Index(nameof(Source), nameof(SiteId), nameof(HostName), IsUnique = false, Name = "idx_CspSource_LookUp")]
public class CspSource : IAuditableEntity, ICspSourceMapping
{
    public Guid Id { get; set; }

    public string Source { get; set; }

    public string Directives { get; set; }

    public Guid? SiteId { get; set; }

    [MaxLength(200)]
    public string HostName { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}