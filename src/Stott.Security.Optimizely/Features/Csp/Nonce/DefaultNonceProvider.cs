namespace Stott.Security.Optimizely.Features.Csp.Nonce;

using System;

using EPiServer.Web.Templating;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

public class DefaultNonceProvider : INonceProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    private readonly CspSettings _settings;

    private readonly string? _nonce;

    public DefaultNonceProvider(
        ICspSettingsService settingsService,
        IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        _settings = settingsService.Get();
        _nonce = Guid.NewGuid().ToString();
    }

    public string? GetNonce()
    {
        // Optimizely CMS Editor / Admin inteface does not support Nonce
        // So only apply this on content pages
        if (!ShouldGenerateNone())
        {
            return null;
        }

        return _nonce;
    }

    public string? GetCspValue()
    {
        // Optimizely CMS Editor / Admin inteface does not support Nonce
        // So only apply this on content pages
        if (!ShouldGenerateNone())
        {
            return null;
        }

        var strictDynamicValue = _settings.IsStrictDynamicEnabled ? CspConstants.StrictDynamic : null;

        return $"'nonce-{_nonce}' {strictDynamicValue}";
    }

    protected virtual bool ShouldGenerateNone()
    {
        if (_settings is not { IsEnabled: true, IsNonceEnabled: true })
        {
            return false;
        }

        var isHeaderListApi = _contextAccessor.HttpContext?.Request?.Path.StartsWithSegments("/stott.security.optimizely/api/compiled-headers") ?? false;
        var renderingContext = _contextAccessor.HttpContext?.Items?[ContentRenderingContext.ContentRenderingContextKey] as ContentRenderingContext;
        var isContent = renderingContext?.Content is not null;

        return isHeaderListApi || isContent;
    }
}