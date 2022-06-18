namespace Stott.Security.Core.Features.Settings.Repository;

using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Core.Entities;

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

    public async Task SaveAsync(bool isEnabled, bool isReportOnly)
    {
        var recordToSave = await _context.CspSettings.FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CspSettings();
            _context.CspSettings.Add(recordToSave);
        }

        recordToSave.IsEnabled = isEnabled;
        recordToSave.IsReportOnly = isReportOnly;

        await _context.SaveChangesAsync();
    }
}
