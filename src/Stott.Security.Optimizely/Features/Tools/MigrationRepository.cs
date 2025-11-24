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
using Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;

namespace Stott.Security.Optimizely.Features.Tools;

internal sealed class MigrationRepository : IMigrationRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public MigrationRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task SaveAsync(SettingsModel? settings, string? modifiedBy)
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
            await UpdateCspSettings(settings.Csp, modifiedBy, modifiedDate);
            await UpdateCspSandbox(settings.Csp.Sandbox, modifiedBy, modifiedDate);
            await UpdateCspSources(settings.Csp.Sources, modifiedBy, modifiedDate);
        }
        
        if (settings.Cors is not null)
        {
            await UpdateCors(settings.Cors, modifiedBy, modifiedDate);
        }
        
        if (settings.Headers is not null)
        {
            await UpdateSecurityHeaders(settings.Headers, modifiedBy, modifiedDate);
        }

        if (settings.PermissionPolicy is not null)
        {
            await UpdatePermissionPolicySettings(settings.PermissionPolicy, modifiedBy, modifiedDate);
            await UpdatePermissionsPolicyDirectives(settings.PermissionPolicy?.Directives, modifiedBy, modifiedDate);
        }

        await _context.Value.SaveChangesAsync();
    }

    private async Task UpdateCspSettings(CspSettingsModel? settings, string modifiedBy, DateTime modified)
    {
        if (settings is null)
        {
            return;
        }

        var settingsToUpdate = await _context.Value.CspSettings.OrderByDescending(x => x.Modified).FirstOrDefaultAsync();
        if (settingsToUpdate == null)
        {
            settingsToUpdate = new CspSettings();
            _context.Value.CspSettings.Add(settingsToUpdate);
        }

        CspSettingsMapper.ToEntity(settings, settingsToUpdate);
        settingsToUpdate.IsReportOnly = settings?.IsEnabled ?? false; // If enabled, then make it report only
        settingsToUpdate.Modified = modified;
        settingsToUpdate.ModifiedBy = modifiedBy;
    }

    private async Task UpdateCspSandbox(SandboxModel? sandbox, string modifiedBy, DateTime modified)
    {
        if (sandbox is null)
        {
            return;
        }

        var recordToSave = await _context.Value.CspSandboxes.FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CspSandbox();
            _context.Value.CspSandboxes.Add(recordToSave);
        }

        CspSandboxMapper.ToEntity(sandbox, recordToSave);

        recordToSave.Modified = modified;
        recordToSave.ModifiedBy = modifiedBy;
    }

    private async Task UpdateCspSources(List<CspSourceModel>? sources, string modifiedBy, DateTime modified)
    {
        var existingSources = await _context.Value.CspSources.ToListAsync();

        var newSources = sources?.Where(x => !string.IsNullOrWhiteSpace(x.Source) && x.Directives is { Count: > 0 }).ToList() ?? new List<CspSourceModel>();

        var sourcesToDelete = existingSources.Where(x => !newSources.Any(y => y.Source!.Equals(x.Source))).ToList();
        foreach (var sourceToDelete in sourcesToDelete)
        {
            _context.Value.CspSources.Remove(sourceToDelete);
        }

        var sourcesToAdd = newSources.Where(x => !existingSources.Any(y => x.Source!.Equals(y.Source))).ToList();
        foreach (var sourceToAdd in sourcesToAdd)
        {
            _context.Value.CspSources.Add(new CspSource
            {
                Source = sourceToAdd.Source,
                Directives = string.Join(',', sourceToAdd.Directives ?? new List<string>()),
                Modified = modified,
                ModifiedBy = modifiedBy
            });
        }

        var sourcesToUpdate = (from existingSource in existingSources
                               join newSource in newSources on existingSource.Source equals newSource.Source
                               select new
                               {
                                   existingSource,
                                   newSource.Directives
                               }).ToList();

        foreach (var sourceToUpdate in sourcesToUpdate)
        {
            sourceToUpdate.existingSource.Modified = modified;
            sourceToUpdate.existingSource.ModifiedBy = modifiedBy;
            sourceToUpdate.existingSource.Directives = string.Join(',', sourceToUpdate.Directives ?? new List<string>());

            _context.Value.CspSources.Attach(sourceToUpdate.existingSource);
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

    private async Task UpdateSecurityHeaders(SecurityHeaderModel securityHeaders, string modifiedBy, DateTime modified)
    {
        var recordToSave = await _context.Value.SecurityHeaderSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new SecurityHeaderSettings();
            _context.Value.SecurityHeaderSettings.Add(recordToSave);
        }

        SecurityHeaderMapper.ToEntity(recordToSave, securityHeaders);
        recordToSave.Modified = modified;
        recordToSave.ModifiedBy = modifiedBy;
    }

    private async Task UpdatePermissionPolicySettings(IPermissionPolicySettings settings, string modifiedBy, DateTime modified)
    {
        var recordToSave = await _context.Value.PermissionPolicySettings.OrderByDescending(x => x.Modified).FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new PermissionPolicySettings();
            _context.Value.PermissionPolicySettings.Add(recordToSave);
        }

        recordToSave.IsEnabled = settings.IsEnabled;
        recordToSave.Modified = modified;
        recordToSave.ModifiedBy = modifiedBy;
    }

    private async Task UpdatePermissionsPolicyDirectives(IList<PermissionPolicyDirectiveModel>? directives, string modifiedBy, DateTime modified)
    {
        var existingDirectives = await _context.Value.PermissionPolicies.ToListAsync();

        var newDirectives = directives?.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList() ?? new List<PermissionPolicyDirectiveModel>();

        var directivesToDelete = existingDirectives.Where(x => !newDirectives.Any(y => y.Name!.Equals(x.Directive))).ToList();
        foreach (var directiveToDelete in directivesToDelete)
        {
            _context.Value.PermissionPolicies.Remove(directiveToDelete);
        }

        var directivesToAdd = newDirectives.Where(x => !existingDirectives.Any(y => x.Name!.Equals(y.Directive))).ToList();
        foreach (var directiveToAdd in directivesToAdd)
        {
            _context.Value.PermissionPolicies.Add(PermissionPolicyMapper.ToEntity(directiveToAdd, modifiedBy, modified));
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

            _context.Value.PermissionPolicies.Attach(item.existingDirective);
        }
    }

    private static void HandleRemapping(CspSettingsModel settings, bool isEnabled, string sourceName)
    {
        if (!isEnabled || settings is not { Sources.Count: >0 })
        {
            return;
        }

        var allDirectives = settings.Sources.SelectMany(x => x.Directives!).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var nonceDirectives = CspConstants.NonceDirectives.Where(allDirectives.Contains).ToList();

        var existingSource = settings.Sources.FirstOrDefault(x => x.Source == sourceName);
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