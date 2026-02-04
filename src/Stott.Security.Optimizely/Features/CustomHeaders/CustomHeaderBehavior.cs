namespace Stott.Security.Optimizely.Features.CustomHeaders;

/// <summary>
/// Defines the behavior for a custom header.
/// </summary>
public enum CustomHeaderBehavior
{
    /// <summary>
    /// Add the header to the HTTP response with the specified value.
    /// </summary>
    Add = 0,

    /// <summary>
    /// Remove the header from the HTTP response.
    /// </summary>
    Remove = 1
}
