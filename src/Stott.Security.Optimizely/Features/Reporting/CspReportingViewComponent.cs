namespace Stott.Security.Optimizely.Features.Reporting;

using System;

using Microsoft.AspNetCore.Mvc;

[Obsolete("Reporting view component has been deprecated in favour of reporturi and reportto directives.")]
public sealed class CspReportingViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return Content(string.Empty);
    }
}