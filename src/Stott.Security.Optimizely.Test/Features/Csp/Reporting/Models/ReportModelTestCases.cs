namespace Stott.Security.Optimizely.Test.Features.Csp.Reporting.Models;

using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

public static class ReportModelTestCases
{
    public static IEnumerable<TestCaseData> PartialMatchingDirectiveTestCases
    {
        get
        {
            return CspConstants.AllDirectives.Select(x => new TestCaseData($"{x} ''self'' data: https://*.global.siteimproveanalytics.io https://*.google-analytics.com https", x));
        }
    }

    public static IEnumerable<TestCaseData> MatchingDirectiveTestCases
    {
        get
        {
            return CspConstants.AllDirectives.Select(x => new TestCaseData(x));
        }
    }
}