namespace Stott.Security.Optimizely.Features.Cors.Repository;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

public sealed class CorsSettingsRepository : ICorsSettingsRepository
{
    private readonly ICspDataContext _context;

    public CorsSettingsRepository(ICspDataContext context)
    {
        _context = context;
    }

    public async Task<CorsConfiguration> GetAsync()
    {
        var model = new CorsConfiguration();
        var settings = await _context.CorsSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
        if (settings != null)
        {
            CorsSettingsMapper.MapToModel(settings, model);
        }

        return model;
    }

    public async Task SaveAsync(CorsConfiguration? model, string? modifiedBy)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentNullException(nameof(modifiedBy));
        }

        var recordToSave = await _context.CorsSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CorsSettings();
            _context.CorsSettings.Add(recordToSave);
        }

        CorsSettingsMapper.MapToEntity(model, recordToSave);
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.SaveChangesAsync();
    }
}