namespace Stott.Security.Optimizely.Features.Csp.Reporting.Models;

using System;
using System.Linq;
using Newtonsoft.Json;
using Stott.Security.Optimizely.Common;

public sealed class ReportToBody : ICspReport
{
    private string? _effectiveDirective;

    [JsonProperty("blockedURL")]
    public string? BlockedUri { get; set; }

    [JsonProperty("disposition")]
    public string? Disposition { get; set; }

    [JsonProperty("documentURL")]
    public string? DocumentUri { get; set; }

    [JsonProperty("effectiveDirective")]
    public string? EffectiveDirective
    {
        get => GetViolatedDirective(_effectiveDirective);
        set => _effectiveDirective = value;
    }

    [JsonProperty("originalPolicy")]
    public string? OriginalPolicy { get; set; }

    [JsonProperty("referrer")]
    public string? Referrer { get; set; }

    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }

    [JsonProperty("sourceFile")]
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
