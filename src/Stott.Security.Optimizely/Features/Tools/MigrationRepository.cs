using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Cors.Repository;
using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Features.Sandbox.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;
using Stott.Security.Optimizely.Features.Settings;
using Stott.Security.Optimizely.Features.Settings.Repository;

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
        await UpdateCspSettings(settings.Csp, modifiedBy, modifiedDate);
        await UpdateCspSandbox(settings.Csp?.Sandbox, modifiedBy, modifiedDate);
        await UpdateCspSources(settings.Csp?.Sources, modifiedBy, modifiedDate);
        await UpdateCors(settings.Cors, modifiedBy, modifiedDate);
        await UpdateSecurityHeaders(settings.Headers, modifiedBy, modifiedDate);

        await _context.Value.SaveChangesAsync();
    }

    private async Task UpdateCspSettings(ICspSettings? settings, string modifiedBy, DateTime modified)
    {
        if (settings is null)
        {
            return;
        }

        var settingsToUpdate = await _context.Value.CspSettings.FirstOrDefaultAsync();
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

    private async Task UpdateCors(CorsConfiguration? corsConfiguration, string modifiedBy, DateTime modified)
    {
        if (corsConfiguration is null)
        {
            return;
        }

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

    private async Task UpdateSecurityHeaders(SecurityHeaderModel? securityHeaders, string modifiedBy, DateTime modified)
    {
        if (securityHeaders is null)
        {
            return;
        }

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
}