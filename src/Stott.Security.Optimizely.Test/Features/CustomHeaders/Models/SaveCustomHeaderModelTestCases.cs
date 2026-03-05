namespace Stott.Security.Optimizely.Test.Features.CustomHeaders.Models;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

public static class SaveCustomHeaderModelTestCases
{
    public static IEnumerable<TestCaseData> InvalidHeaderNameCharacters
    {
        get
        {
            yield return new TestCaseData("Header Name");
            yield return new TestCaseData("Header:Name");
            yield return new TestCaseData("Header;Name");
            yield return new TestCaseData("Header/Name");
            yield return new TestCaseData("Header?Name");
            yield return new TestCaseData("Header@Name");
            yield return new TestCaseData("Header[Name]");
        }
    }

    public static IEnumerable<TestCaseData> ValidHeaderNameCharacters
    {
        get
        {
            yield return new TestCaseData("X-Custom-Header");
            yield return new TestCaseData("X_Custom_Header");
            yield return new TestCaseData("Header123");
            yield return new TestCaseData("my-header");
        }
    }

    public static IEnumerable<TestCaseData> BuiltInCorsHeaders
    {
        get
        {
            yield return new TestCaseData(CspConstants.HeaderNames.CorsAllowOrigin);
            yield return new TestCaseData(CspConstants.HeaderNames.CorsAllowMethods);
            yield return new TestCaseData(CspConstants.HeaderNames.CorsAllowHeaders);
            yield return new TestCaseData(CspConstants.HeaderNames.CorsAllowCredentials);
            yield return new TestCaseData(CspConstants.HeaderNames.CorsMaxAge);
            yield return new TestCaseData(CspConstants.HeaderNames.CorsExposeHeaders);
        }
    }
}
