using System;
using System.Globalization;
using System.Linq;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using OptimizelyTwelveTest.Features.Common.Pages;

namespace OptimizelyTwelveTest.Features.Listing;

public class ListingBlockViewComponent : BlockComponent<ListingBlock>
{
    private readonly IContentRouteHelper contentRouteHelper;

    private readonly IContentListingService contentListingService;

    private readonly IUrlResolver urlResolver;

    public ListingBlockViewComponent(
        IContentRouteHelper contentRouteHelper, IUrlResolver urlResolver)
    {
        this.contentRouteHelper = contentRouteHelper;
        this.contentListingService = new ContentListingService();
        this.urlResolver = urlResolver;
    }

    protected override IViewComponentResult InvokeComponent(ListingBlock currentContent)
    {
        var page = contentRouteHelper.Content as ISitePageData;
        if (page == null)
        {
            return Content(string.Empty);
        }

        var searchTerm = string.Empty;
        if (Request.Query.TryGetValue("freeText", out var rawSearchTerm))
        {
            searchTerm = rawSearchTerm.ToString();
        }

        var pageNumber = 1;
        if (Request.Query.TryGetValue("page", out var rawPage) && int.TryParse(rawPage, out var parsedPageNumber))
        {
            pageNumber = parsedPageNumber;
        }

        var results = contentListingService.GetListings(searchTerm, pageNumber);
        var pages = Enumerable.Range(1, 4).Select(x => new PaginationModel
        {
            PageNumber= x,
            TotalPages = 4,
            BaseUrl = GeneratePageUrl(page, x, searchTerm)
        });

        var model = new ListingBlockViewModel
        {
            Heading = string.IsNullOrWhiteSpace(searchTerm) ? "All Campaigns" : $"Search Results for '{searchTerm}'",
            CurrentContent = currentContent,
            Results = results,
            Pagination = [..pages]
        };

        return View(model);
    }

    private string GeneratePageUrl(ISitePageData page, int pageNumber, string searchTerm)
    {
        var url = urlResolver.GetUrl(page.ContentLink.ToReferenceWithoutVersion(), CultureInfo.CurrentUICulture.Name, new UrlResolverArguments { ContextMode = EPiServer.Web.ContextMode.Default, ForceAbsolute = true});
        var query = string.IsNullOrWhiteSpace(searchTerm) ? $"page={pageNumber}" : $"freeText={searchTerm}&page={pageNumber}";
        var uriBuilder = new UriBuilder(url)
        {
            Query = query
        };

        return uriBuilder.ToString();
    }
}
