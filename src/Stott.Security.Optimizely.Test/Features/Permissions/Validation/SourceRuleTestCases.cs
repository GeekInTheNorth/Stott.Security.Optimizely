using System;
using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Test.Features.Permissions.Validation;

public static class SourceRuleTestCases
{
    public static IEnumerable<TestCaseData> DirectiveValidationTestCases
    {
        get
        {
            yield return new TestCaseData(null, false);
            yield return new TestCaseData(Array.Empty<string>(), false);
            yield return new TestCaseData(new[] { CspConstants.Directives.ScriptSource }, true);
            yield return new TestCaseData(new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement }, true);
            yield return new TestCaseData(new[] { CspConstants.Directives.ScriptSourceElement }, true);
            yield return new TestCaseData(new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.StyleSourceElement }, false);
            yield return new TestCaseData(new[] { CspConstants.Directives.StyleSourceElement }, false);
        }
    }
}