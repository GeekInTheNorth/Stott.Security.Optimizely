namespace OptimizelyTwelveTest.Features.GeneralContent
{
    using Common.Pages;

    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;
    using EPiServer.Web;

    using System.ComponentModel.DataAnnotations;

    [ContentType(
        DisplayName = "GeneralContentPage", 
        GUID = "30479482-2964-4a41-8da4-013e1b5e4a9b", 
        Description = "",
        GroupName = SystemTabNames.Content)]
    public class GeneralContentPage : SitePageData
    {
        [Display(
            Name = "Hero Image",
            Description = "The image to render at the top of the page",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference HeroImage { get; set; }

        [Display(
            Name = "Heading",
            Description = "The H1 to display",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual string Heading { get; set; }

        [Display(
            Name = "Main Content Area",
            Description = "Renders blocks within the main content section of the home page.",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        public virtual ContentArea MainContentArea { get; set; }
    }
}