namespace Stott.Security.Optimizely.Features.Reporting;

using System;
using System.Collections.Generic;
using System.Linq;

using Stott.Security.Optimizely.Common;

public sealed class ViolationReportSummary
{
    public int Key { get; set; }

    public string? Source { get; set; }

    public string? SanitizedSource
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Source))
            {
                return string.Empty;
            }

            if (CspConstants.AllSources.Contains(Source))
            {
                return Source;
            }

            if (Uri.IsWellFormedUriString(Source, UriKind.Absolute))
            {
                return new Uri(Source).GetLeftPart(UriPartial.Authority);
            }

            return Source;
        }
    }

    public string? Directive { get; set; }

    public int Violations { get; set; }

    public DateTime LastViolated { get; set; }

    public IEnumerable<string> GetDomainSuggestions()
    {
        if (string.IsNullOrWhiteSpace(Source))
        {
            yield break;
        }

        if (Uri.IsWellFormedUriString(Source, UriKind.Absolute))
        {
            var domain = new Uri(Source).GetLeftPart(UriPartial.Authority).ToLowerInvariant();

            yield return domain;

            var components = domain.Split(new[] { '.', ':' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
            components.Remove("http");
            components.Remove("https");

            if (components.Count > 2)
            {
                yield return $"https://*.{string.Join('.', components.Skip(1))}";
            }

            if (components.Count > 3)
            {
                yield return $"https://*.{string.Join('.', components.Skip(2))}";
            }
        }
        else
        {
            yield return Source;
        }
    }
}