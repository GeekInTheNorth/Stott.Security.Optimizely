using Stott.Security.Optimizely.Features.Cors;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class SettingsModel
{
    public CspSettingsModel? Csp { get; set; }

    public CorsConfiguration? Cors { get; set; }

    public SecurityHeadersModel? Headers { get; set; }
}