using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Stott.Security.Optimizely.Features.LandingPage
{
    [Authorize(Roles = "CmsAdmin,WebAdmins,Administrators")]
    public class CspLandingPageController : Controller
    {
        [HttpGet]
        [Route("[controller]/[action]")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
