namespace Stott.Security.Optimizely.Test.Features.SecurityHeaders.Service;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

public static class SecurityHeaderServiceTestCases
{
    public static IEnumerable<TestCaseData> GetXContentTypeOptionsHeaderTestCases
    {
        get
        {
            yield return new TestCaseData(XContentTypeOptions.None, false);
            yield return new TestCaseData(XContentTypeOptions.NoSniff, true);
        }
    }

    public static IEnumerable<TestCaseData> GetXssProtectionHeaderTestCases
    {
        get
        {
            yield return new TestCaseData(XssProtection.None, false);
            yield return new TestCaseData(XssProtection.Enabled, true);
            yield return new TestCaseData(XssProtection.EnabledWithBlocking, true);
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

    public static IEnumerable<TestCaseData> GetCrossOriginEmbedderPolicyTestCases
    {
        get
        {
            yield return new TestCaseData(CrossOriginEmbedderPolicy.None, false);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.RequireCorp, true);
            yield return new TestCaseData(CrossOriginEmbedderPolicy.UnsafeNone, true);
        }
    }

    public static IEnumerable<TestCaseData> GetStrictTransportSecurityTestCases
    {
        get
        {
            yield return new TestCaseData(true, 200000, true, "max-age=200000; includeSubDomains");
            yield return new TestCaseData(true, 150000, false, "max-age=150000");
            yield return new TestCaseData(false, 150000, false, null);
            yield return new TestCaseData(false, 150000, true, null);
        }
    }
}