namespace Stott.Security.Optimizely.Features.Settings.Repository;

using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

public class CspSettingsRepository : ICspSettingsRepository
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

    public async Task SaveAsync(
        bool isEnabled, 
        bool isReportOnly, 
        bool isWhitelistEnabled, 
        string whitelistUrl, 
        string modifiedBy)
    {
        var recordToSave = await _context.CspSettings.FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CspSettings();
            _context.CspSettings.Add(recordToSave);
        }

        recordToSave.IsEnabled = isEnabled;
        recordToSave.IsReportOnly = isReportOnly;
        recordToSave.IsWhitelistEnabled = isWhitelistEnabled;
        recordToSave.WhitelistUrl = whitelistUrl;
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.SaveChangesAsync();
    }
}
