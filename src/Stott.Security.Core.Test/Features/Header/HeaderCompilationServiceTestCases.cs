using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.SecurityHeaders.Enums;

namespace Stott.Security.Core.Test.Features.Header
{
    public static class HeaderCompilationServiceTestCases
    {
        public static IEnumerable<TestCaseData> GetEmptySourceTestCases
        {
            get
            {
                yield return new TestCaseData(null, null);
                yield return new TestCaseData(null, new List<CspSource>(0));
                yield return new TestCaseData(new List<CspSource>(0), null);
                yield return new TestCaseData(new List<CspSource>(0), new List<CspSource>(0));
            }
        }

        public static IEnumerable<TestCaseData> GetReferrerPolicyTestCases
        {
            get
            {
                yield return new TestCaseData(ReferrerPolicy.None, false);
                yield return new TestCaseData(ReferrerPolicy.NoReferrer, true);
                yield return new TestCaseData(ReferrerPolicy.NoReferrerWhenDowngrade, true);
                yield return new TestCaseData(ReferrerPolicy.Origin, true);
                yield return new TestCaseData(ReferrerPolicy.OriginWhenCrossOrigin, true);
                yield return new TestCaseData(ReferrerPolicy.SameOrigin, true);
                yield return new TestCaseData(ReferrerPolicy.StrictOrigin, true);
                yield return new TestCaseData(ReferrerPolicy.StrictOriginWhenCrossOrigin, true);
                yield return new TestCaseData(ReferrerPolicy.UnsafeUrl, true);
            }
        }

        public static IEnumerable<TestCaseData> GetFrameOptionsTestCases
        {
            get
            {
                yield return new TestCaseData(XFrameOptions.None, false);
                yield return new TestCaseData(XFrameOptions.SameOrigin, true);
                yield return new TestCaseData(XFrameOptions.Deny, true);
            }
        }

        public static IEnumerable<TestCaseData> GetCspReportOnlyTestCases
        {
            get
            {
                yield return new TestCaseData(false, CspConstants.HeaderNames.ContentSecurityPolicy);
                yield return new TestCaseData(true, CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
            }
        }
    }
}
