namespace OptimizelyTwelveTest.Features.Home
{
    using OptimizelyTwelveTest.Features.Common.Pages;

    public class HomePageViewModel : ISitePageViewModel<HomePage>
    {
        public HomePage CurrentPage { get; set; }
    }
}