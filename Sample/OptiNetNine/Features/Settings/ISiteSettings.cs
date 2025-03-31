using EPiServer.Core;

namespace OptiNetSix.Features.Settings
{
    public interface ISiteSettings
    {
        public string SiteName { get; }

        public ContentReference NotFoundPage { get; }
    }
}