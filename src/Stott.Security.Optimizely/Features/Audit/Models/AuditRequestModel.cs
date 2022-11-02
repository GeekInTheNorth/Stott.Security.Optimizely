namespace Stott.Security.Optimizely.Features.Audit.Models;

using System;

public class AuditRequestModel
{
    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public string ActionedBy { get; set; }

    public string RecordType { get; set; }

    public string OperationType { get; set; }

    public int From { get; set; }

    public int Take { get; set; }
}