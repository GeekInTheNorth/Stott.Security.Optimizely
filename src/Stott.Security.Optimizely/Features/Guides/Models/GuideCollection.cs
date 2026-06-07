namespace Stott.Security.Optimizely.Features.Guides.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class GuideCollection
{
    [JsonPropertyName("cms13")]
    public string? CurrentVersion { get; set; }

    [JsonPropertyName("articles")]
    public List<GuideModel> Guides { get; set; } = [];
}