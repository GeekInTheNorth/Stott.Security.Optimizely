namespace Stott.Security.Optimizely.Features.SecurityHeaders.Repository;

using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

internal sealed class SecurityHeaderRepository : ISecurityHeaderRepository
{
    private readonly ICspDataContext _context;

    public SecurityHeaderRepository(ICspDataContext context)
    {
        _context = context;
    }

    public async Task<SecurityHeaderSettings> GetAsync()
    {
        var settings = await _context.SecurityHeaderSettings.FirstOrDefaultAsync();

        return settings ?? new SecurityHeaderSettings();
    }

    public async Task SaveAsync(SecurityHeaderSettings settingsToSave)
    {
        if (settingsToSave == null) 
        { 
            return; 
        }

        _context.SecurityHeaderSettings.Attach(settingsToSave);
        await _context.SaveChangesAsync();
    }
}