using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Cors.Repository;
using Stott.Security.Optimizely.Features.Tools.Models;

namespace Stott.Security.Optimizely.Features.Tools;

internal sealed class MigrationRepository : IMigrationRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public MigrationRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task SaveAsync(SettingsModel? settings, string? modifiedBy, Guid? siteId = null, string? hostName = null)
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
            await UpdateCspSettings(settings.Csp, modifiedBy, modifiedDate, siteId, hostName);
            await UpdateCspSandbox(settings.Csp.Sandbox, modifiedBy, modifiedDate, siteId, hostName);
            await UpdateCspSources(settings.Csp.Sources, modifiedBy, modifiedDate, siteId, hostName);
        }

        if (settings.Cors is not null)
        {
            await UpdateCors(settings.Cors, modifiedBy, modifiedDate);
        }

        if (settings.PermissionPolicy is not null)
        {
            await UpdatePermissionPolicySettings(settings.PermissionPolicy, modifiedBy, modifiedDate, siteId, hostName);
            await UpdatePermissionsPolicyDirectives(settings.PermissionPolicy?.Directives, modifiedBy, modifiedDate, siteId, hostName);
        }

        if (settings.CustomHeaders is not null)
        {
            await UpdateCustomHeaders(settings.CustomHeaders, modifiedBy, modifiedDate, siteId, hostName);
        }

        await _context.Value.SaveChangesAsync();
    }

    private async Task UpdateCspSettings(CspSettingsMigrationModel? settings, string modifiedBy, DateTime modified, Guid? siteId = null, string? hostName = null)
    {
        if (settings is null)
        {
            return;
        }

        var settingsToUpdate = await _context.Value.CspSettings
            .Where(x => x.SiteId == siteId && x.HostName == hostName)
            .OrderByDescending(x => x.Modified)
            .FirstOrDefaultAsync();
        if (settingsToUpdate == null)
        {
            settingsToUpdate = MigrationMapper.ConvertToEntity(settings, siteId, hostName, modified, modifiedBy);
            _context.Value.CspSettings.Add(settingsToUpdate);
        }
        else
        {
            MigrationMapper.MapToEntity(settingsToUpdate, settings, modified, modifiedBy);
        }
    }

    private async Task UpdateCspSandbox(CspSandboxMigrationModel? sandbox, string modifiedBy, DateTime modified, Guid? siteId = null, string? hostName = null)
    {
        if (sandbox is null)
        {
            return;
        }

        var recordToSave = await _context.Value.CspSandboxes
            .Where(x => x.SiteId == siteId && x.HostName == hostName)
            .FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = MigrationMapper.ConvertToEntity(sandbox, siteId, hostName, modified, modifiedBy);
            _context.Value.CspSandboxes.Add(recordToSave);
        }
        else
        {
            MigrationMapper.MapToEntity(recordToSave, sandbox, modified, modifiedBy);
        }
    }

    private async Task UpdateCspSources(List<CspSourceMigrationModel>? sources, string modifiedBy, DateTime modified, Guid? siteId = null, string? hostName = null)
    {
        var existingSources = await _context.Value.CspSources.Where(x => x.SiteId == siteId && x.HostName == hostName).ToListAsync();

        var newSources = sources?.Where(x => !string.IsNullOrWhiteSpace(x.Source) && x.Directives is { Count: > 0 }).ToList() ?? new List<CspSourceMigrationModel>();

        var sourcesToDelete = existingSources.Where(x => !newSources.Any(y => string.Equals(y.Source, x.Source, StringComparison.OrdinalIgnoreCase))).ToList();
        foreach (var sourceToDelete in sourcesToDelete)
        {
            _context.Value.CspSources.Remove(sourceToDelete);
        }

        var sourcesToAdd = newSources.Where(x => !existingSources.Any(y => string.Equals(x.Source, y.Source, StringComparison.OrdinalIgnoreCase))).ToList();
        foreach (var sourceToAdd in sourcesToAdd)
        {
            _context.Value.CspSources.Add(MigrationMapper.ConvertToEntity(sourceToAdd, siteId, hostName, modified, modifiedBy));
        }

        var matches = (from existingSource in existingSources
                       join newSource in newSources
                       on existingSource.Source?.ToUpperInvariant() equals newSource.Source?.ToUpperInvariant()
                       select new
                       {
                           existingSource,
                           newSource
                       }).ToList();

        foreach (var match in matches)
        {
            MigrationMapper.MapToEntity(match.existingSource, match.newSource, modified, modifiedBy);
            _context.Value.CspSources.Attach(match.existingSource);
        }
    }

    private async Task UpdateCors(CorsConfiguration corsConfiguration, string modifiedBy, DateTime modified)
    {
        var recordToSave = await _context.Value.CorsSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CorsSettings();
            _context.Value.CorsSettings.Add(recordToSave);
        }

        CorsSettingsMapper.MapToEntity(corsConfiguration, recordToSave);
        recordToSave.Modified = modified;
        recordToSave.ModifiedBy = modifiedBy;
    }

    private async Task UpdatePermissionPolicySettings(PermissionPolicyMigrationModel settings, string modifiedBy, DateTime modified, Guid? siteId = null, string? hostName = null)
    {
        var recordToSave = await _context.Value.PermissionPolicySettings.Where(x => x.SiteId == siteId && x.HostName == hostName).OrderByDescending(x => x.Modified).FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = MigrationMapper.ConvertToEntity(settings, siteId, hostName, modified, modifiedBy);
            _context.Value.PermissionPolicySettings.Add(recordToSave);
        }
        else
        {
            MigrationMapper.MapToEntity(recordToSave, settings, modified, modifiedBy);
        }
    }

    private async Task UpdatePermissionsPolicyDirectives(IList<PermissionPolicyDirectiveMigrationModel>? directives, string modifiedBy, DateTime modified, Guid? siteId = null, string? hostName = null)
    {
        var existingDirectives = await _context.Value.PermissionPolicies.Where(x => x.SiteId == siteId && x.HostName == hostName).ToListAsync();

        var newDirectives = directives?.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList() ?? new List<PermissionPolicyDirectiveMigrationModel>();

        var directivesToDelete = existingDirectives.Where(x => !newDirectives.Any(y => y.Name!.Equals(x.Directive))).ToList();
        foreach (var directiveToDelete in directivesToDelete)
        {
            _context.Value.PermissionPolicies.Remove(directiveToDelete);
        }

        var directivesToAdd = newDirectives.Where(x => !existingDirectives.Any(y => x.Name!.Equals(y.Directive))).ToList();
        foreach (var directiveToAdd in directivesToAdd)
        {
            _context.Value.PermissionPolicies.Add(MigrationMapper.ConvertToEntity(directiveToAdd, siteId, hostName, modified, modifiedBy));
        }

        var matches = (from existingDirective in existingDirectives
                       join newDirective in newDirectives on existingDirective.Directive equals newDirective.Name
                       select new
                       {
                           existingDirective,
                           newDirective
                       }).ToList();

        foreach (var item in matches)
        {
            MigrationMapper.MapToEntity(item.existingDirective, item.newDirective, modified, modifiedBy);
            _context.Value.PermissionPolicies.Attach(item.existingDirective);
        }
    }

    private async Task UpdateCustomHeaders(List<CustomHeaderMigrationModel> customHeaders, string modifiedBy, DateTime modified, Guid? siteId = null, string? hostName = null)
    {
        var existingHeaders = await _context.Value.CustomHeaders.Where(x => x.SiteId == siteId && x.HostName == hostName).ToListAsync();

        var newHeaders = customHeaders.Where(x => !string.IsNullOrWhiteSpace(x.HeaderName)).ToList();

        var headersToDelete = existingHeaders.Where(x => !newHeaders.Any(y => y.HeaderName!.Equals(x.HeaderName, StringComparison.OrdinalIgnoreCase))).ToList();
        foreach (var headerToDelete in headersToDelete)
        {
            _context.Value.CustomHeaders.Remove(headerToDelete);
        }

        var headersToAdd = newHeaders.Where(x => !existingHeaders.Any(y => x.HeaderName!.Equals(y.HeaderName, StringComparison.OrdinalIgnoreCase))).ToList();
        foreach (var headerToAdd in headersToAdd)
        {
            _context.Value.CustomHeaders.Add(MigrationMapper.ConvertToEntity(headerToAdd, siteId, hostName, modified, modifiedBy));
        }

        var matches = (from existingHeader in existingHeaders
                       join newHeader in newHeaders on existingHeader.HeaderName.ToUpperInvariant() equals newHeader.HeaderName!.ToUpperInvariant()
                       select new
                       {
                           existingHeader,
                           newHeader
                       }).ToList();

        foreach (var item in matches)
        {
            MigrationMapper.MapToEntity(item.existingHeader, item.newHeader, modified, modifiedBy);
            _context.Value.CustomHeaders.Attach(item.existingHeader);
        }
    }

    private static void HandleRemapping(CspSettingsMigrationModel settings, bool isEnabled, string sourceName)
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
            settings.Sources.Add(new CspSourceMigrationModel
            {
                Source = sourceName,
                Directives = nonceDirectives
            });
        }
    }
}