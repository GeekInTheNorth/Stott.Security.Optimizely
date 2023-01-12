namespace Stott.Security.Optimizely.Features.Reporting;

using Microsoft.AspNetCore.Mvc;

public sealed class CspReportingViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}