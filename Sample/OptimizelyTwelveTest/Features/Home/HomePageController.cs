namespace OptimizelyTwelveTest.Features.Home
{
    using Microsoft.AspNetCore.Mvc;

    using OptimizelyTwelveTest.Features.Common;
    using OptimizelyTwelveTest.Features.Security;


    public class HomePageController : PageControllerBase<HomePage>
    {
        [SecurityHeaderAction]
        public IActionResult Index(HomePage currentPage)
        {
            var model = new HomePageViewModel { CurrentPage = currentPage };

            return View(model);
        }
    }
}
