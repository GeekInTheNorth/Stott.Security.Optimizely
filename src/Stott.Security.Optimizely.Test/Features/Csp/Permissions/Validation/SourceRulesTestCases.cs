using System.Collections.Generic;

using NUnit.Framework;
using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.Validation;

public static class SourceRulesTestCases
{
    public static IEnumerable<TestCaseData> DirectiveValidationTestCases
    {
        get
        {
            yield return new TestCaseData(CspConstants.Sources.InlineSpeculationRules, new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement });
            yield return new TestCaseData(CspConstants.Sources.UnsafeEval, new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSourceAttribute, CspConstants.Directives.WorkerSource });
            yield return new TestCaseData(CspConstants.Sources.WebAssemblyUnsafeEval, new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSourceAttribute, CspConstants.Directives.WorkerSource });
            yield return new TestCaseData(CspConstants.Sources.UnsafeHashes, new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement });
            yield return new TestCaseData(CspConstants.Sources.UnsafeInline, new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSourceAttribute, CspConstants.Directives.StyleSource, CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSourceAttribute });
            yield return new TestCaseData(CspConstants.Sources.Self, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData(CspConstants.Sources.None, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData(CspConstants.Sources.SchemeBlob, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData(CspConstants.Sources.SchemeData, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData(CspConstants.Sources.SchemeFileSystem, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData(CspConstants.Sources.SchemeHttp, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData(CspConstants.Sources.SchemeHttps, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData(CspConstants.Sources.SchemeWs, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData(CspConstants.Sources.SchemeWss, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData(CspConstants.Sources.SchemeMediaStream, CspConstants.AllDirectives.ToArray());
            yield return new TestCaseData("https://www.example.com", CspConstants.AllDirectives.ToArray());
        }
    }
}