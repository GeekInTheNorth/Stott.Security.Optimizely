namespace OptimizelyTwelveTest.Features.Search
{
    using OptimizelyTwelveTest.Features.Common.Pages;

    using System.Collections.Generic;

    public class SearchResponse
    {
        public int TotalRecords { get; set; }

        public IList<SearchResultItem> Results { get; set; }
    }
}