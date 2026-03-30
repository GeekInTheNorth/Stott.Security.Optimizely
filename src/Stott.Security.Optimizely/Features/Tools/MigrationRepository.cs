using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Cors.Repository;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

namespace Stott.Security.Optimizely.Features.Tools;

internal sealed class MigrationRepository(Lazy<IStottSecurityDataContext> context) : IMigrationRepository
{
    public async Task SaveAsync(SettingsModel? settings, string? modifiedBy, string? appId = null, string? hostName = null)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy) || settings is null)
        {
            return;
        }

        var modifiedDate = DateTime.UtcNow;

        if (settings.Csp is not null)
        {
            HandleRemapping(settings.Csp, settings.Csp.IsNonceEnabled, CspConstants.Sources.Nonce);
            HandleRemapping(settings.Csp, settings.Csp.IsStrictDynamicEnabled, CspConstants.Sources.StrictDynamic);
            await UpdateCspSettings(settings.Csp, modifiedBy, modifiedDate, appId, hostName);
            await UpdateCspSandbox(settings.Csp.Sandbox, modifiedBy, modifiedDate, appId, hostName);
            await UpdateCspSources(settings.Csp.Sources, modifiedBy, modifiedDate, appId, hostName);
        }

        if (settings.Cors is not null)
        {
            await UpdateCors(settings.Cors, modifiedBy, modifiedDate);
        }

        if (settings.PermissionPolicy is not null)
        {
            await UpdatePermissionPolicySettings(settings.PermissionPolicy, modifiedBy, modifiedDate, appId, hostName);
            await UpdatePermissionsPolicyDirectives(settings.PermissionPolicy?.Directives, modifiedBy, modifiedDate, appId, hostName);
        }

        if (settings.CustomHeaders is not null)
        {
            await UpdateCustomHeaders(settings.CustomHeaders, modifiedBy, modifiedDate, appId, hostName);
        }

        await context.Value.SaveChangesAsync();
    }

    private async Task UpdateCspSettings(CspSettingsModel? settings, string modifiedBy, DateTime modified, string? appId, string? hostName)
    {
        if (settings is null)
        {
            return;
        }

        var settingsToUpdate = await context.Value.CspSettings
            .Where(x => x.AppId == appId && x.HostName == hostName)
            .OrderByDescending(x => x.Modified)
            .FirstOrDefaultAsync();
        if (settingsToUpdate == null)
        {
            settingsToUpdate = new CspSettings { AppId = appId, HostName = hostName };
            context.Value.CspSettings.Add(settingsToUpdate);
        }

        CspSettingsMapper.ToEntity(settings, settingsToUpdate);
        settingsToUpdate.IsReportOnly = settings?.IsEnabled ?? false; // If enabled, then make it report only
        settingsToUpdate.Modified = modified;
        settingsToUpdate.ModifiedBy = modifiedBy;
    }

    private async Task UpdateCspSandbox(SandboxModel? sandbox, string modifiedBy, DateTime modified, string? appId, string? hostName)
    {
        if (sandbox is null)
        {
            return;
        }

        var recordToSave = await context.Value.CspSandboxes
            .Where(x => x.AppId == appId && x.HostName == hostName)
            .FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CspSandbox { AppId = appId, HostName = hostName };
            context.Value.CspSandboxes.Add(recordToSave);
        }

        CspSandboxMapper.ToEntity(sandbox, recordToSave);

        recordToSave.Modified = modified;
        recordToSave.ModifiedBy = modifiedBy;
    }

    private async Task UpdateCspSources(List<CspSourceModel>? sources, string modifiedBy, DateTime modified, string? appId, string? hostName)
    {
        var existingSources = await context.Value.CspSources.Where(x => x.AppId == appId && x.HostName == hostName).ToListAsync();

        var newSources = sources?.Where(x => !string.IsNullOrWhiteSpace(x.Source) && x.Directives is { Count: > 0 }).ToList() ?? [];

        var sourcesToDelete = existingSources.Where(x => !newSources.Any(y => string.Equals(y.Source, x.Source, StringComparison.OrdinalIgnoreCase))).ToList();
        foreach (var sourceToDelete in sourcesToDelete)
        {
            context.Value.CspSources.Remove(sourceToDelete);
        }

        var sourcesToAdd = newSources.Where(x => !existingSources.Any(y => string.Equals(x.Source, y.Source, StringComparison.OrdinalIgnoreCase))).ToList();
        foreach (var sourceToAdd in sourcesToAdd)
        {
            context.Value.CspSources.Add(new CspSource
            {
                Source = sourceToAdd.Source,
                Directives = string.Join(',', sourceToAdd.Directives ?? []),
                AppId = appId,
                HostName = hostName,
                Modified = modified,
                ModifiedBy = modifiedBy
            });
        }

        var sourcesToUpdate = (from existingSource in existingSources
                               join newSource in newSources
                               on existingSource.Source?.ToUpperInvariant() equals newSource.Source?.ToUpperInvariant()
                               select new
                               {
                                   existingSource,
                                   newSource.Directives
                               }).ToList();

        foreach (var sourceToUpdate in sourcesToUpdate)
        {
            sourceToUpdate.existingSource.Modified = modified;
            sourceToUpdate.existingSource.ModifiedBy = modifiedBy;
            sourceToUpdate.existingSource.Directives = string.Join(',', sourceToUpdate.Directives ?? []);

            context.Value.CspSources.Attach(sourceToUpdate.existingSource);
        }
    }

    private async Task UpdateCors(CorsConfiguration corsConfiguration, string modifiedBy, DateTime modified)
    {
        var recordToSave = await context.Value.CorsSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CorsSettings();
            context.Value.CorsSettings.Add(recordToSave);
        }

        CorsSettingsMapper.MapToEntity(corsConfiguration, recordToSave);
        recordToSave.Modified = modified;
        recordToSave.ModifiedBy = modifiedBy;
    }

    private async Task UpdatePermissionPolicySettings(PermissionPolicyModel settings, string modifiedBy, DateTime modified, string? appId, string? hostName)
    {
        var recordToSave = await context.Value.PermissionPolicySettings.Where(x => x.AppId == appId && x.HostName == hostName).OrderByDescending(x => x.Modified).FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new PermissionPolicySettings { AppId = appId, HostName = hostName };
            context.Value.PermissionPolicySettings.Add(recordToSave);
        }

        recordToSave.IsEnabled = settings.IsEnabled;
        recordToSave.Modified = modified;
        recordToSave.ModifiedBy = modifiedBy;
    }

    private async Task UpdatePermissionsPolicyDirectives(IList<PermissionPolicyDirectiveModel>? directives, string modifiedBy, DateTime modified, string? appId, string? hostName)
    {
        var existingDirectives = await context.Value.PermissionPolicies.Where(x => x.AppId == appId && x.HostName == hostName).ToListAsync();

        var newDirectives = directives?.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList() ?? [];

        var directivesToDelete = existingDirectives.Where(x => !newDirectives.Any(y => y.Name!.Equals(x.Directive))).ToList();
        foreach (var directiveToDelete in directivesToDelete)
        {
            context.Value.PermissionPolicies.Remove(directiveToDelete);
        }

        var directivesToAdd = newDirectives.Where(x => !existingDirectives.Any(y => x.Name!.Equals(y.Directive))).ToList();
        foreach (var directiveToAdd in directivesToAdd)
        {
            context.Value.PermissionPolicies.Add(PermissionPolicyMapper.ToEntity(directiveToAdd, modifiedBy, modified, appId, hostName));
        }

        var directivesToUpdate = (from existingDirective in existingDirectives
                                  join newDirective in newDirectives on existingDirective.Directive equals newDirective.Name
                                  select new
                                  {
                                      existingDirective,
                                      newDirective.Sources,
                                      newDirective.EnabledState
                                  }).ToList();

        foreach (var item in directivesToUpdate)
        {
            item.existingDirective.Modified = modified;
            item.existingDirective.ModifiedBy = modifiedBy;
            item.existingDirective.EnabledState = item.EnabledState.ToString();
            item.existingDirective.Origins = string.Join(',', item.Sources.Select(x => x.Url));

            context.Value.PermissionPolicies.Attach(item.existingDirective);
        }
    }

    private async Task UpdateCustomHeaders(List<CustomHeaderModel> customHeaders, string modifiedBy, DateTime modified, string? appId, string? hostName)
    {
        var existingHeaders = await context.Value.CustomHeaders.Where(x => x.AppId == appId && x.HostName == hostName).ToListAsync();

        var newHeaders = customHeaders.Where(x => !string.IsNullOrWhiteSpace(x.HeaderName)).ToList();

        var headersToDelete = existingHeaders.Where(x => !newHeaders.Any(y => y.HeaderName!.Equals(x.HeaderName, StringComparison.OrdinalIgnoreCase))).ToList();
        foreach (var headerToDelete in headersToDelete)
        {
            context.Value.CustomHeaders.Remove(headerToDelete);
        }

        var headersToAdd = newHeaders.Where(x => !existingHeaders.Any(y => x.HeaderName!.Equals(y.HeaderName, StringComparison.OrdinalIgnoreCase))).ToList();
        foreach (var headerToAdd in headersToAdd)
        {
            context.Value.CustomHeaders.Add(new CustomHeader
            {
                HeaderName = headerToAdd.HeaderName!,
                Behavior = headerToAdd.Behavior,
                HeaderValue = headerToAdd.HeaderValue,
                AppId = appId,
                HostName = hostName,
                Modified = modified,
                ModifiedBy = modifiedBy
            });
        }

        var headersToUpdate = (from existingHeader in existingHeaders
                               join newHeader in newHeaders on existingHeader.HeaderName.ToUpperInvariant() equals newHeader.HeaderName!.ToUpperInvariant()
                               select new
                               {
                                   existingHeader,
                                   newHeader.Behavior,
                                   newHeader.HeaderValue
                               }).ToList();

        foreach (var item in headersToUpdate)
        {
            item.existingHeader.Behavior = item.Behavior;
            item.existingHeader.HeaderValue = item.HeaderValue;
            item.existingHeader.Modified = modified;
            item.existingHeader.ModifiedBy = modifiedBy;

            context.Value.CustomHeaders.Attach(item.existingHeader);
        }
    }

    private static void HandleRemapping(CspSettingsModel settings, bool isEnabled, string sourceName)
    {
        if (!isEnabled || settings is not { Sources.Count: >0 } || string.IsNullOrWhiteSpace(sourceName))
        {
            return;
        }

        var allDirectives = settings.Sources
                                    .Where(x => x.Directives is not null)
                                    .SelectMany(x => x.Directives!)
                                    .Distinct(StringComparer.OrdinalIgnoreCase)
                                    .ToList();

        var nonceDirectives = CspConstants.NonceDirectives.Where(allDirectives.Contains).ToList();
        var existingSource = settings.Sources.FirstOrDefault(x => sourceName.Equals(x.Source, StringComparison.OrdinalIgnoreCase));
        if (existingSource is null)
        {
            settings.Sources.Add(new CspSourceModel
            {
                Source = sourceName,
                Directives = nonceDirectives
            });
        }
    }
}
