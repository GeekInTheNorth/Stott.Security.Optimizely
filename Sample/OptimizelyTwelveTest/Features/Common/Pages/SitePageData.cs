namespace OptimizelyTwelveTest.Features.Common.Pages
{
    using EPiServer.Core;
    using EPiServer.Web;

    using System.ComponentModel.DataAnnotations;

    public class SitePageData : PageData, ISitePageData
    {
        [Display(
            Name = "Teaser Title",
            Description = "A teaser title to use when rendering as a block",
            GroupName = GroupNames.Teaser,
            Order = 700)]
        public virtual string TeaserTitle { get; set; }

        [Display(
            Name = "Teaser Text",
            Description = "Teaser text to use when rendering as a block",
            GroupName = GroupNames.Teaser,
            Order = 701)]
        public virtual string TeaserText { get; set; }

        [Display(
            Name = "Teaser Image",
            Description = "The image to use when rendering as a block",
            GroupName = GroupNames.Teaser,
            Order = 702)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference TeaserImage { get; set; }

        [Display(
            Name = "Meta Title",
            Description = "A meta title to be rendered within the page's title",
            GroupName = GroupNames.SearchEngineOptimization,
            Order = 800)]
        public virtual string MetaTitle { get; set; }

        [Display(
            Name = "Meta Description",
            Description = "The meta description of the page to be shown in search results.",
            GroupName = GroupNames.SearchEngineOptimization,
            Order = 801)]
        public virtual string MetaText { get; set; }

        [Display(
            Name = "Meta Image",
            Description = "The image to use for the meta image for the page",
            GroupName = GroupNames.SearchEngineOptimization,
            Order = 802)]
        [UIHint(UIHint.Image)]
        public virtual ContentReference MetaImage { get; set; }
    }
}
