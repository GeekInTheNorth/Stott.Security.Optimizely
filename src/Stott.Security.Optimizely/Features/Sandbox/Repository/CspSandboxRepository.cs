namespace Stott.Security.Optimizely.Features.Sandbox.Repository;

using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

internal sealed class CspSandboxRepository : ICspSandboxRepository
{
    private readonly ICspDataContext _context;

    public CspSandboxRepository(ICspDataContext context)
    {
        _context = context;
    }

    public async Task<SandboxModel> GetAsync()
    {
        var sandboxSettings = await _context.CspSandboxes.FirstOrDefaultAsync();

        if (sandboxSettings == null)
        {
            return new SandboxModel();
        }

        return new SandboxModel
        {
            IsSandboxEnabled = sandboxSettings.IsSandboxEnabled,
            IsAllowDownloadsEnabled = sandboxSettings.IsAllowDownloadsEnabled,
            IsAllowDownloadsWithoutGestureEnabled = sandboxSettings.IsAllowDownloadsWithoutGestureEnabled,
            IsAllowFormsEnabled = sandboxSettings.IsAllowFormsEnabled,
            IsAllowModalsEnabled = sandboxSettings.IsAllowModalsEnabled,
            IsAllowOrientationLockEnabled = sandboxSettings.IsAllowOrientationLockEnabled,
            IsAllowPointerLockEnabled = sandboxSettings.IsAllowPointerLockEnabled,
            IsAllowPopupsEnabled = sandboxSettings.IsAllowPopupsEnabled,
            IsAllowPopupsToEscapeTheSandboxEnabled = sandboxSettings.IsAllowPopupsToEscapeTheSandboxEnabled,
            IsAllowPresentationEnabled = sandboxSettings.IsAllowPresentationEnabled,
            IsAllowSameOriginEnabled = sandboxSettings.IsAllowSameOriginEnabled,
            IsAllowScriptsEnabled = sandboxSettings.IsAllowScriptsEnabled,
            IsAllowStorageAccessByUserEnabled = sandboxSettings.IsAllowStorageAccessByUserEnabled,
            IsAllowTopNavigationEnabled = sandboxSettings.IsAllowTopNavigationEnabled,
            IsAllowTopNavigationByUserEnabled = sandboxSettings.IsAllowTopNavigationByUserEnabled,
            IsAllowTopNavigationToCustomProtocolEnabled = sandboxSettings.IsAllowTopNavigationToCustomProtocolEnabled
        };
    }

    public async Task SaveAsync(SandboxModel model, string modifiedBy)
    {
        var recordToSave = await _context.CspSandboxes.FirstOrDefaultAsync();

        if (recordToSave == null)
        {
            recordToSave = new CspSandbox();
            _context.CspSandboxes.Add(recordToSave);
        }

        recordToSave.IsSandboxEnabled = model.IsSandboxEnabled;
        recordToSave.IsAllowDownloadsEnabled = model.IsAllowDownloadsEnabled;
        recordToSave.IsAllowDownloadsWithoutGestureEnabled = model.IsAllowDownloadsWithoutGestureEnabled;
        recordToSave.IsAllowFormsEnabled = model.IsAllowFormsEnabled;
        recordToSave.IsAllowModalsEnabled = model.IsAllowModalsEnabled;
        recordToSave.IsAllowOrientationLockEnabled = model.IsAllowOrientationLockEnabled;
        recordToSave.IsAllowPointerLockEnabled = model.IsAllowPointerLockEnabled;
        recordToSave.IsAllowPopupsEnabled = model.IsAllowPopupsEnabled;
        recordToSave.IsAllowPopupsToEscapeTheSandboxEnabled = model.IsAllowPopupsToEscapeTheSandboxEnabled;
        recordToSave.IsAllowPresentationEnabled = model.IsAllowPresentationEnabled;
        recordToSave.IsAllowSameOriginEnabled = model.IsAllowSameOriginEnabled;
        recordToSave.IsAllowScriptsEnabled = model.IsAllowScriptsEnabled;
        recordToSave.IsAllowStorageAccessByUserEnabled = model.IsAllowStorageAccessByUserEnabled;
        recordToSave.IsAllowTopNavigationEnabled = model.IsAllowTopNavigationEnabled;
        recordToSave.IsAllowTopNavigationByUserEnabled = model.IsAllowTopNavigationByUserEnabled;
        recordToSave.IsAllowTopNavigationToCustomProtocolEnabled = model.IsAllowTopNavigationToCustomProtocolEnabled;
        recordToSave.Modified = DateTime.UtcNow;
        recordToSave.ModifiedBy = modifiedBy;

        await _context.SaveChangesAsync();
    }
}