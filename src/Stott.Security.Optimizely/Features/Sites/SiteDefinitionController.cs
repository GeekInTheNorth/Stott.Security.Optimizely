using System.Linq;

using EPiServer.Web;

using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Sites;

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
                                       AvailableHosts = x.Hosts.ToHostSummaries().ToList()
                                   })
                                   .ToList();

        return CreateSuccessJson(sites);
    }
}
