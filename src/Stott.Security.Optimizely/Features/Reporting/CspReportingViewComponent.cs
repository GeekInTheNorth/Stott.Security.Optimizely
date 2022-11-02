using Microsoft.AspNetCore.Mvc;

namespace Stott.Security.Optimizely.Features.Reporting
{
    public class CspReportingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
