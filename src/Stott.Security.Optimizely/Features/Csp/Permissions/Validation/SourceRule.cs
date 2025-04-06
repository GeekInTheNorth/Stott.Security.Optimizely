using System;
using System.Collections.Generic;
using System.Linq;

namespace Stott.Security.Optimizely.Features.Csp.Permissions.Validation;

internal sealed class SourceRule
{
    public string Source { get; set; } = string.Empty;

    public string[] ValidDirectives { get; set; } = Array.Empty<string>();

    public string ErrorTemplate => $"{Source} is only valid with {string.Join(',', ValidDirectives)}.";

    public bool IsValid(IList<string>? directives)
    {
        if (directives is not { Count: > 0 })
        {
            return false;
        }

        var nonMatchingDirectives = directives.Where(x => !ValidDirectives.Contains(x, StringComparer.OrdinalIgnoreCase)).ToList();

        return nonMatchingDirectives.Count == 0;
    }

    public bool IsValid(string? directive)
    {
        if (string.IsNullOrWhiteSpace(directive))
        {
            return false;
        }

        return ValidDirectives.Contains(directive, StringComparer.OrdinalIgnoreCase);
    }
}