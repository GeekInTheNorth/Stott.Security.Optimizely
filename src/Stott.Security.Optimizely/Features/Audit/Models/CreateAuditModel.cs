using System;
using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Audit.Models;

public class CreateAuditModel
{
    public DateTime Actioned { get; set; }

    public string? ActionedBy { get; set; }

    public string? OperationType { get; set; }

    public string? RecordType { get; set; }

    public string? Identifier { get; set; }

    public List<CreateAuditItem>? Changes { get; set; }

    public class CreateAuditItem
    {
        public string? PropertyName { get; set; }
        
        public string? OriginalValue { get; set; }
        
        public string? NewValue { get; set; }

        public bool HasChanged => !string.Equals(OriginalValue, NewValue, StringComparison.OrdinalIgnoreCase);
    }
}
