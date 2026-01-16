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
            yield return new TestCaseData(CspConstants.Sources.UnsafeHashes, new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSourceAttribute, CspConstants.Directives.StyleSource, CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSourceAttribute });
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

    public static IEnumerable<TestCaseData> DirectiveHashValidationTestCases
    {
        get
        {
            yield return new TestCaseData("'sha256-Sx4jYpKXGZQ5yJZ7+ZJ6PpE0vYQkFZJpGv6zKcP9nC8='", new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSourceAttribute, CspConstants.Directives.StyleSource, CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSourceAttribute });
            yield return new TestCaseData("'sha384-3m1p5T0plbYtOVuJ2y1DxDj9wJ0xWnQy+T7qZtL9cM8OVz2G2Qjhg6Cjq7UQjL4u'", new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSourceAttribute, CspConstants.Directives.StyleSource, CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSourceAttribute });
            yield return new TestCaseData("'sha512-Rybpl2uZ+0IhGfQHC2MZfE+P2qZuw5NZ9JzM9fHqTfgdp5eR3q2UO4FnGQ9tHXaBA3f2YgJfSyk0xqkQsZg2Pfgg=='", new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.ScriptSourceAttribute, CspConstants.Directives.StyleSource, CspConstants.Directives.StyleSourceElement, CspConstants.Directives.StyleSourceAttribute });
        }
    }
}