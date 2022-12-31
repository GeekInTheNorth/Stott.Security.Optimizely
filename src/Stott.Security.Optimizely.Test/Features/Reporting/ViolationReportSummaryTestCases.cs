namespace Stott.Security.Optimizely.Test.Features.Reporting;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

public static class ViolationReportSummaryTestCases
{
    public static IEnumerable<TestCaseData> SanitizedSourceTestCases
    {
        get
        {
            foreach (var source in CspConstants.AllSources)
            {
                yield return new TestCaseData(source, source);
            }

            yield return new TestCaseData("https://www.example.com/sub-url/", "https://www.example.com");
            yield return new TestCaseData("https://www.example.com/sub-url/?q=123", "https://www.example.com");
            yield return new TestCaseData("https://localhost:5000/some-url/", "https://localhost:5000");
            yield return new TestCaseData("https://localhost:5000?q=123", "https://localhost:5000");
            yield return new TestCaseData("/relative-url/?q=123", "/relative-url/?q=123");
            yield return new TestCaseData("non-built-in-source-or-url", "non-built-in-source-or-url");
        }
    }
}