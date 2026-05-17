using System;
using System.Linq;

using EPiServer.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Sites;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class SiteDefinitionController : BaseController
{
    private readonly ISiteDefinitionRepository _siteRepository;

    public SiteDefinitionController(ISiteDefinitionRepository siteRepository)
    {
        _siteRepository = siteRepository;
    }

    [HttpGet]
    [Route("/stott.security.optimizely/api/[action]")]
    public IActionResult Sites()
    {
        var sites = _siteRepository.List()
                                   .Select(x => new SiteViewModel
                                   {
                                       SiteId = x.Id,
                                       SiteName = x.Name,
                                       AvailableHosts = x.Hosts.ToHostSummaries().ToList(),
                                       HasMultipleHosts = x.Hosts.HasMultipleHosts()
                                   })
                                   .ToList();

        // Prepend the Global sentinel (Guid.Empty = "All Sites") so the UI can render the root context option.
        sites.Insert(0, new SiteViewModel
        {
            SiteId = Guid.Empty,
            SiteName = "All Sites",
            AvailableHosts = new System.Collections.Generic.List<SiteHostViewModel>(0),
            HasMultipleHosts = false
        });

        return CreateSuccessJson(sites);
    }
}
