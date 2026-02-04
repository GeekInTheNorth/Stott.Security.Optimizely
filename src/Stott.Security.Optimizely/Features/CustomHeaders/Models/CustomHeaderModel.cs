namespace Stott.Security.Optimizely.Features.CustomHeaders.Models;

using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Gets or sets a description about what the header is used for.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a list of allowed values for the header.
    /// </summary>
    public IList<CustomHeaderAllowedValue>? AllowedValues { get; set; }

    /// <summary>
    /// Indicates which interface to use for editing this header.
    /// </summary>
    public string? PropertyType { get; set; }

    /// <summary>
    /// Indicates whether the header name can be edited. This is false for fixed headers to prevent renaming them, which could lead to misconfiguration.
    /// </summary>
    public bool IsHeaderNameEditable { get; set; }
}
