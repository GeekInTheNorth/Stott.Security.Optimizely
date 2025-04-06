namespace OptiNetSix.Features.NotFound;

using OptiNetSix.Features.Common.Pages;

public class NotFoundPageViewModel : ISitePageViewModel<NotFoundPage>
{
    public NotFoundPage CurrentPage { get; set; }

    public int StatusCode { get; set; }
}