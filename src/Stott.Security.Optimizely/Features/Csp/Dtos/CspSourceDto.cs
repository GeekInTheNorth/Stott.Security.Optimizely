using System;
using System.Collections.Generic;
using System.Linq;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Csp.Dtos;

internal sealed class CspSourceDto
{
    public string Source { get; }

    public List<string> Directives { get; }

    public CspSourceDto(string? source, string? directives)
    {
        Source = source ?? string.Empty;

        if (string.IsNullOrWhiteSpace(directives))
        {
            Directives = new List<string>(0);
        }
        else
        {
            var splitDirectives = directives.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            Directives = CspConstants.AllDirectives
                          .Where(x => splitDirectives.Contains(x, StringComparer.OrdinalIgnoreCase))
                          .ToList();
        }
    }
}