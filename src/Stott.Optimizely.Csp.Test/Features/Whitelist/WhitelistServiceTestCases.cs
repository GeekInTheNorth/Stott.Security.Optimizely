using System.Collections.Generic;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;

namespace Stott.Optimizely.Csp.Test.Features.Whitelist
{
    public static class WhitelistServiceTestCases
    {
        public static IEnumerable<TestCaseData> ValidWhiteListTests
        {
            get
            {
                yield return new TestCaseData("https://www.example.com", CspConstants.Directives.DefaultSource);
            }
        }

        public static IEnumerable<TestCaseData> InvalidWhiteListTests
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
    }
}
