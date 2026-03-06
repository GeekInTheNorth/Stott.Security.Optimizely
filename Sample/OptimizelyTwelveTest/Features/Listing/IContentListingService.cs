using System.Collections.Generic;

namespace OptimizelyTwelveTest.Features.Listing;

public interface IContentListingService
{
    IList<ListingResult> GetListings(string freeText, int page);
}
