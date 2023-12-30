namespace Stott.Security.Optimizely.Features.Reporting.Models;

public interface ICspReport
{
    string? BlockedUri { get; }

    string? Disposition { get; }

    string? DocumentUri { get; }

    string? EffectiveDirective { get; }

    string? OriginalPolicy { get; }

    string? Referrer { get; }

    string? SourceFile { get; }

    string? ViolatedDirective { get; }
}