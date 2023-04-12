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

            yield return new TestCaseData(null, string.Empty);
            yield return new TestCaseData(string.Empty, string.Empty);
            yield return new TestCaseData(" ", string.Empty);
            yield return new TestCaseData("https://www.example.com/sub-url/", "https://www.example.com");
            yield return new TestCaseData("https://www.example.com/sub-url/?q=123", "https://www.example.com");
            yield return new TestCaseData("https://localhost:5000/some-url/", "https://localhost:5000");
            yield return new TestCaseData("https://localhost:5000?q=123", "https://localhost:5000");
            yield return new TestCaseData("/relative-url/?q=123", "/relative-url/?q=123");
            yield return new TestCaseData("non-built-in-source-or-url", "non-built-in-source-or-url");
        }
    }

    public static IEnumerable<TestCaseData> DomainSuggestionTestCases
    {
        get
        {
            yield return new TestCaseData(null, new List<string>(0));
            yield return new TestCaseData(string.Empty, new List<string>(0));
            yield return new TestCaseData(" ", new List<string>(0));
            yield return new TestCaseData("https://localhost:8000", new List<string> { "https://localhost:8000" });
            yield return new TestCaseData("https://localhost:8000/some-path/", new List<string> { "https://localhost:8000" });
            yield return new TestCaseData("https://stackoverflow.com/some-path/", new List<string> { "https://stackoverflow.com" });
            yield return new TestCaseData("/relative-url/", new List<string> { "/relative-url/" });

            yield return new TestCaseData(
                "https://abc.global.siteimproveanalytics.io/some-path",
                new List<string>
                {
                    "https://abc.global.siteimproveanalytics.io",
                    "https://*.global.siteimproveanalytics.io",
                    "https://*.siteimproveanalytics.io"
                });

            yield return new TestCaseData(
                "https://www.gov.uk/some-path",
                new List<string>
                {
                    "https://www.gov.uk",
                    "https://*.gov.uk"
                });
        }
    }
}