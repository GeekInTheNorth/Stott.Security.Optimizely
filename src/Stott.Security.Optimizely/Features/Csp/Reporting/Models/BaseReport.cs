namespace Stott.Security.Optimizely.Features.Csp.Reporting.Models;

using System;
using System.Linq;

using Stott.Security.Optimizely.Common;

public class BaseReport
{
    protected static string? GetViolatedDirective(string? directive)
    {
        if (string.IsNullOrWhiteSpace(directive))
        {
            return null;
        }

        return CspConstants.AllDirectives
                           .FirstOrDefault(x => string.Equals(x, directive, StringComparison.OrdinalIgnoreCase)) ??
               CspConstants.AllDirectives
                           .Where(x => directive.StartsWith(x, StringComparison.OrdinalIgnoreCase))
                           .OrderByDescending(x => x.Length)
                           .FirstOrDefault();
    }
}