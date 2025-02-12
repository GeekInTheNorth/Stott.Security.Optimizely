using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

internal sealed class PermissionPolicyRepository : IPermissionPolicyRepository
{
    private readonly Lazy<ICspDataContext> _context;

    public PermissionPolicyRepository(Lazy<ICspDataContext> context)
    {
        _context = context;
    }

    public async Task<List<PermissionPolicyDirectiveModel>> GetAsync()
    {
        var data = await _context.Value.PermissionPolicies.ToListAsync();

        return data.Select(PermissionPolicyMapper.ToModel).ToList();
    }

    public async Task<List<string>> ListFragments()
    {
        var data = await _context.Value.PermissionPolicies.ToListAsync();

        return data.Select(PermissionPolicyMapper.ToPolicyFragment).ToList();
    }

    public async Task Save(SavePermissionPolicyModel model, string modifiedBy)
    {
        if (model is null) throw new ArgumentNullException(nameof(model));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        var recordToSave = await _context.Value.PermissionPolicies.FirstOrDefaultAsync(x => x.Directive == model.Name);
        if (recordToSave is null)
        {
            recordToSave = new Entities.PermissionPolicy();

            _context.Value.PermissionPolicies.Add(recordToSave);
        }

        PermissionPolicyMapper.ToEntity(model, recordToSave, modifiedBy);

        await _context.Value.SaveChangesAsync();
    }
}
