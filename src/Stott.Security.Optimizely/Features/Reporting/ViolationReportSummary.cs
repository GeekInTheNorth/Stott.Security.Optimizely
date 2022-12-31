namespace Stott.Security.Optimizely.Features.Reporting;

using System;

public class ViolationReportSummary
{
    public int Key { get; set; }

    public string Source { get; set; }

    public string Directive { get; set; }

    public int Violations { get; set; }

    public DateTime LastViolated { get; set; }
}