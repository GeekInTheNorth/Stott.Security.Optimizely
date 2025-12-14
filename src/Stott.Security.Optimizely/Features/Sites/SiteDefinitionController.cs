using System;
using System.Linq;

using EPiServer.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.SecurityTxt;

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
                                       AvailableHosts = x.Hosts.ToHostSummaries().ToList()
                                   })
                                   .ToList();
        sites.Insert(0, new SiteViewModel
        {
            SiteId = Guid.Empty,
            SiteName = "All Sites",
            AvailableHosts = SecurityTxtHelpers.CreateHostSummaries("All Hosts")
        });

        return CreateSuccessJson(sites);
    }
}
