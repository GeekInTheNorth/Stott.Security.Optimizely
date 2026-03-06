using EPiServer.DataAnnotations;
using OptimizelyTwelveTest.Features.Common.Pages;

namespace OptimizelyTwelveTest.Features.Campaign;

[ContentType(DisplayName = "Campaign Page", GUID = "d1b9c8e5-9f3a-4c2b-8e5a-1f2b3c4d5e6f", Description = "A page type for creating rich campaigns.")]
public class CampaignPage : SiteExperienceData
{
}
