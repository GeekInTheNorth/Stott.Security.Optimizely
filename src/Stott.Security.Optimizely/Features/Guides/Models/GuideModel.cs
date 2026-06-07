namespace Stott.Security.Optimizely.Features.Guides.Models;

using System;

/// <summary>
/// Represents a single supporting article ("Guide") sourced from the remote Stott Security feed.
/// </summary>
public sealed class GuideModel
{
    public string Title { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset Date { get; set; }
}
