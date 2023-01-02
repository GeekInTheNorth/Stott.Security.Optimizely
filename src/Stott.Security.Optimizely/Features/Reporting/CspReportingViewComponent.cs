namespace Stott.Security.Optimizely.Features.Reporting;

using Microsoft.AspNetCore.Mvc;

public class CspReportingViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}