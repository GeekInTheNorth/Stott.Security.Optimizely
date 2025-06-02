namespace Stott.Security.Optimizely.Features.Csp.Reporting;

using System;
using System.Collections.Generic;
using System.Linq;

using Stott.Security.Optimizely.Common;

public sealed class ViolationReportSummary
{
    public Guid Key { get; }

    public string Source { get; }

    public string SanitizedSource { get; }

    public IList<string> SourceSuggestions { get; }

    public string Directive { get; set; }

    public IList<string> DirectiveSuggestions { get; }

    public int Violations { get; set; }

    public DateTime LastViolated { get; set; }

    public ViolationReportSummary(
        Guid key,
        string? source,
        string? directive,
        int violations,
        DateTime lastViolated)
    {
        Key = key;
        Source = source ?? string.Empty;
        Directive = directive ?? string.Empty;
        Violations = violations;
        LastViolated = lastViolated;

        SanitizedSource = GetSanitizedSource(source);
        SourceSuggestions = GetSourceSuggestions(SanitizedSource).ToList();
        DirectiveSuggestions = GetDirectiveSuggestions(Directive).ToList();
    }

    private static IEnumerable<string> GetSourceSuggestions(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            yield break;
        }

        if (CspConstants.AllSources.Contains(source))
        {
            yield return source;
            yield break;
        }

        if (Uri.IsWellFormedUriString(source, UriKind.Absolute))
        {
            var uri = new Uri(source);
            var domain = uri.GetLeftPart(UriPartial.Authority).ToLowerInvariant();
            var scheme = uri.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();

            yield return domain;

            var components = domain.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            components.Remove(scheme);

            if (components.Count > 2)
            {
                yield return $"{scheme}*.{string.Join('.', components.Skip(1))}";
            }

            if (components.Count > 3)
            {
                yield return $"{scheme}*.{string.Join('.', components.Skip(2))}";
            }
        }
        else
        {
            yield return source;
        }
    }

    private static string GetSanitizedSource(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return string.Empty;
        }

        if (CspConstants.AllSources.Contains(source))
        {
            return source;
        }

        if (Uri.IsWellFormedUriString(source, UriKind.Absolute))
        {
            return new Uri(source).GetLeftPart(UriPartial.Authority);
        }

        return source ?? string.Empty;
    }

    private static IEnumerable<string> GetDirectiveSuggestions(string directive)
    {
        if (string.IsNullOrWhiteSpace(directive))
        {
            yield break;
        }

        switch (directive)
        {
            // In CSP 3: child-src is deprecated in favor of frame-src
            case CspConstants.Directives.ChildSource:
            case CspConstants.Directives.FrameSource:
                yield return CspConstants.Directives.FrameSource;
                yield return CspConstants.Directives.ChildSource;
                break;

            case CspConstants.Directives.ScriptSourceAttribute:
            case CspConstants.Directives.ScriptSourceElement:
                yield return directive;
                yield return CspConstants.Directives.ScriptSource;
                break;

            case CspConstants.Directives.StyleSourceAttribute:
            case CspConstants.Directives.StyleSourceElement:
                yield return directive;
                yield return CspConstants.Directives.StyleSource;
                break;

            default:
                yield return directive;
                break;
        }
    }
}