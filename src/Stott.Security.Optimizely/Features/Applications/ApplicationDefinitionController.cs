using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.SecurityTxt;

namespace Stott.Security.Optimizely.Features.Applications;

[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = CspConstants.AuthorizationPolicy)]
public sealed class ApplicationDefinitionController(IApplicationDefinitionService appService) : BaseController
{
    [HttpGet]
    [Route("/stott.security.optimizely/api/[action]")]
    public async Task<IActionResult> Applications()
    {
        var apps = await appService.GetAllApplicationsAsync();
        var allApps = apps.ToList();

        allApps.Insert(0, new ApplicationViewModel
        {
            AppName = "All Sites",
            AvailableHosts = SecurityTxtHelpers.CreateHostSummaries("All Hosts")
        });

        return CreateSuccessJson(allApps);
    }
}