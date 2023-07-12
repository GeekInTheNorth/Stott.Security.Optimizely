namespace Stott.Security.Optimizely.Features.Cors;

using System.Collections.Generic;

public sealed class CorsConfiguration
{
    public bool IsEnabled { get; set; }

    public CorsConfigurationMethods AllowMethods { get; set; } = new();

    public List<CorsConfigurationItem> AllowOrigins { get; set; } = new();

    public List<CorsConfigurationItem> AllowHeaders { get; set; } = new();

    public List<CorsConfigurationItem> ExposeHeaders { get; set; } = new();

    public bool AllowCredentials { get; set; }

    public int MaxAge { get; set; }
}