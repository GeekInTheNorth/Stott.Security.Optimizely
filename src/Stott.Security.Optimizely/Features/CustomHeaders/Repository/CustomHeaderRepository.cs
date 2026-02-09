namespace Stott.Security.Optimizely.Features.CustomHeaders.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

/// <summary>
/// Repository implementation for custom header data access.
/// </summary>
internal sealed class CustomHeaderRepository : ICustomHeaderRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public CustomHeaderRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CustomHeader>> GetAllAsync()
    {
        return await _context.Value.CustomHeaders.OrderBy(x => x.HeaderName).ToListAsync();
    }

    public async Task<CustomHeader?> GetByIdAsync(Guid id)
    {
        return await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<CustomHeader?> GetByHeaderNameAsync(string headerName)
    {
        if (string.IsNullOrWhiteSpace(headerName))
        {
            return null;
        }

        return await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.HeaderName == headerName);
    }

    public async Task SaveAsync(ICustomHeader model, string modifiedBy)
    {
        CustomHeader? recordToSave = null;

        if (!Guid.Empty.Equals(model.Id))
        {
            recordToSave = await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.Id == model.Id);
        }

        if (recordToSave is null && !string.IsNullOrWhiteSpace(model.HeaderName))
        {
            recordToSave = await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.HeaderName == model.HeaderName);
        }

        if (recordToSave is null)
        {
            recordToSave = new CustomHeader
            {
                Id = Guid.NewGuid()
            };
            _context.Value.CustomHeaders.Add(recordToSave);
        }

        CustomHeaderMapper.ToEntity(model, recordToSave);
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var recordToDelete = await _context.Value.CustomHeaders.FirstOrDefaultAsync(x => x.Id == id);
        if (recordToDelete is not null)
        {
            _context.Value.CustomHeaders.Remove(recordToDelete);
            await _context.Value.SaveChangesAsync();
        }
    }
}
