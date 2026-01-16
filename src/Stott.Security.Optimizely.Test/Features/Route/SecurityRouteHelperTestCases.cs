using System.Collections.Generic;
using NUnit.Framework;
using Stott.Security.Optimizely.Features.Route;

namespace Stott.Security.Optimizely.Test.Features.Route;

public static class SecurityRouteHelperTestCases
{
    public static IEnumerable<TestCaseData> StandardRouteTestCases()
    {
        yield return new TestCaseData("", SecurityRouteType.Default);
        yield return new TestCaseData("/safe-path", SecurityRouteType.Default);
        yield return new TestCaseData("/excluded-path", SecurityRouteType.NoNonceOrHash);
        yield return new TestCaseData("/excluded-path/sub-path", SecurityRouteType.NoNonceOrHash);
        yield return new TestCaseData("/another/excluded-path", SecurityRouteType.NoNonceOrHash);
        yield return new TestCaseData("/another/excluded-path/sub-path", SecurityRouteType.NoNonceOrHash);
    }

    public static IEnumerable<TestCaseData> CspSourceContentRouteTestCases()
    {
        yield return new TestCaseData("", SecurityRouteType.ContentSpecific);
        yield return new TestCaseData("/safe-path", SecurityRouteType.ContentSpecific);
        yield return new TestCaseData("/excluded-path", SecurityRouteType.ContentSpecificNoNonceOrHash);
        yield return new TestCaseData("/excluded-path/sub-path", SecurityRouteType.ContentSpecificNoNonceOrHash);
        yield return new TestCaseData("/another/excluded-path", SecurityRouteType.ContentSpecificNoNonceOrHash);
        yield return new TestCaseData("/another/excluded-path/sub-path", SecurityRouteType.ContentSpecificNoNonceOrHash);
    }
}