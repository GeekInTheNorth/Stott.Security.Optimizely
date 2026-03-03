using System.Collections.Generic;

namespace OptimizelyTwelveTest.Features.Listing;

public class ListingBlockViewModel
{
    public string Heading { get; set; }

    public ListingBlock CurrentContent { get; set; }

    public IList<ListingResult> Results { get; set; }

    public List<PaginationModel> Pagination { get; set; }
}

public class PaginationModel
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public string BaseUrl { get; set; }
}