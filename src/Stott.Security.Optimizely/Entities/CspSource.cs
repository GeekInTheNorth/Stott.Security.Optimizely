namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Stott.Security.Optimizely.Features.Audit;

[Table("tbl_CspSource")]
public class CspSource : IAuditableEntity
{
    public Guid Id { get; set; }

    public string Source { get; set; }

    public string Directives { get; set; }
}