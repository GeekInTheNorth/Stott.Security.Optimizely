namespace Stott.Security.Optimizely.Features.Cors;

using System;

public sealed class CorsConfigurationItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Value { get; set; } = string.Empty;
}