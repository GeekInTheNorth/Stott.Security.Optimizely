namespace Stott.Security.Optimizely.Features.Reporting.Models;

using Newtonsoft.Json;

public sealed class ReportToBody : BaseReport, ICspReport
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
}
