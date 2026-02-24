namespace Stott.Security.Optimizely.Test.Features.CustomHeaders.Repository;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

public static class CustomHeaderMapperTestCases
{
    public static IEnumerable<TestCaseData> FixedHeaderNames
    {
        get
        {
            yield return new TestCaseData(CspConstants.HeaderNames.XssProtection);
            yield return new TestCaseData(CspConstants.HeaderNames.FrameOptions);
            yield return new TestCaseData(CspConstants.HeaderNames.ContentTypeOptions);
            yield return new TestCaseData(CspConstants.HeaderNames.ReferrerPolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.CrossOriginEmbedderPolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.CrossOriginOpenerPolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.CrossOriginResourcePolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.StrictTransportSecurity);
        }
    }

    public static IEnumerable<TestCaseData> NonHstsFixedHeaders
    {
        get
        {
            yield return new TestCaseData(CspConstants.HeaderNames.XssProtection);
            yield return new TestCaseData(CspConstants.HeaderNames.FrameOptions);
            yield return new TestCaseData(CspConstants.HeaderNames.ContentTypeOptions);
            yield return new TestCaseData(CspConstants.HeaderNames.ReferrerPolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.CrossOriginEmbedderPolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.CrossOriginOpenerPolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.CrossOriginResourcePolicy);
        }
    }

    public static IEnumerable<TestCaseData> FixedSelectHeaders
    {
        get
        {
            yield return new TestCaseData(CspConstants.HeaderNames.XssProtection);
            yield return new TestCaseData(CspConstants.HeaderNames.FrameOptions);
            yield return new TestCaseData(CspConstants.HeaderNames.ContentTypeOptions);
            yield return new TestCaseData(CspConstants.HeaderNames.ReferrerPolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.CrossOriginEmbedderPolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.CrossOriginOpenerPolicy);
            yield return new TestCaseData(CspConstants.HeaderNames.CrossOriginResourcePolicy);
        }
    }
}
