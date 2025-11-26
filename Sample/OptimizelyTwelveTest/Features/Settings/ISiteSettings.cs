using EPiServer.Core;

namespace OptimizelyTwelveTest.Features.Settings
{
    public interface ISiteSettings
    {
        public string SiteName { get; }

        public ContentReference NotFoundPage { get; }
    }
}