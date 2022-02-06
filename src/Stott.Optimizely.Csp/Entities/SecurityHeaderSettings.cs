using EPiServer.Data;
using EPiServer.Data.Dynamic;

using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;

namespace Stott.Optimizely.Csp.Entities
{
    public class SecurityHeaderSettings : IDynamicData
    {
        public Identity Id { get; set; }

        public bool IsXContentTypeOptionsEnabled { get; set; }

        public bool IsXXssProtectionEnabled { get; set; }

        public ReferrerPolicy ReferrerPolicy { get; set; }

        public XFrameOptions FrameOptions { get; set; }
    }
}
