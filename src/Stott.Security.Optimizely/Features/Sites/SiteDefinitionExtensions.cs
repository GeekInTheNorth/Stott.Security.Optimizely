using System.Collections.Generic;
using System.Linq;

using EPiServer.Web;

namespace Stott.Security.Optimizely.Features.Sites;

public static class SiteDefinitionExtensions
{
    public static IEnumerable<SiteHostViewModel> ToHostSummaries(this IList<HostDefinition> hostDefinitions)
    {
        yield return new SiteHostViewModel
        {
            DisplayName = "Default",
            HostName = string.Empty,
            HostType = "Undefined",
            HostLanguage = "-"
        };
        if (hostDefinitions is not { Count: > 0 })
        {
            yield break;
        }

        foreach (var host in hostDefinitions.Where(x => x.Url is not null))
        {
            yield return new SiteHostViewModel
            {
                DisplayName = host.Name,
                HostName = host.Name,
                HostType = host.Type.ToString(),
                HostLanguage = host.Language?.DisplayName ?? host.Language?.Name ?? "-"
            };
        }
    }

    /// <summary>
    /// True when the site exposes more than one non-wildcard host binding (excluding the implicit Default).
    /// Used by the context switcher UI to decide whether to render a host drill-down.
    /// </summary>
    public static bool HasMultipleHosts(this IList<HostDefinition> hostDefinitions)
    {
        return hostDefinitions is { Count: > 0 }
            && hostDefinitions.Count(x => x.Url is not null) > 1;
    }
}
