using System.Collections.Generic;

using Microsoft.Net.Http.Headers;

using NUnit.Framework;

namespace Stott.Security.Optimizely.Test.Extensions;

public static class HeaderDictionaryExtensionsTestCases
{
    public static IEnumerable<TestCaseData> ValidHeaderTestCases
    {
        get
        {
            yield return new TestCaseData(HeaderNames.CacheControl, "public, max-age=31557600");
            yield return new TestCaseData(HeaderNames.AccessControlAllowHeaders, "*");
        }
    }

    public static IEnumerable<TestCaseData> InvalidHeaderTestCases
    {
        get
        {
            yield return new TestCaseData(" ", " ");
            yield return new TestCaseData(" ", string.Empty);
            yield return new TestCaseData(" ", null);
            yield return new TestCaseData(string.Empty, " ");
            yield return new TestCaseData(string.Empty, string.Empty);
            yield return new TestCaseData(string.Empty, null);
            yield return new TestCaseData(null, " ");
            yield return new TestCaseData(null, string.Empty);
            yield return new TestCaseData(null, null);
        }
    }
}
