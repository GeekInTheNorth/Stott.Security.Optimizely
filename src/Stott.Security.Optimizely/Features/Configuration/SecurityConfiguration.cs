using System;
using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Configuration;

public sealed class SecurityConfiguration
{
    public IList<string> ExclusionPaths { get; set; } = new List<string>();

    public TimeSpan AuditRetentionPeriod { get; set; } = TimeSpan.FromDays(730); // 2 years
}