namespace Stott.Security.Optimizely.Features.Csp.Reporting.Models;

using System;
using System.Linq;
using System.Text.Json.Serialization;
using Stott.Security.Optimizely.Common;

public sealed class ReportToBody : ICspReport
{
    private string? _effectiveDirective;

    [JsonPropertyName("blockedURL")]
    public string? BlockedUri { get; set; }

    [JsonPropertyName("disposition")]
    public string? Disposition { get; set; }

    [JsonPropertyName("documentURL")]
    public string? DocumentUri { get; set; }

    [JsonPropertyName("effectiveDirective")]
    public string? EffectiveDirective
    {
        get => GetViolatedDirective(_effectiveDirective);
        set => _effectiveDirective = value;
    }

    [JsonPropertyName("originalPolicy")]
    public string? OriginalPolicy { get; set; }

    [JsonPropertyName("referrer")]
    public string? Referrer { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("sourceFile")]
    public string? SourceFile { get; set; }

    [JsonIgnore]
    public string? ViolatedDirective => EffectiveDirective;

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
