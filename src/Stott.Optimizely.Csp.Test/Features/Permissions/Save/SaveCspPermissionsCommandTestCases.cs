using System.Collections.Generic;

using NUnit.Framework;

namespace Stott.Optimizely.Csp.Test.Features.Permissions.Save
{
    public static class SaveCspPermissionsCommandTestCases
    {
        public static IEnumerable<TestCaseData> InvalidDirectivesTestCases
        {
            get
            {
                yield return new TestCaseData((List<string>)null);
                yield return new TestCaseData(new List<string>(0));
            }
        }
    }
}
