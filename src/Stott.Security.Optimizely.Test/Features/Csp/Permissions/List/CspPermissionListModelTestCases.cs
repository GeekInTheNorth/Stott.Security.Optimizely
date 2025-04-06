namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.List;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

public static class CspPermissionListModelTestCases
{
    public static IEnumerable<TestCaseData> SortSourceTestCases
    {
        get
        {
            foreach (var source in CspConstants.AllSources)
            {
                yield return new TestCaseData(source, source);
            }

            yield return new TestCaseData("https://www.google.com/test-page/", "google.com : https://www.google.com/test-page/");
            yield return new TestCaseData("https://www.google.com", "google.com : https://www.google.com");
            yield return new TestCaseData("https://*.google.com", "google.com : https://0.google.com");
            yield return new TestCaseData("*.google.com", "0.google.com");
        }
    }
}