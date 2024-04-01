namespace Stott.Security.Optimizely.Features.SecurityHeaders.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;

internal sealed class SecurityHeaderService : ISecurityHeaderService
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

    public async Task<SecurityHeaderModel> GetAsync()
    {
        var settings = await _repository.GetAsync();

        return SecurityHeaderMapper.ToModel(settings);
    }

    public async Task SaveAsync(
        XContentTypeOptions xContentTypeOptions,
        XssProtection xXssProtection,
        ReferrerPolicy referrerPolicy,
        XFrameOptions frameOptions,
        string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        var settings = await _repository.GetAsync();
        settings ??= new SecurityHeaderSettings();

        settings.XContentTypeOptions = xContentTypeOptions;
        settings.XssProtection = xXssProtection;
        settings.ReferrerPolicy = referrerPolicy;
        settings.FrameOptions = frameOptions;
        settings.Modified = DateTime.UtcNow;
        settings.ModifiedBy = modifiedBy;

        await _repository.SaveAsync(settings);

        _cacheWrapper.RemoveAll();
    }

    public async Task SaveAsync(
        CrossOriginEmbedderPolicy crossOriginEmbedderPolicy, 
        CrossOriginOpenerPolicy crossOriginOpenerPolicy, 
        CrossOriginResourcePolicy crossOriginResourcePolicy,
        string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        var settings = await _repository.GetAsync();
        settings ??= new SecurityHeaderSettings();

        settings.CrossOriginEmbedderPolicy = crossOriginEmbedderPolicy;
        settings.CrossOriginOpenerPolicy = crossOriginOpenerPolicy;
        settings.CrossOriginResourcePolicy = crossOriginResourcePolicy;
        settings.Modified = DateTime.UtcNow;
        settings.ModifiedBy = modifiedBy;

        await _repository.SaveAsync(settings);

        _cacheWrapper.RemoveAll();
    }

    public async Task SaveAsync(
        bool isStrictTransportSecurityEnabled, 
        bool isStrictTransportSecuritySubDomainsEnabled, 
        int strictTransportSecurityMaxAge,
        string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            return;
        }

        var settings = await _repository.GetAsync();
        settings ??= new SecurityHeaderSettings();

        settings.IsStrictTransportSecurityEnabled = isStrictTransportSecurityEnabled;
        settings.IsStrictTransportSecuritySubDomainsEnabled = isStrictTransportSecuritySubDomainsEnabled;
        settings.StrictTransportSecurityMaxAge = strictTransportSecurityMaxAge;
        settings.Modified = DateTime.UtcNow;
        settings.ModifiedBy = modifiedBy;

        await _repository.SaveAsync(settings);

        _cacheWrapper.RemoveAll();
    }
}