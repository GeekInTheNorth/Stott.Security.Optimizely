using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stott.Security.Core.Common;

namespace Stott.Security.Optimizely.Features.LandingPage
{
    [Authorize(Roles = CspConstants.AuthorizationPolicy)]
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
