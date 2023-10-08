namespace Stott.Security.Optimizely.Test.Features.Cors;

using System.Collections.Generic;

using NUnit.Framework;

public static class CorsConfigurationTestCases
{
    public static IEnumerable<TestCaseData> InvalidOriginTestCases
    {
        get
        {
            yield return new TestCaseData("not-an-origin");
            yield return new TestCaseData("/relative-url/");
            yield return new TestCaseData("https://www.example.com/not-origin-only/");
            yield return new TestCaseData("https://www.example.com/?q=not-origin-only");
            yield return new TestCaseData("https://www.example.com/not-origin-only");
            yield return new TestCaseData("https://www.example.com?q=not-origin-only");
        }
    }

    public static IEnumerable<TestCaseData> ValidOriginTestCases
    {
        get
        {
            yield return new TestCaseData("https://www.example.com");
            yield return new TestCaseData("https://www.example.com/");
            yield return new TestCaseData("http://www.example.com");
            yield return new TestCaseData("https://localhost:5000/");
            yield return new TestCaseData("https://localhost:5000");
        }
    }

    public static IEnumerable<TestCaseData> InvalidHeaderTestCases
    {
        get
        {
            yield return new TestCaseData("not a valid header");
            yield return new TestCaseData("n0t~a~val1d~header");
        }
    }

    public static IEnumerable<TestCaseData> ValidHeaderTestCases
    {
        get
        {
            yield return new TestCaseData("valid-header");
            yield return new TestCaseData("Valid-Header");
            yield return new TestCaseData("valid");
            yield return new TestCaseData("valid_header");
            yield return new TestCaseData("v4l1d-h34d3r");
        }
    }
}