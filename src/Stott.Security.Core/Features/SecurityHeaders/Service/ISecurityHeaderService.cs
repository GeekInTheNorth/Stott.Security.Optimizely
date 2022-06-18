namespace Stott.Security.Core.Features.SecurityHeaders.Service;

using System.Threading.Tasks;

using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.SecurityHeaders.Enums;

public interface ISecurityHeaderService
{
    Task<SecurityHeaderSettings> GetAsync();

    Task SaveAsync(bool isXContentTypeOptionsEnabled, bool isXXssProtectionEnabled, ReferrerPolicy referrerPolicy, XFrameOptions frameOptions);
}
