namespace Stott.Security.Optimizely.Features.Settings.Repository;

using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

internal sealed class CspSettingsRepository : ICspSettingsRepository
{
    private readonly ICspDataContext _context;

    public CspSettingsRepository(ICspDataContext context)
    {
        _context = context;
    }

    public async Task<CspSettings> GetAsync()
    {
        var settings = await _context.CspSettings.FirstOrDefaultAsync();

        return settings ?? new CspSettings();
    }

    public async Task SaveAsync(ICspSettings settings, string modifiedBy)
    {
        var recordToSave = await _context.CspSettings.FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CspSettings();
            _context.CspSettings.Add(recordToSave);
        }

        recordToSave.IsEnabled = settings.IsEnabled;
        recordToSave.IsReportOnly = settings.IsReportOnly;
        recordToSave.IsAllowListEnabled = settings.IsAllowListEnabled;
        recordToSave.AllowListUrl = settings.AllowListUrl;
        recordToSave.IsUpgradeInsecureRequestsEnabled = settings.IsUpgradeInsecureRequestsEnabled;
        recordToSave.IsNonceEnabled = settings.IsNonceEnabled;
        recordToSave.IsStrictDynamicEnabled = settings.IsStrictDynamicEnabled;
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.SaveChangesAsync();
    }
}