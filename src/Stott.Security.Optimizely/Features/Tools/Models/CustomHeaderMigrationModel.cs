using Stott.Security.Optimizely.Features.CustomHeaders;

namespace Stott.Security.Optimizely.Features.Tools.Models;

public sealed class CustomHeaderMigrationModel
{
    public string? HeaderName { get; set; }

    public CustomHeaderBehavior Behavior { get; set; }

    public string? HeaderValue { get; set; }
}
