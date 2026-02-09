namespace Stott.Security.Optimizely.Features.Header;

public sealed class HeaderDto
{
    /// <summary>
    /// Gets or sets the unique identifier associated with the object.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the string value associated with this instance.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this header should be removed from the response.
    /// When true, the middleware will call context.Response.Headers.Remove(Key).
    /// </summary>
    public bool IsRemoval { get; set; }
}