namespace OptimizelyTwelveTest.Features.Search
{
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;
    using EPiServer.Web;

    using OptimizelyTwelveTest.Features.Common.Pages;

    using System.ComponentModel.DataAnnotations;

    [ContentType(
        DisplayName = "Search Page",
        GUID = "529f8bf1-2b92-4993-9bf7-d7af1a82b464",
        Description = "A page for general search of the site.",
        GroupName = SystemTabNames.Content)]
    public class SearchPage : SitePageData
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
            Name = "Initial Page Size",
            Description = "The number of results to display on the initial search.",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        [Range(1,100)]
        public virtual int InitialPageSize { get; set; }

        [Display(
            Name = "Load More Page Size",
            Description = "The number of results to display on subsequent page loads.",
            GroupName = SystemTabNames.Content,
            Order = 40)]
        [Range(1, 100)]
        public virtual int LoadMorePageSize { get; set; }

        [Display(
            Name = "Load More CTA Text",
            Description = "The text to display on the Load More CTA button.",
            GroupName = SystemTabNames.Content,
            Order = 50)]
        [Required]
        public virtual string LoadMoreCtaText { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            this.InitialPageSize = 12;
            this.LoadMorePageSize = 9;
            this.LoadMoreCtaText = "Load More";
        }
    }
}