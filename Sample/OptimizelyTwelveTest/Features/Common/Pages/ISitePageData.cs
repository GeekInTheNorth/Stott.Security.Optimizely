namespace OptimizelyTwelveTest.Features.Common.Pages
{
    using EPiServer.Core;

    public interface ISitePageData
    {
        string TeaserTitle { get; }

        string TeaserText { get; }

        ContentReference TeaserImage { get; }

        string MetaTitle { get; }

        string MetaText { get; }

        ContentReference MetaImage { get; }
    }
}