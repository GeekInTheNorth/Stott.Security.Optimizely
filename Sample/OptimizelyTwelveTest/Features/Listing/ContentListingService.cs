using System;
using System.Collections.Generic;
using System.Linq;

namespace OptimizelyTwelveTest.Features.Listing;

public class ContentListingService : IContentListingService
{
    public IList<ListingResult> GetListings(string freeText, int page)
    {
        // In a real implementation, this would query the CMS for content items.
        var listings = Enumerable.Range(0, 20).Select(x => new ListingResult { Title = $"Campaign {x}", Url = $"/campaign-{x}" }).ToList();

        if (!string.IsNullOrWhiteSpace(freeText))
        {
            listings = [.. listings.Where(x => x.Title.Contains(freeText, StringComparison.OrdinalIgnoreCase))];
        }

        var skip = (page - 1) * 10;

        return listings.Skip(skip).Take(10).ToList();
    }
}
