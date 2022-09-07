namespace Stott.Security.Core.Features.SecurityHeaders.Service;

using System.Threading.Tasks;

using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.SecurityHeaders.Enums;

public interface ISecurityHeaderService
{
    Task<SecurityHeaderSettings> GetAsync();

    Task SaveAsync(
        XContentTypeOptions xContentTypeOptions,
        XssProtection xXssProtection,
        ReferrerPolicy referrerPolicy,
        XFrameOptions frameOptions);

    Task SaveAsync(
    CrossOriginEmbedderPolicy crossOriginEmbedderPolicy,
    CrossOriginOpenerPolicy crossOriginOpenerPolicy,
    CrossOriginResourcePolicy crossOriginResourcePolicy);

    Task SaveAsync(
        bool isStrictTransportSecurityEnabled,
        bool isStrictTransportSecuritySubDomainsEnabled,
        int strictTransportSecurityMaxAge);
}
