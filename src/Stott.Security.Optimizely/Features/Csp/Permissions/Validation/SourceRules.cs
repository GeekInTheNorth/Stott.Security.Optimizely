using System;
using System.Collections.Generic;
using System.Linq;
using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Csp.Permissions.Validation;

internal static class SourceRules
{
    public static SourceRule GetRuleForSource(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return new SourceRule
            {
                Source = string.Empty,
                ValidDirectives = CspConstants.AllDirectives.ToArray()
            };
        }

        var specificSource = GetExactSourceRules().FirstOrDefault(r => r.Source.Equals(source, StringComparison.OrdinalIgnoreCase)) ??
                             GetHashSourceRules().FirstOrDefault(r => source.StartsWith(r.Source, StringComparison.OrdinalIgnoreCase));

        return specificSource ?? new SourceRule
        {
            Source = source ?? string.Empty,
            ValidDirectives = CspConstants.AllDirectives.ToArray()
        };
    }

    private static IEnumerable<SourceRule> GetExactSourceRules()
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
                CspConstants.Directives.ScriptSourceElement,
                CspConstants.Directives.ScriptSourceAttribute,
                CspConstants.Directives.StyleSource,
                CspConstants.Directives.StyleSourceElement,
                CspConstants.Directives.StyleSourceAttribute
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
        yield return new SourceRule
        {
            Source = CspConstants.Sources.Nonce,
            ValidDirectives = new[]
            {
                CspConstants.Directives.ScriptSource,
                CspConstants.Directives.ScriptSourceElement,
                CspConstants.Directives.StyleSource,
                CspConstants.Directives.StyleSourceElement
            }
        };
        yield return new SourceRule
        {
            Source = CspConstants.Sources.StrictDynamic,
            ValidDirectives = new[]
            {
                CspConstants.Directives.ScriptSource,
                CspConstants.Directives.ScriptSourceElement
            }
        };
    }

    private static IEnumerable<SourceRule> GetHashSourceRules()
    {
        yield return new SourceRule
        {
            Source = "'sha256-",
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
        yield return new SourceRule
        {
            Source = "'sha384-",
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
        yield return new SourceRule
        {
            Source = "'sha512-",
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