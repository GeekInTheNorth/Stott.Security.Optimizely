using System.Collections.Generic;

using NUnit.Framework;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Test.Features.Header
{
    public static class SecurityHeaderServiceTestCases
    {
        public static IEnumerable<TestCaseData> GetEmptySourceTestCases
        {
            get
            {
                yield return new TestCaseData((List<CspSource>)null, (List<CspSource>)null);
                yield return new TestCaseData((List<CspSource>)null, new List<CspSource>(0));
                yield return new TestCaseData(new List<CspSource>(0), (List<CspSource>)null);
                yield return new TestCaseData(new List<CspSource>(0), new List<CspSource>(0));
            }
        }
    }
}
