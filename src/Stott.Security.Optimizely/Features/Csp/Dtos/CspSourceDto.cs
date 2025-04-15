using System;
using System.Collections.Generic;
using System.Linq;

namespace Stott.Security.Optimizely.Features.Csp.Dtos;

internal sealed class CspSourceDto
{
    public string Source { get; }

    public List<string> Directives { get; }

    public CspSourceDto(string? source, string? directives)
    {
        Source = source ?? string.Empty;
        Directives = directives?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Distinct().ToList() ?? new List<string>(0);
    }
}