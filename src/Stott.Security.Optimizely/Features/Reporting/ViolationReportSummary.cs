namespace Stott.Security.Optimizely.Features.Reporting;

using System;

using Stott.Security.Optimizely.Common;

public class ViolationReportSummary
{
    public int Key { get; set; }

    public string Source { get; set; }

    public string SanitizedSource
    {
        get
        {
            if (CspConstants.AllSources.Contains(Source))
            {
                return Source;
            }

            if (Uri.IsWellFormedUriString(Source, UriKind.Absolute))
            {
                return new Uri(Source).GetLeftPart(UriPartial.Authority);
            }

            return Source;
        }
    }

    public string Directive { get; set; }

    public int Violations { get; set; }

    public DateTime LastViolated { get; set; }
}