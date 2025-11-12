namespace OptimizelyTwelveTest.Features.GeneralContent
{
    using Microsoft.AspNetCore.Mvc;

    using OptimizelyTwelveTest.Features.Common;
    using OptimizelyTwelveTest.Features.Security;

    public class GeneralContentPageController : PageControllerBase<GeneralContentPage>
    {
        [SecurityHeaderAction]
        public IActionResult Index(GeneralContentPage currentContent)
        {
            var model = new GeneralContentPageViewModel { CurrentPage = currentContent };

            return View(model);
        }
    }
}
