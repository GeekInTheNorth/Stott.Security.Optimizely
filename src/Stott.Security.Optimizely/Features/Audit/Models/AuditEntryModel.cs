namespace Stott.Security.Optimizely.Features.Audit.Models;

using System;
using System.Collections.Generic;

public class AuditEntryModel
{
    public Guid Id { get; set; }

    public DateTime Actioned { get; set; }

    public string ActionedBy { get; set; }

    public string OperationType { get; set; }

    public string RecordType { get; set; }

    public string Identifier { get; set; }

    public IEnumerable<AuditEntryItemModel> Changes { get; set; }
}