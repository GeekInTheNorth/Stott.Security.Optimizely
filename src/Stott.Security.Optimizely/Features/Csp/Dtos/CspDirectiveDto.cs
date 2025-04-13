using System.Collections.Generic;
using System.Linq;

namespace Stott.Security.Optimizely.Features.Csp.Dtos;

internal sealed class CspDirectiveDto
{
    public string Directive { get; }

    public IList<string> Sources { get; }

    public int PredictedSize { get; }

    public CspDirectiveDto(string directive, IList<string> sources)
    {
        Directive = directive;
        Sources = sources.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        PredictedSize = Directive.Length + 3 + Sources.Sum(s => s.Length + 1);
    }

    public CspDirectiveDto(string directive, string source)
        : this(directive, new List<string> { source })
    {
    }

    public override string ToString()
    {
        return $"{Directive} {string.Join(" ", Sources)}; ";
    }
}
