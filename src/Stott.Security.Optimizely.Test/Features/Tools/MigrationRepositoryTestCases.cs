using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Tools;

namespace Stott.Security.Optimizely.Test.Features.Tools;

public static class MigrationRepositoryTestCases
{
    public static IEnumerable<TestCaseData> GetInvalidArgumentsTestCases
    {
        get
        {
            yield return new TestCaseData(null, null);
            yield return new TestCaseData(null, string.Empty);
            yield return new TestCaseData(null, " ");
            yield return new TestCaseData(new SettingsModel(), null);
            yield return new TestCaseData(new SettingsModel(), string.Empty);
            yield return new TestCaseData(new SettingsModel(), " ");
        }
    }

    public static IEnumerable<TestCaseData> GetNonceDirectiveTestCases
    {
        get
        {
            return CspConstants.AllDirectives.Select(x => new TestCaseData(x, CspConstants.NonceDirectives.Contains(x)));
        }
    }
}