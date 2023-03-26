namespace Stott.Security.Optimizely.Features.Reporting;

using System;
using System.Linq;

using Stott.Security.Optimizely.Common;

public sealed class ReportModel
{
    private string? _effectiveDirective;

    private string? _violatedDirective;

    public string? BlockedUri { get; set; }

    public string? Disposition { get; set; }

    public string? DocumentUri { get; set; }

    public string? EffectiveDirective
    {
        get => GetViolatedDirective(_effectiveDirective);
        set => _effectiveDirective = value;
    }

    public string? OriginalPolicy { get; set; }

    public string? Referrer { get; set; }

    public string? ScriptSample { get; set; }

    public string? SourceFile { get; set; }

    public string? ViolatedDirective 
    {
        get => GetViolatedDirective(_violatedDirective);
        set => _violatedDirective = value;
    }

    private static string? GetViolatedDirective(string? directive)
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