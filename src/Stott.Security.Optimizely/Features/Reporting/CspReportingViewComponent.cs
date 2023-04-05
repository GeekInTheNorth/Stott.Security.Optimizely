namespace Stott.Security.Optimizely.Features.Reporting;

using Microsoft.AspNetCore.Mvc;

using Stott.Security.Optimizely.Features.StaticFile;

public sealed class CspReportingViewComponent : ViewComponent
{
    private readonly IStaticFileResolver _staticFileResolver;

    public CspReportingViewComponent(IStaticFileResolver staticFileResolver)
    {
        _staticFileResolver = staticFileResolver;
    }

    public IViewComponentResult Invoke()
    {
        var model = new CspReportingViewModel
        {
            JavaScriptPath = $"/stott.security.optimizely/static/{_staticFileResolver.GetReportingScriptFileName()}"
        };

        return View(model);
    }
}