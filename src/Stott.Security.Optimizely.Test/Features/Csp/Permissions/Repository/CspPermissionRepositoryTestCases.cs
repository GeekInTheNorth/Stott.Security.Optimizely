using System.Collections.Generic;

using NUnit.Framework;
using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.Repository;

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

    public static IEnumerable<TestCaseData> AppendHandlesSimilarDirectivesTestCases
    {
        get
        {
            yield return new TestCaseData(CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSource, $"{CspConstants.Directives.ScriptSourceElement},{CspConstants.Directives.ScriptSource}");
            yield return new TestCaseData(CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, $"{CspConstants.Directives.ScriptSource},{CspConstants.Directives.ScriptSourceElement}");
            yield return new TestCaseData(CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSourceElement);
            yield return new TestCaseData(CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSource);
            yield return new TestCaseData(CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSource, $"{CspConstants.Directives.StyleSourceElement},{CspConstants.Directives.StyleSource}");
            yield return new TestCaseData(CspConstants.Directives.StyleSource, CspConstants.Directives.StyleSourceElement, $"{CspConstants.Directives.StyleSource},{CspConstants.Directives.StyleSourceElement}");
            yield return new TestCaseData(CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSourceElement);
            yield return new TestCaseData(CspConstants.Directives.StyleSource, CspConstants.Directives.StyleSource, CspConstants.Directives.StyleSource);
        }
    }
}
