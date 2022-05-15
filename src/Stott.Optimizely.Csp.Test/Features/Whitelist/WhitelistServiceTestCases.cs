using System.Collections.Generic;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;

namespace Stott.Optimizely.Csp.Test.Features.Whitelist
{
    public static class WhitelistServiceTestCases
    {
        public static IEnumerable<TestCaseData> ValidWhitelistTests
        {
            get
            {
                yield return new TestCaseData("https://www.example.com", CspConstants.Directives.DefaultSource);
            }
        }

        public static IEnumerable<TestCaseData> InvalidWhitelistTests
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

        public static IEnumerable<TestCaseData> WhitelistTests
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
}
