#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Features.Audit;

namespace Stott.Security.Optimizely.Entities;

[Table("tbl_stott_permissionpolicy")]
[Index(nameof(Directive), IsUnique = false, Name = "idx_permissionpolicy_lookUp")]
public class PermissionPolicy : IAuditableEntity
{
    public Guid Id { get; set; }

    [MaxLength(50)]
    public string Directive { get; set; }

    [MaxLength(50)]
    public string EnabledState { get; set; }

    public string Origins { get; set; }

    public DateTime Modified { get; set; }

    public string ModifiedBy { get; set; }
}
