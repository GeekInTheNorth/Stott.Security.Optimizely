using System.Collections.Generic;
using Stott.Security.Optimizely.Features.Sites;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

internal static class SecurityTxtHelpers
{
    internal static List<SiteHostViewModel> CreateHostSummaries(string defaultHostName)
    {
        return new List<SiteHostViewModel>
    {
        new SiteHostViewModel
        {
            DisplayName = defaultHostName,
            HostName = string.Empty
        }
    };
    }
}