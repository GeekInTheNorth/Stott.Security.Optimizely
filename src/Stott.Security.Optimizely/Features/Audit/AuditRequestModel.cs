namespace Stott.Security.Optimizely.Features.Audit;

using System;

public class AuditRequestModel
{
    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public string ActionedBy { get; set; }

    public string RecordType { get; set; }

    public string OperationType { get; set; }
}