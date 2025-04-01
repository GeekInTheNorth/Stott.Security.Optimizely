namespace OptiNetNine.Features.GeneralContent
{
    using Microsoft.AspNetCore.Mvc;

    using OptiNetNine.Features.Common;
    using OptiNetNine.Features.Security;

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
