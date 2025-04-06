namespace Stott.Security.Optimizely.Features.Csp.Reporting.Models;

using Newtonsoft.Json;

public sealed class ReportUriBody : BaseReport, ICspReport
{
    private string? _effectiveDirective;

    private string? _violatedDirective;

    [JsonProperty("blocked-uri")]
    public string? BlockedUri { get; set; }

    [JsonProperty("disposition")]
    public string? Disposition { get; set; }

    [JsonProperty("document-uri")]
    public string? DocumentUri { get; set; }

    [JsonProperty("effective-directive")]
    public string? EffectiveDirective
    {
        get => GetViolatedDirective(_effectiveDirective);
        set => _effectiveDirective = value;
    }

    [JsonProperty("original-policy")]
    public string? OriginalPolicy { get; set; }

    [JsonProperty("referrer")]
    public string? Referrer { get; set; }

    [JsonProperty("status-code")]
    public int StatusCode { get; set; }

    [JsonProperty("source-file")]
    public string? SourceFile { get; set; }

    [JsonProperty("violated-directive")]
    public string? ViolatedDirective
    {
        get => GetViolatedDirective(_violatedDirective);
        set => _violatedDirective = value;
    }
}
