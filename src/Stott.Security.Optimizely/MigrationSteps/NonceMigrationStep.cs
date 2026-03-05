using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.DataAbstraction.Migration;
using EPiServer.ServiceLocation;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

namespace Stott.Security.Optimizely.MigrationSteps;

public class NonceMigrationStep : MigrationStep
{
    private bool _hasStarted = false;

    public override void AddChanges()
    {
        try
        {
            if (_hasStarted)
            {
                return;
            }

            _hasStarted = true;

            ConvertNonceSettings();
        }
        catch (Exception)
        {
        }
    }

    private static void ConvertNonceSettings()
    {
        var settingsService = ServiceLocator.Current.GetInstance<ICspSettingsService>();
        var settings = settingsService.GetAsync().GetAwaiter().GetResult();
        if (settings == null || !settings.IsNonceEnabled)
        {
            return;
        }

        GenerateNonceSources(settings.IsNonceEnabled, settings.IsStrictDynamicEnabled);

        settings.IsNonceEnabled = false;
        settings.IsStrictDynamicEnabled = false;
        settingsService.SaveAsync(settings, "System").GetAwaiter().GetResult();
    }

    private static void GenerateNonceSources(bool includeNonce, bool includeStrictDynamic)
    {
        var sourcesService = ServiceLocator.Current.GetInstance<ICspPermissionService>();
        var sources = sourcesService.GetAsync().GetAwaiter().GetResult();
        if (sources == null || !sources.Any())
        {
            return;
        }

        var allDirectives = sources.Where(x => !string.IsNullOrWhiteSpace(x.Directives))
                                   .SelectMany(x => x.Directives.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                                   .Distinct(StringComparer.OrdinalIgnoreCase)
                                   .ToList();
        var nonceDirectives = CspConstants.NonceDirectives.Where(allDirectives.Contains).ToList();
        if (nonceDirectives.Count == 0)
        {
            return;
        }

        if (includeNonce && !sources.Any(x => string.Equals(x.Source, CspConstants.Sources.Nonce, StringComparison.OrdinalIgnoreCase)))
        {
            sourcesService.SaveAsync(Guid.Empty, CspConstants.Sources.Nonce, nonceDirectives, "System").GetAwaiter().GetResult();
        }

        if (includeStrictDynamic && !sources.Any(x => string.Equals(x.Source, CspConstants.Sources.StrictDynamic, StringComparison.OrdinalIgnoreCase)))
        {
            sourcesService.SaveAsync(Guid.Empty, CspConstants.Sources.StrictDynamic, nonceDirectives, "System").GetAwaiter().GetResult();
        }
    }
}
