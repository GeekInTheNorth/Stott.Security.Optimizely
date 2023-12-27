namespace Stott.Security.Optimizely.Features.Reporting.Models;

using Newtonsoft.Json;

public sealed class ReportToWrapper
{
    [JsonProperty("age")]
    public int Age { get; set; }

    [JsonProperty("body")]
    public ReportToBody CspReport { get; set; } = new();

    [JsonProperty("type")]
    public string? ReportType { get; set; }

    [JsonProperty("user_agent")]
    public string? UserAgent { get; set; }
}
