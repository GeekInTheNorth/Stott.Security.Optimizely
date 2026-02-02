using System;
using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Configuration;

public sealed class SecuritySetupOptions
{
    public string? ConnectionStringName { get; set; }

    public List<string> NonceHashExclusionPaths { get; set; } = new List<string>() { "/episerver", "/ui", "/util", "/stott.robotshandler", "/stott.security.optimizely" };

    public TimeSpan AuditRetentionPeriod { get; set; } = TimeSpan.FromDays(730); // 2 years
}