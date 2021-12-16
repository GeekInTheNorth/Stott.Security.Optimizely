namespace OptimizelyTwelveTest.Features.Home
{
    using EPiServer.DataAbstraction;

    using System.ComponentModel.DataAnnotations;
    using EPiServer.Core;
    using EPiServer.DataAnnotations;
    using Settings;

    public partial class HomePage
    {
        [Display(
            Name = "Site Settings",
            Description = "The currently active settings for this site.",
            GroupName = SystemTabNames.Settings,
            Order = 1000)]
        [AllowedTypes(typeof(SiteSettingsPage))]
        public virtual ContentReference SiteSettings { get; set; }
    }
}
