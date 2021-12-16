namespace OptimizelyTwelveTest.Features.Settings
{
    using Common;

    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;

    using System.ComponentModel.DataAnnotations;

    [ContentType(
        DisplayName = "Site Settings Page", 
        GUID = "aaee57d9-ce29-41e7-a46d-76ce6e199036", 
        Description = "",
        GroupName = GroupNames.Settings)]
    public class SiteSettingsPage : PageData, ISiteSettings
    {
        [Display(
            Name = "Site Name",
            Description = "Site Name",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual string SiteName { get; set; }
    }
}