using EPiServer.Core;

namespace OptiNetNine.Features.Settings
{
    public interface ISiteSettings
    {
        public string SiteName { get; }

        public ContentReference NotFoundPage { get; }
    }
}