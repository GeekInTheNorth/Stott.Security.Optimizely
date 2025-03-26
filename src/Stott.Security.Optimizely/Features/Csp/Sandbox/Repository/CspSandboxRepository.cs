namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

internal sealed class CspSandboxRepository : ICspSandboxRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public CspSandboxRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<SandboxModel> GetAsync()
    {
        var sandboxSettings = await _context.Value.CspSandboxes.FirstOrDefaultAsync();

        return CspSandboxMapper.ToModel(sandboxSettings);
    }

    public async Task SaveAsync(SandboxModel model, string modifiedBy)
    {
        var recordToSave = await _context.Value.CspSandboxes.FirstOrDefaultAsync();

        if (recordToSave == null)
        {
            recordToSave = new CspSandbox();
            _context.Value.CspSandboxes.Add(recordToSave);
        }

        CspSandboxMapper.ToEntity(model, recordToSave);

        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }
}