namespace Stott.Security.Optimizely.Features.CustomHeaders;

/// <summary>
/// Defines the behavior for a custom header.
/// </summary>
public enum CustomHeaderBehavior
{
    /// <summary>
    /// Do not perform any action for this header
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Add the header to the HTTP response with the specified value.
    /// </summary>
    Add = 1,

    /// <summary>
    /// Remove the header from the HTTP response.
    /// </summary>
    Remove = 2
}
