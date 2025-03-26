namespace Stott.Security.Optimizely.Features.SecurityHeaders.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;

internal sealed class SecurityHeaderService : ISecurityHeaderService
{
    private readonly ISecurityHeaderRepository _repository;

    private readonly ICacheWrapper _cacheWrapper;

    private const string CacheKey = "stott.security.response.headers";

    public SecurityHeaderService(
        ISecurityHeaderRepository repository, 
        ICacheWrapper cacheWrapper)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
    }

    public async Task<SecurityHeaderModel> GetAsync()
    {
        var settings = await this.GetSettingsData();

        return SecurityHeaderMapper.ToModel(settings);
    }

    public async Task<IEnumerable<KeyValuePair<string, string>>> GetCompiledHeaders()
    {
        var settings = await this.GetSettingsData();

        return BuildHeaders(settings).ToList();
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

    private async Task<SecurityHeaderSettings> GetSettingsData()
    {
        var settings = _cacheWrapper.Get<SecurityHeaderSettings>(CacheKey);
        if (settings is null)
        {
            settings = await _repository.GetAsync();

            _cacheWrapper.Add(CacheKey, settings);
        }

        return settings;
    }

    private static IEnumerable<KeyValuePair<string, string>> BuildHeaders(SecurityHeaderSettings settings)
    {
        if (settings is null)
        {
            yield break;
        }

        if (settings.XContentTypeOptions != XContentTypeOptions.None)
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.ContentTypeOptions, settings.XContentTypeOptions.GetSecurityHeaderValue());
        }

        if (settings.XssProtection != XssProtection.None)
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.XssProtection, settings.XssProtection.GetSecurityHeaderValue());
        }

        if (settings.ReferrerPolicy != ReferrerPolicy.None)
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.ReferrerPolicy, settings.ReferrerPolicy.GetSecurityHeaderValue());
        }

        if (settings.FrameOptions != XFrameOptions.None)
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.FrameOptions, settings.FrameOptions.GetSecurityHeaderValue());
        }

        if (settings.CrossOriginEmbedderPolicy != CrossOriginEmbedderPolicy.None)
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.CrossOriginEmbedderPolicy, settings.CrossOriginEmbedderPolicy.GetSecurityHeaderValue());
        }

        if (settings.CrossOriginOpenerPolicy != CrossOriginOpenerPolicy.None)
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.CrossOriginOpenerPolicy, settings.CrossOriginOpenerPolicy.GetSecurityHeaderValue());
        }

        if (settings.CrossOriginResourcePolicy != CrossOriginResourcePolicy.None)
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.CrossOriginResourcePolicy, settings.CrossOriginResourcePolicy.GetSecurityHeaderValue());
        }

        if (settings.IsStrictTransportSecurityEnabled)
        {
            yield return new KeyValuePair<string, string>(CspConstants.HeaderNames.StrictTransportSecurity, GetStrictTransportSecurityValue(settings));
        }
    }

    private static string GetStrictTransportSecurityValue(SecurityHeaderSettings settings)
    {
        return settings.IsStrictTransportSecuritySubDomainsEnabled ?
            $"max-age={settings.StrictTransportSecurityMaxAge}; includeSubDomains" :
            $"max-age={settings.StrictTransportSecurityMaxAge}";
    }
}