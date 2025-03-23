using System.Collections.Generic;

using NUnit.Framework;

namespace Stott.Security.Optimizely.Test.TestCases;

public static class CommonTestCases
{
    public static IEnumerable<TestCaseData> EmptyNullOrWhitespaceStrings
    {
        get
        {
            yield return new TestCaseData((string)null);
            yield return new TestCaseData(string.Empty);
            yield return new TestCaseData(" ");
        }
    }

    public static IEnumerable<TestCaseData> BooleanTestCases
    {
        get
        {
            yield return new TestCaseData(true);
            yield return new TestCaseData(false);
        }
    }
}
