using Microsoft.AspNetCore.Mvc;

namespace Stott.Security.Core.Features.Reporting
{
    public class CspReportingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
