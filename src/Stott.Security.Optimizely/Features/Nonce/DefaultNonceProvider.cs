namespace Stott.Security.Optimizely.Features.Nonce;

using System;

using EPiServer.Core;
using EPiServer.Web.Templating;

using Microsoft.AspNetCore.Http;

using Stott.Security.Optimizely.Features.Settings.Service;

public sealed class DefaultNonceProvider : INonceProvider
{
    private readonly ICspSettingsService _settingsService;

    private readonly IHttpContextAccessor _contextAccessor;

    private string? _nonce;

    public DefaultNonceProvider(
        ICspSettingsService settingsService,
        IHttpContextAccessor contextAccessor)
    {
        _settingsService = settingsService;
        _contextAccessor = contextAccessor;
    }

    public string? GetNonce()
    {
        // Optimizely CMS Editor / Admin inteface does not support Nonce
        // So only apply this on content pages
        if (!IsContentPage())
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(_nonce))
        {
            return _nonce;
        }

        _nonce = Guid.NewGuid().ToString();

        var settings = _settingsService.Get();

        return settings.IsEnabled && settings.IsNonceEnabled ? _nonce : null;
    }

    private bool IsContentPage()
    {
        var isStottSecurity = _contextAccessor.HttpContext?.Request?.Path.StartsWithSegments("/stott.security.optimizely/api/compiled-headers") ?? false;

        var renderingContext = _contextAccessor.HttpContext?.Items["Epi:ContentRenderingContext"] as ContentRenderingContext;

        return isStottSecurity || renderingContext?.Content is PageData;
    }
}