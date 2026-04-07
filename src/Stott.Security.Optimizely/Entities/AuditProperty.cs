#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

[Table("tbl_StottV7_AuditProperty")]
[Index(nameof(AuditHeaderId), IsUnique = false, Name = "idx_StottV7_AuditProperty_LookUp")]
public class AuditProperty
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Guid AuditHeaderId { get; set; }

    public string Field { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }

    public AuditHeader Header { get; set; }
}