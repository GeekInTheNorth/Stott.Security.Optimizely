namespace Stott.Security.Optimizely.Entities;

using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

[Table("tbl_CspAuditEntry")]
[Index(nameof(Actioned), nameof(ActionedBy), nameof(RecordType), IsUnique = false, Name = "idx_CspAuditEntry_LookUp")]
public class AuditEntry
{
    public Guid Id { get; set; }

    public DateTime Actioned { get; set; }

    public string ActionedBy { get; set; }

    public string OperationType { get; set; }

    public string RecordType { get; set; }

    public string Field { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }
}