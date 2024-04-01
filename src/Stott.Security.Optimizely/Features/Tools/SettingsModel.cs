using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.SecurityHeaders;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class SettingsModel
{
    public CspSettingsModel? Csp { get; set; }

    public CorsConfiguration? Cors { get; set; }

    public SecurityHeaderModel? Headers { get; set; }
}