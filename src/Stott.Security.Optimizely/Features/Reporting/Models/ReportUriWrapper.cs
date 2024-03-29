﻿namespace Stott.Security.Optimizely.Features.Reporting.Models;

using Newtonsoft.Json;

public sealed class ReportUriWrapper
{
    [JsonProperty("csp-report")]
    public ReportUriBody CspReport { get; set; } = new();
}
