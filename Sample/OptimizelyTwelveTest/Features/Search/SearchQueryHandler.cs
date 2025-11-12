namespace OptimizelyTwelveTest.Features.Search
{
    using System.Linq;
    using EPiServer.Find;
    using EPiServer.Find.Cms;

    using MediatR;

    using OptimizelyTwelveTest.Features.Common.Pages;

    using System.Threading;
    using System.Threading.Tasks;
    using EPiServer.Web.Routing;
    using OptimizelyTwelveTest.Features.GeneralContent;
    using OptimizelyTwelveTest.Features.Home;

    public class SearchQueryHandler : IRequestHandler<SearchQuery, SearchResponse>
    {
        private readonly IClient _findClient;

        private readonly UrlResolver _urlResolver;

        public SearchQueryHandler(IClient findClient, UrlResolver urlResolver)
        {
            _findClient = findClient;
            _urlResolver = urlResolver;
        }

        public Task<SearchResponse> Handle(SearchQuery request, CancellationToken cancellationToken)
        {
            var pageSize = request.InitialPageSize;
            var skip = 0;
            if (request.Page > 1)
            {
                pageSize = request.LoadMorePageSize;
                skip = request.InitialPageSize + (request.LoadMorePageSize * (request.Page - 1));
            }

            var searchResult = _findClient.Search<SitePageData>()
                                          .For(request.SearchText)
                                          .UsingSynonyms()
                                          .ApplyBestBets()
                                          .Skip(skip)
                                          .Take(pageSize)
                                          .GetContentResult();

            var response = new SearchResponse
            {
                TotalRecords = searchResult.TotalMatching,
                Results = searchResult.Items.Select(ToSearchResultItem).ToList()
            };

            return Task.FromResult(response);
        }

        private SearchResultItem ToSearchResultItem(SitePageData sitePageData)
        {
            if (sitePageData is HomePage homePage)
            {
                return new SearchResultItem
                {
                    Title = homePage.TeaserTitle ?? homePage.Heading,
                    Description = homePage.TeaserText,
                    ImageUrl = _urlResolver.GetUrl(homePage.TeaserImage)
                };
            }

            if (sitePageData is GeneralContentPage generalContentPage)
            {
                return new SearchResultItem
                {
                    Title = generalContentPage.TeaserTitle ?? generalContentPage.Heading,
                    Description = generalContentPage.TeaserText,
                    ImageUrl = _urlResolver.GetUrl(generalContentPage.TeaserImage)
                };
            }

            return new SearchResultItem
            {
                Title = sitePageData.TeaserTitle,
                Description = sitePageData.TeaserText,
                ImageUrl = _urlResolver.GetUrl(sitePageData.TeaserImage)
            };
        }
    }
}