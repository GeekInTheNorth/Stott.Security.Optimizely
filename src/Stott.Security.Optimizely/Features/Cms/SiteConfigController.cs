using System.Collections.Generic;
using System.Linq;
using EPiServer.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stott.Security.Optimizely.Common;

namespace Stott.Security.Optimizely.Features.Cms;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
[Route("/stott.security.optimizely/api/cms/sites/[action]")]
public sealed class SiteConfigController : BaseController
{
    private readonly ISiteDefinitionRepository _siteDefinitionRepository;

    public SiteConfigController(ISiteDefinitionRepository siteDefinitionRepository)
    {
        _siteDefinitionRepository = siteDefinitionRepository;
    }

    [HttpGet]
    public IActionResult List()
    {
        var rawSites = _siteDefinitionRepository.List();

        return CreateSuccessJson(Map(rawSites));
    }

    private static IEnumerable<SiteConfigModel> Map(IEnumerable<SiteDefinition> siteDefinitions)
    {
        if (siteDefinitions is null)
        {
            yield break;
        }

        foreach (var siteDefinition in siteDefinitions)
        {
            yield return new SiteConfigModel
            {
                Id = siteDefinition.Id,
                Name = siteDefinition.Name,
                Hosts = MapHosts(siteDefinition.Hosts).ToList()
            };
        }
    }

    private static IEnumerable<SiteHostModel> MapHosts(IEnumerable<HostDefinition> hosts)
    {
        if (hosts is null)
        {
            yield break;
        }

        foreach (var host in hosts)
        {
            yield return new SiteHostModel
            {
                DisplayName = host.Name,
                HostName = host.Url.ToString()
            };
        }
    }
}