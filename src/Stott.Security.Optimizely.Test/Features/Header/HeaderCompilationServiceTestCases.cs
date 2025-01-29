namespace Stott.Security.Optimizely.Test.Features.Header;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;

public static class HeaderCompilationServiceTestCases
{
    public static IEnumerable<TestCaseData> GetEmptySourceTestCases
    {
        get
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData(new List<CspSource>(0));
        }
    }

    public static IEnumerable<TestCaseData> GetReportingEndpointsTestCases
    {
        get
        {
            yield return new TestCaseData(false, false, null, false, null);
            yield return new TestCaseData(true, false, null, true, "stott-security-endpoint=\"htts://www.example.com/report-to\"");
            yield return new TestCaseData(false, true, "https://www.external.com/report-to", true, "stott-security-external-endpoint=\"https://www.external.com/report-to\"");
            yield return new TestCaseData(true, true, "https://www.external.com/report-to", true, "stott-security-endpoint=\"htts://www.example.com/report-to\", stott-security-external-endpoint=\"https://www.external.com/report-to\"");
        }
    }

    public static IEnumerable<TestCaseData> GetCspReportOnlyTestCases
    {
        get
        {
            yield return new TestCaseData(false, CspConstants.HeaderNames.ContentSecurityPolicy);
            yield return new TestCaseData(true, CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        }
    }
}