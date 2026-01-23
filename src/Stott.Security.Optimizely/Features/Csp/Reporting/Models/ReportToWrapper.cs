using System.Text.Json.Serialization;

namespace Stott.Security.Optimizely.Features.Csp.Reporting.Models;

public sealed class ReportToWrapper
{
    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("body")]
    public ReportToBody CspReport { get; set; } = new();

    [JsonPropertyName("type")]
    public string? ReportType { get; set; }

    [JsonPropertyName("user_agent")]
    public string? UserAgent { get; set; }
}
