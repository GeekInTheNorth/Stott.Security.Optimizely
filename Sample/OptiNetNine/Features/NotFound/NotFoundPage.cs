namespace OptiNetNine.Features.NotFound;

using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

using OptiNetNine.Features.Common.Pages;

[ContentType(
    DisplayName = "Not Found Page",
    GUID = "4344C9C4-7183-482C-A670-9EC550CA19D6",
    Description = "A page designed as a default landing page.",
    GroupName = SystemTabNames.Content)]
public partial class NotFoundPage : SitePageData
{
}