using System.Collections.Generic;
using System.Linq;

namespace Stott.Security.Optimizely.Features.Applications;

public sealed class ApplicationViewModel
{
    public string? AppId { get; set; }

    public string? AppName { get; set; }

    public List<HostViewModel>? AvailableHosts { get; set; }

    public bool HasMultipleHosts => AvailableHosts?.Count(x => !string.IsNullOrWhiteSpace(x.HostName)) > 1;
}