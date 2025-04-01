namespace OptiNetSix.Features.GeneralContent
{
    using Microsoft.AspNetCore.Mvc;

    using OptiNetSix.Features.Common;
    using OptiNetSix.Features.Security;

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
