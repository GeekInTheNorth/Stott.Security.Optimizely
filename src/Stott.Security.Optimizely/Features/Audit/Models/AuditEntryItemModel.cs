namespace Stott.Security.Optimizely.Features.Audit.Models;

using System;

public class AuditEntryItemModel
{
    public Guid Id { get; set; }

    public string Field { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }
}