#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.Csp;

[Table("tbl_CspSource")]
public class CspSource : IAuditableEntity, ICspSourceMapping
{
    public Guid Id { get; set; }

    public string Source { get; set; }

    public string Directives { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}