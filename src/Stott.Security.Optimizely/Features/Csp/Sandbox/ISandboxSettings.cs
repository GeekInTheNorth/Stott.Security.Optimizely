namespace Stott.Security.Optimizely.Features.Csp.Sandbox;

public interface ISandboxSettings
{
    bool IsSandboxEnabled { get; }

    bool IsAllowDownloadsEnabled { get; }

    bool IsAllowDownloadsWithoutGestureEnabled { get; }

    bool IsAllowFormsEnabled { get; }

    bool IsAllowModalsEnabled { get; }

    bool IsAllowOrientationLockEnabled { get; }

    bool IsAllowPointerLockEnabled { get; }

    bool IsAllowPopupsEnabled { get; }

    bool IsAllowPopupsToEscapeTheSandboxEnabled { get; }

    bool IsAllowPresentationEnabled { get; }

    bool IsAllowSameOriginEnabled { get; }

    bool IsAllowScriptsEnabled { get; }

    bool IsAllowStorageAccessByUserEnabled { get; }

    bool IsAllowTopNavigationEnabled { get; }

    bool IsAllowTopNavigationByUserEnabled { get; }

    bool IsAllowTopNavigationToCustomProtocolEnabled { get; }

    string? AppId { get; }

    string? HostName { get; }
}
