namespace Stott.Security.Optimizely.Test.Features.Header;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

public static class HeaderCompilationServiceTestCases
{
    public static IEnumerable<TestCaseData> GetEmptySourceTestCases
    {
        get
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData(new List<CspSource>(0));
        }
    }

    public static IEnumerable<TestCaseData> GetReportingEndpointsTestCases
    {
        get
        {
            yield return new TestCaseData(false, false, null, false, null);
            yield return new TestCaseData(true, false, null, true, "stott-security-endpoint=\"htts://www.example.com/report-to\"");
            yield return new TestCaseData(false, true, "https://www.external.com/report-to", true, "stott-security-external-endpoint=\"https://www.external.com/report-to\"");
            yield return new TestCaseData(true, true, "https://www.external.com/report-to", true, "stott-security-endpoint=\"htts://www.example.com/report-to\", stott-security-external-endpoint=\"https://www.external.com/report-to\"");
        }
    }

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
            yield return new TestCaseData(true, 200000, true, true, "max-age=200000; includeSubDomains");
            yield return new TestCaseData(true, 150000, false, true, "max-age=150000");
            yield return new TestCaseData(false, 150000, false, false, null);
            yield return new TestCaseData(false, 150000, true, false, null);
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