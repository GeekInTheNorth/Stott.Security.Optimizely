namespace Stott.Security.Optimizely.Features.Csp;

public interface ICspSourceMapping
{
    string? Source { get; }

    string? Directives { get; }
}