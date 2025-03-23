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

    public async Task<PermissionPolicySettingsModel> GetSettingsAsync()
    {
        var data = await _context.Value.PermissionPolicySettings.OrderByDescending(x => x.Modified).FirstOrDefaultAsync();

        return PermissionPolicyMapper.ToModel(data);
    }

    public async Task<List<PermissionPolicyDirectiveModel>> ListDirectivesAsync()
    {
        var data = await _context.Value.PermissionPolicies.ToListAsync();

        return data.Select(PermissionPolicyMapper.ToModel).ToList();
    }

    public async Task<List<string>> ListDirectiveFragments()
    {
        var data = await _context.Value.PermissionPolicies.ToListAsync();

        return data.Select(PermissionPolicyMapper.ToPolicyFragment).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
    }

    public async Task SaveDirectiveAsync(SavePermissionPolicyModel model, string modifiedBy)
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

    public async Task SaveSettingsAsync(IPermissionPolicySettings settings, string modifiedBy)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrWhiteSpace(modifiedBy)) throw new ArgumentNullException(nameof(modifiedBy));

        var data = await _context.Value.PermissionPolicySettings.OrderByDescending(x => x.Modified).FirstOrDefaultAsync();
        if (data is null)
        {
            data = new PermissionPolicySettings();

            _context.Value.PermissionPolicySettings.Add(data);
        }

        data.IsEnabled = settings.IsEnabled;
        data.Modified = DateTime.UtcNow;
        data.ModifiedBy = modifiedBy;

        await _context.Value.SaveChangesAsync();
    }
}
