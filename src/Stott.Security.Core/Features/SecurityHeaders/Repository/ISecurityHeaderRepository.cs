using System.Threading.Tasks;

using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.SecurityHeaders.Enums;

namespace Stott.Security.Core.Features.SecurityHeaders.Repository
{
    public interface ISecurityHeaderRepository
    {
        Task<SecurityHeaderSettings> GetAsync();

        Task SaveAsync(bool isXContentTypeOptionsEnabled, bool isXXssProtectionEnabled, ReferrerPolicy referrerPolicy, XFrameOptions frameOptions);
    }
}
