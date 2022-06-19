using System.Collections.Generic;

using NUnit.Framework;

namespace Stott.Security.Core.Test.Features.Permissions.Repository
{
    public static class CspPermissionRepositoryTestCases
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
