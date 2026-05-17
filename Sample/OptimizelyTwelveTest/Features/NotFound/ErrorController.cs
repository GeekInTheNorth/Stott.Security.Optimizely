namespace OptimizelyTwelveTest.Features.NotFound;

using EPiServer;

using Microsoft.AspNetCore.Mvc;

using OptimizelyTwelveTest.Features.Settings;

public sealed class ErrorController(IContentLoader contentLoader, ISiteSettingsResolver siteSettingsResolver) : Controller
{
    [HttpGet]
    [Route("/error")]
    public IActionResult PageNotFound(int statusCode)
    {
        var notFoundPage = GetNotFoundPage();
        var model = new NotFoundPageViewModel() { CurrentPage = notFoundPage, StatusCode = statusCode };

        return View(model);
    }

    private NotFoundPage GetNotFoundPage()
    {
        var siteSettings = siteSettingsResolver.Get();
        if (contentLoader.TryGet<NotFoundPage>(siteSettings?.NotFoundPage, out var notFoundPage))
        {
            return notFoundPage;
        }

        return default;
    }
}