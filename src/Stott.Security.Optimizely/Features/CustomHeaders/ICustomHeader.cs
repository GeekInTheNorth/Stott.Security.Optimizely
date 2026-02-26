using System;

namespace Stott.Security.Optimizely.Features.CustomHeaders;

public interface ICustomHeader
{
    /// <summary>
    /// Gets or sets the unique identifier for the custom header.
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the HTTP header.
    /// </summary>
    string HeaderName { get; set; }

    /// <summary>
    /// Gets or sets the behavior for the header (Add or Remove).
    /// </summary>
    CustomHeaderBehavior Behavior { get; set; }

    /// <summary>
    /// Gets or sets the value of the header (required when Behavior is Add).
    /// </summary>
    string? HeaderValue { get; set; }
}
