using Stott.Security.Optimizely.Features.CustomHeaders;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class CustomHeaderModel
{
    public string? HeaderName { get; set; }

    public CustomHeaderBehavior Behavior { get; set; }

    public string? HeaderValue { get; set; }
}
