namespace Stott.Security.Optimizely.Test.Features.Cors.Provider;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

public static class CustomCorsPolicyProviderTestCases
{
    public static IEnumerable<TestCaseData> GetNullOrDefaultPolicyNameTestCases
    {
        get
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData("");
            yield return new TestCaseData(" ");
            yield return new TestCaseData(CspConstants.CorsPolicy);
        }
    }
}