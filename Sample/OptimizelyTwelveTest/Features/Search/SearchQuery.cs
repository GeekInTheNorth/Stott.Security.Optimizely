namespace OptimizelyTwelveTest.Features.Search
{
    using MediatR;

    public class SearchQuery : IRequest<SearchResponse>
    {
        public string SearchText { get; set; }

        public int InitialPageSize { get; set; }

        public int LoadMorePageSize { get; set; }

        public int Page { get; set; }
    }
}