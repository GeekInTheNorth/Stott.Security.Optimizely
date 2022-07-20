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

    public async Task SaveAsync(
        XContentTypeOptions xContentTypeOptions,
        XssProtection xXssProtection,
        ReferrerPolicy referrerPolicy,
        XFrameOptions frameOptions)
    {
        var settings = await _repository.GetAsync();
        settings ??= new SecurityHeaderSettings();

        settings.XContentTypeOptions = xContentTypeOptions;
        settings.XssProtection = xXssProtection;
        settings.ReferrerPolicy = referrerPolicy;
        settings.FrameOptions = frameOptions;

        await _repository.SaveAsync(settings);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }

    public async Task SaveAsync(
        CrossOriginEmbedderPolicy crossOriginEmbedderPolicy, 
        CrossOriginOpenerPolicy crossOriginOpenerPolicy, 
        CrossOriginResourcePolicy crossOriginResourcePolicy)
    {
        var settings = await _repository.GetAsync();
        settings ??= new SecurityHeaderSettings();

        settings.CrossOriginEmbedderPolicy = crossOriginEmbedderPolicy;
        settings.CrossOriginOpenerPolicy = crossOriginOpenerPolicy;
        settings.CrossOriginResourcePolicy = crossOriginResourcePolicy;

        await _repository.SaveAsync(settings);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }

    public async Task SaveAsync(
        bool isStrictTransportSecurityEnabled, 
        bool isStrictTransportSecuritySubDomainsEnabled, 
        int strictTransportSecurityMaxAge)
    {
        var settings = await _repository.GetAsync();
        settings ??= new SecurityHeaderSettings();

        settings.IsStrictTransportSecurityEnabled = isStrictTransportSecurityEnabled;
        settings.IsStrictTransportSecuritySubDomainsEnabled = isStrictTransportSecuritySubDomainsEnabled;
        settings.StrictTransportSecurityMaxAge = strictTransportSecurityMaxAge;

        await _repository.SaveAsync(settings);

        _cacheWrapper.Remove(CspConstants.CacheKeys.CompiledCsp);
    }
}
