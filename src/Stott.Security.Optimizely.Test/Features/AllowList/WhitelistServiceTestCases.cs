namespace Stott.Security.Optimizely.Test.Features.AllowList;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

public static class AllowListServiceTestCases
{
    public static IEnumerable<TestCaseData> ValidAllowListTests
    {
        get
        {
            yield return new TestCaseData("https://www.example.com", CspConstants.Directives.DefaultSource);
        }
    }

    public static IEnumerable<TestCaseData> InvalidAllowListTests
    {
        get
        {
            yield return new TestCaseData(null, CspConstants.Directives.DefaultSource);
            yield return new TestCaseData(string.Empty, CspConstants.Directives.DefaultSource);
            yield return new TestCaseData("https://www.example.com", string.Empty);
            yield return new TestCaseData("https://www.example.com", null);
            yield return new TestCaseData(null, null);
            yield return new TestCaseData(string.Empty, string.Empty);
        }
    }

    public static IEnumerable<TestCaseData> AllowListTests
    {
        get
        {
            yield return new TestCaseData("https://www.siteone.com", CspConstants.Directives.DefaultSource, "https://www.siteone.com", CspConstants.Directives.DefaultSource, true);
            yield return new TestCaseData("https://www.siteone.com", CspConstants.Directives.DefaultSource, "https://*.siteone.com", CspConstants.Directives.DefaultSource, true);
            yield return new TestCaseData("https://www.siteone.com/child-page/", CspConstants.Directives.DefaultSource, "https://www.siteone.com", CspConstants.Directives.DefaultSource, true);
            yield return new TestCaseData("https://www.siteone.com/child-page/", CspConstants.Directives.DefaultSource, "https://*.siteone.com", CspConstants.Directives.DefaultSource, true);
            yield return new TestCaseData("https://www.siteone.com", CspConstants.Directives.DefaultSource, "https://www.sitetwo.com", CspConstants.Directives.DefaultSource, false);
            yield return new TestCaseData("https://www.siteone.com", CspConstants.Directives.DefaultSource, "https://*.sitetwo.com", CspConstants.Directives.DefaultSource, false);
            yield return new TestCaseData("https://www.siteone.com/child-page/", CspConstants.Directives.DefaultSource, "https://www.sitetwo.com", CspConstants.Directives.DefaultSource, false);
            yield return new TestCaseData("https://www.siteone.com/child-page/", CspConstants.Directives.DefaultSource, "https://*.sitetwo.com", CspConstants.Directives.DefaultSource, false);
        }
    }
}
