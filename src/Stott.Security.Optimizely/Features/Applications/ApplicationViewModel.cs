using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Applications;

public sealed class ApplicationViewModel
{
    public string? AppId { get; set; }

    public string? AppName { get; set; }

    public List<HostViewModel>? AvailableHosts { get; set; }
}