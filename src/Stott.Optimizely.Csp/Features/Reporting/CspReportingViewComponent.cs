using Microsoft.AspNetCore.Mvc;

namespace Stott.Optimizely.Csp.Features.Reporting
{
    public class CspReportingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
