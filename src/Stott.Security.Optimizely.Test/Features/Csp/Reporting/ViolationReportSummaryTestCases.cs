namespace Stott.Security.Optimizely.Test.Features.Csp.Reporting;

using System.Collections.Generic;
using System.Linq;

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

    public static IEnumerable<TestCaseData> SourceSuggestionTestCases
    {
        get
        {
            yield return new TestCaseData(null, new List<string>(0));
            yield return new TestCaseData(string.Empty, new List<string>(0));
            yield return new TestCaseData(" ", new List<string>(0));
            yield return new TestCaseData("https://localhost:8000", new List<string> { "https://localhost:8000" });
            yield return new TestCaseData("https://localhost:8000/some-path/", new List<string> { "https://localhost:8000" });
            yield return new TestCaseData("https://stackoverflow.com/some-path/", new List<string> { "https://stackoverflow.com" });
            yield return new TestCaseData("http://stackoverflow.com/some-path/", new List<string> { "http://stackoverflow.com" });
            yield return new TestCaseData("wss://localhost:44369/OptimizelyTwelveTest/", new List<string> { "wss://localhost:44369" });
            yield return new TestCaseData("ws://localhost:44369/OptimizelyTwelveTest/", new List<string> { "ws://localhost:44369" });
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

    public static IEnumerable<TestCaseData> NonUrlSourceSuggestionTestCases
    {
        get
        {
            return CspConstants.AllSources.Select(x => new TestCaseData(x));
        }
    }

    public static IEnumerable<TestCaseData> DirectiveSuggestionTestCases
    {
        get
        {
            yield return new TestCaseData(CspConstants.Directives.BaseUri, new List<string> { CspConstants.Directives.BaseUri });
            yield return new TestCaseData(CspConstants.Directives.ChildSource, new List<string> { CspConstants.Directives.FrameSource, CspConstants.Directives.ChildSource });
            yield return new TestCaseData(CspConstants.Directives.ConnectSource, new List<string> { CspConstants.Directives.ConnectSource });
            yield return new TestCaseData(CspConstants.Directives.DefaultSource, new List<string> { CspConstants.Directives.DefaultSource });
            yield return new TestCaseData(CspConstants.Directives.FontSource, new List<string> { CspConstants.Directives.FontSource });
            yield return new TestCaseData(CspConstants.Directives.FormAction, new List<string> { CspConstants.Directives.FormAction });
            yield return new TestCaseData(CspConstants.Directives.FrameAncestors, new List<string> { CspConstants.Directives.FrameAncestors });
            yield return new TestCaseData(CspConstants.Directives.FrameSource, new List<string> { CspConstants.Directives.FrameSource, CspConstants.Directives.ChildSource });
            yield return new TestCaseData(CspConstants.Directives.ImageSource, new List<string> { CspConstants.Directives.ImageSource });
            yield return new TestCaseData(CspConstants.Directives.ManifestSource, new List<string> { CspConstants.Directives.ManifestSource });
            yield return new TestCaseData(CspConstants.Directives.MediaSource, new List<string> { CspConstants.Directives.MediaSource });
            yield return new TestCaseData(CspConstants.Directives.ObjectSource, new List<string> { CspConstants.Directives.ObjectSource });
            yield return new TestCaseData(CspConstants.Directives.ScriptSourceAttribute, new List<string> { CspConstants.Directives.ScriptSourceAttribute, CspConstants.Directives.ScriptSource });
            yield return new TestCaseData(CspConstants.Directives.ScriptSourceElement, new List<string> { CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSource });
            yield return new TestCaseData(CspConstants.Directives.ScriptSource, new List<string> { CspConstants.Directives.ScriptSource });
            yield return new TestCaseData(CspConstants.Directives.StyleSourceAttribute, new List<string> { CspConstants.Directives.StyleSourceAttribute, CspConstants.Directives.StyleSource });
            yield return new TestCaseData(CspConstants.Directives.StyleSourceElement, new List<string> { CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSource });
            yield return new TestCaseData(CspConstants.Directives.StyleSource, new List<string> { CspConstants.Directives.StyleSource });
            yield return new TestCaseData(CspConstants.Directives.WorkerSource, new List<string> { CspConstants.Directives.WorkerSource });
        }
    }
}