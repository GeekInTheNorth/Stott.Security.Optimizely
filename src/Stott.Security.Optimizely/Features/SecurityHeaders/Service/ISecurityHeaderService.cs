namespace Stott.Security.Optimizely.Features.SecurityHeaders.Service;

using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

public interface ISecurityHeaderService
{
    Task<SecurityHeaderModel> GetAsync();

    Task<IEnumerable<HeaderDto>> GetCompiledHeaders();

    Task SaveAsync(
        XContentTypeOptions xContentTypeOptions,
        XssProtection xXssProtection,
        ReferrerPolicy referrerPolicy,
        XFrameOptions frameOptions,
        string? modifiedBy);

    Task SaveAsync(
        CrossOriginEmbedderPolicy crossOriginEmbedderPolicy,
        CrossOriginOpenerPolicy crossOriginOpenerPolicy,
        CrossOriginResourcePolicy crossOriginResourcePolicy,
        string? modifiedBy);

    Task SaveAsync(
        bool isStrictTransportSecurityEnabled,
        bool isStrictTransportSecuritySubDomainsEnabled,
        int strictTransportSecurityMaxAge,
        string? modifiedBy);
}