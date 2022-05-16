using System.Threading.Tasks;

using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;

namespace Stott.Optimizely.Csp.Features.SecurityHeaders.Repository
{
    public interface ISecurityHeaderRepository
    {
        Task<SecurityHeaderSettings> GetAsync();

        Task SaveAsync(bool isXContentTypeOptionsEnabled, bool isXXssProtectionEnabled, ReferrerPolicy referrerPolicy, XFrameOptions frameOptions);
    }
}
