using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;

namespace Stott.Optimizely.Csp.Features.SecurityHeaders.Repository
{
    public interface ISecurityHeaderRepository
    {
        SecurityHeaderSettings Get();

        void Save(bool isXContentTypeOptionsEnabled, bool isXXssProtectionEnabled, ReferrerPolicy referrerPolicy, XFrameOptions frameOptions);
    }
}
