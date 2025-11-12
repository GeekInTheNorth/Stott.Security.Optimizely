namespace OptimizelyTwelveTest.Features.NotFound;

using OptimizelyTwelveTest.Features.Common.Pages;

public class NotFoundPageViewModel : ISitePageViewModel<NotFoundPage>
{
    public NotFoundPage CurrentPage { get; set; }

    public int StatusCode { get; set; }
}