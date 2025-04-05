namespace OptiNetSix.Features.NotFound;

using EPiServer;

using Microsoft.AspNetCore.Mvc;

using OptiNetSix.Features.Settings;

public sealed class ErrorController : Controller
{
    private readonly IContentLoader _contentLoader;

    private readonly ISiteSettings _siteSettings;

    public ErrorController(IContentLoader contentLoader, ISiteSettings siteSettings)
    {
        _contentLoader = contentLoader;
        _siteSettings = siteSettings;
    }

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
        if (_contentLoader.TryGet<NotFoundPage>(_siteSettings.NotFoundPage, out var notFoundPage))
        {
            return notFoundPage;
        }

        return default;
    }
}