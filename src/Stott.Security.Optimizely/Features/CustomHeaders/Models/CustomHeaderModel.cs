namespace Stott.Security.Optimizely.Features.CustomHeaders.Models;

using System;
using Stott.Security.Optimizely.Features.CustomHeaders;

/// <summary>
/// Represents a custom header for display and data transfer.
/// </summary>
public sealed class CustomHeaderModel : ICustomHeader
{
    /// <summary>
    /// Gets or sets the unique identifier for the custom header.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the HTTP header.
    /// </summary>
    public string HeaderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the behavior for the header (Add or Remove).
    /// </summary>
    public CustomHeaderBehavior Behavior { get; set; }

    /// <summary>
    /// Gets or sets the value of the header (required when Behavior is Add).
    /// </summary>
    public string? HeaderValue { get; set; }
}
