namespace Stott.Security.Optimizely.Features.Header;

public interface ICspSourceMapping
{
    string? Source { get; }

    string? Directives { get; }
}