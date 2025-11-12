namespace OptiNetNine.Features.NotFound;

using OptiNetNine.Features.Common.Pages;

public class NotFoundPageViewModel : ISitePageViewModel<NotFoundPage>
{
    public NotFoundPage CurrentPage { get; set; }

    public int StatusCode { get; set; }
}