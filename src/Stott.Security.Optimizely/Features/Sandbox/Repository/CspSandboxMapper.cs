using Stott.Security.Optimizely.Entities;

namespace Stott.Security.Optimizely.Features.Sandbox.Repository;

internal static class CspSandboxMapper
{
    internal static SandboxModel ToModel(CspSandbox? sandboxEntity)
    {
        if (sandboxEntity is null)
        {
            return new SandboxModel();
        }

        return new SandboxModel
        {
            IsSandboxEnabled = sandboxEntity.IsSandboxEnabled,
            IsAllowDownloadsEnabled = sandboxEntity.IsAllowDownloadsEnabled,
            IsAllowDownloadsWithoutGestureEnabled = sandboxEntity.IsAllowDownloadsWithoutGestureEnabled,
            IsAllowFormsEnabled = sandboxEntity.IsAllowFormsEnabled,
            IsAllowModalsEnabled = sandboxEntity.IsAllowModalsEnabled,
            IsAllowOrientationLockEnabled = sandboxEntity.IsAllowOrientationLockEnabled,
            IsAllowPointerLockEnabled = sandboxEntity.IsAllowPointerLockEnabled,
            IsAllowPopupsEnabled = sandboxEntity.IsAllowPopupsEnabled,
            IsAllowPopupsToEscapeTheSandboxEnabled = sandboxEntity.IsAllowPopupsToEscapeTheSandboxEnabled,
            IsAllowPresentationEnabled = sandboxEntity.IsAllowPresentationEnabled,
            IsAllowSameOriginEnabled = sandboxEntity.IsAllowSameOriginEnabled,
            IsAllowScriptsEnabled = sandboxEntity.IsAllowScriptsEnabled,
            IsAllowStorageAccessByUserEnabled = sandboxEntity.IsAllowStorageAccessByUserEnabled,
            IsAllowTopNavigationEnabled = sandboxEntity.IsAllowTopNavigationEnabled,
            IsAllowTopNavigationByUserEnabled = sandboxEntity.IsAllowTopNavigationByUserEnabled,
            IsAllowTopNavigationToCustomProtocolEnabled = sandboxEntity.IsAllowTopNavigationToCustomProtocolEnabled
        };
    }

    internal static void ToEntity(SandboxModel? model, CspSandbox sandboxEntity)
    {
        if (model is null)
        {
            return;
        }

        sandboxEntity.IsSandboxEnabled = model.IsSandboxEnabled;
        sandboxEntity.IsAllowDownloadsEnabled = model.IsAllowDownloadsEnabled;
        sandboxEntity.IsAllowDownloadsWithoutGestureEnabled = model.IsAllowDownloadsWithoutGestureEnabled;
        sandboxEntity.IsAllowFormsEnabled = model.IsAllowFormsEnabled;
        sandboxEntity.IsAllowModalsEnabled = model.IsAllowModalsEnabled;
        sandboxEntity.IsAllowOrientationLockEnabled = model.IsAllowOrientationLockEnabled;
        sandboxEntity.IsAllowPointerLockEnabled = model.IsAllowPointerLockEnabled;
        sandboxEntity.IsAllowPopupsEnabled = model.IsAllowPopupsEnabled;
        sandboxEntity.IsAllowPopupsToEscapeTheSandboxEnabled = model.IsAllowPopupsToEscapeTheSandboxEnabled;
        sandboxEntity.IsAllowPresentationEnabled = model.IsAllowPresentationEnabled;
        sandboxEntity.IsAllowSameOriginEnabled = model.IsAllowSameOriginEnabled;
        sandboxEntity.IsAllowScriptsEnabled = model.IsAllowScriptsEnabled;
        sandboxEntity.IsAllowStorageAccessByUserEnabled = model.IsAllowStorageAccessByUserEnabled;
        sandboxEntity.IsAllowTopNavigationEnabled = model.IsAllowTopNavigationEnabled;
        sandboxEntity.IsAllowTopNavigationByUserEnabled = model.IsAllowTopNavigationByUserEnabled;
        sandboxEntity.IsAllowTopNavigationToCustomProtocolEnabled = model.IsAllowTopNavigationToCustomProtocolEnabled;
    }
}