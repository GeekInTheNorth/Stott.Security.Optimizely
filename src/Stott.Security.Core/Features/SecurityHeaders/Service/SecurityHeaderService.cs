namespace Stott.Security.Core.Features.SecurityHeaders.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Caching;
using Stott.Security.Core.Features.SecurityHeaders.Enums;
using Stott.Security.Core.Features.SecurityHeaders.Repository;

public class SecurityHeaderService : ISecurityHeaderService
{
    private readonly ISecurityHeaderRepository _repository;

    private readonly ICacheWrapper _cacheWrapper;

    public SecurityHeaderService(
        ISecurityHeaderRepository repository, 
        ICacheWrapper cacheWrapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
    }

    public async Task<SecurityHeaderSettings> GetAsync()
    {
        return await _repository.GetAsync();
    }

    public async Task SaveAsync(bool isXContentTypeOptionsEnabled, bool isXXssProtectionEnabled, ReferrerPolicy referrerPolicy, XFrameOptions frameOptions)
    {
        await _repository.SaveAsync(isXContentTypeOptionsEnabled, isXXssProtectionEnabled, referrerPolicy, frameOptions);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }
}