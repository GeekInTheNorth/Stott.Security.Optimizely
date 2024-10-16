using System.Collections.Generic;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Permissions.Validation;

internal static class SourceRules
{
    public static IEnumerable<SourceRule> GetSourceRules()
    {
        yield return new SourceRule
        {
            Source = CspConstants.Sources.InlineSpeculationRules,
            ValidDirectives = new[]
            {
                CspConstants.Directives.ScriptSource,
                CspConstants.Directives.ScriptSourceElement
            }
        };
        yield return new SourceRule
        {
            Source = CspConstants.Sources.UnsafeEval,
            ValidDirectives = new[]
            {
                CspConstants.Directives.ScriptSource,
                CspConstants.Directives.ScriptSourceElement,
                CspConstants.Directives.ScriptSourceAttribute,
                CspConstants.Directives.WorkerSource
            }
        };
        yield return new SourceRule
        {
            Source = CspConstants.Sources.WebAssemblyUnsafeEval,
            ValidDirectives = new[]
            {
                CspConstants.Directives.ScriptSource,
                CspConstants.Directives.ScriptSourceElement,
                CspConstants.Directives.ScriptSourceAttribute,
                CspConstants.Directives.WorkerSource
            }
        };
        yield return new SourceRule
        {
            Source = CspConstants.Sources.UnsafeHashes,
            ValidDirectives = new[]
            {
                CspConstants.Directives.ScriptSource,
                CspConstants.Directives.ScriptSourceElement
            }
        };
        yield return new SourceRule
        {
            Source = CspConstants.Sources.UnsafeInline,
            ValidDirectives = new[]
            {
                CspConstants.Directives.ScriptSource,
                CspConstants.Directives.ScriptSourceElement,
                CspConstants.Directives.ScriptSourceAttribute,
                CspConstants.Directives.StyleSource,
                CspConstants.Directives.StyleSourceElement,
                CspConstants.Directives.StyleSourceAttribute
            }
        };
    }
}