using Microsoft.AspNetCore.Mvc;
using OptimizelyTwelveTest.Features.Common;
using OptimizelyTwelveTest.Features.Security;

namespace OptimizelyTwelveTest.Features.Campaign;

public class CampaignPageController : PageControllerBase<CampaignPage>
{
    [SecurityHeaderAction]
    public IActionResult Index(CampaignPage currentContent)
    {
        var model = new CampaignPageViewModel { CurrentPage = currentContent };

        return View(model);
    }
}
