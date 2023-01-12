#nullable disable
namespace Stott.Security.Optimizely.Entities;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

[Table("tbl_CspAuditHeader")]
[Index(nameof(Actioned), nameof(ActionedBy), nameof(RecordType), IsUnique = false, Name = "idx_CspAuditHeader_LookUp")]
public class AuditHeader
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime Actioned { get; set; }

    public string ActionedBy { get; set; }

    public string OperationType { get; set; }

    public string RecordType { get; set; }

    public string Identifier { get; set; }

    public ICollection<AuditProperty> AuditProperties { get; set; }
}