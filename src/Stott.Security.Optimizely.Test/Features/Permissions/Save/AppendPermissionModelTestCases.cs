namespace Stott.Security.Optimizely.Test.Features.Permissions.Save;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

public static class AppendPermissionModelTestCases
{
    public static IEnumerable<TestCaseData> DirectiveTestCases
    {
        get
        {
            foreach(var directive in CspConstants.AllDirectives)
            {
                yield return new TestCaseData(directive, false);
            }

            yield return new TestCaseData(null, true);
            yield return new TestCaseData(string.Empty, true);
            yield return new TestCaseData(" ", true);
            yield return new TestCaseData("not-a-directive", true);
        }
    }
}