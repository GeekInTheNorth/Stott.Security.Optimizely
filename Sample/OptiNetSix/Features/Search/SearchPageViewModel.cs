namespace OptiNetSix.Features.Search
{
    using OptiNetSix.Features.Common.Pages;

    public class SearchPageViewModel: ISitePageViewModel<SearchPage>
    {
        public SearchPage CurrentPage { get; set; }

        public SearchResponse SearchResults { get; set; }
    }
}