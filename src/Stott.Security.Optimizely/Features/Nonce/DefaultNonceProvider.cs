namespace Stott.Security.Optimizely.Features.Nonce;

using System;

using Stott.Security.Optimizely.Features.Settings.Service;

public sealed class DefaultNonceProvider : INonceProvider
{
    private readonly ICspSettingsService _settingsService;

    private string _nonce;

    public DefaultNonceProvider(ICspSettingsService settingsService)
    {
        _settingsService = settingsService;
        _nonce = Guid.NewGuid().ToString();
    }

    public string? GetNonce()
    {
        var settings = _settingsService.Get();

        return settings.IsEnabled && settings.IsNonceEnabled ? _nonce : null;
    }
}