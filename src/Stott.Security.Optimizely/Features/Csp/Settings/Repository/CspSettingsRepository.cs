namespace Stott.Security.Optimizely.Features.Csp.Settings.Repository;

using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

internal sealed class CspSettingsRepository : ICspSettingsRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public CspSettingsRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<CspSettings> GetAsync()
    {
        var settings = await _context.Value.CspSettings.FirstOrDefaultAsync();

        return settings ?? new CspSettings();
    }

    public async Task SaveAsync(ICspSettings settings, string modifiedBy)
    {
        var recordToSave = await _context.Value.CspSettings.FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CspSettings();
            _context.Value.CspSettings.Add(recordToSave);
        }

        CspSettingsMapper.ToEntity(settings, recordToSave);
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }
}