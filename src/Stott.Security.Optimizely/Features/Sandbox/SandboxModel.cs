namespace Stott.Security.Optimizely.Features.Sandbox;

public class SandboxModel
{
    public bool IsSandboxEnabled { get; set; }
    
    public bool IsAllowDownloadsEnabled { get; set; }
    
    public bool IsAllowDownloadsWithoutGestureEnabled { get; set; }
    
    public bool IsAllowFormsEnabled { get; set; }
    
    public bool IsAllowModalsEnabled { get; set; }
    
    public bool IsAllowOrientationLockEnabled { get; set; }
    
    public bool IsAllowPointerLockEnabled { get; set; }
    
    public bool IsAllowPopupsEnabled { get; set; }
    
    public bool IsAllowPopupsToEscapeTheSandboxEnabled { get; set; }
    
    public bool IsAllowPresentationEnabled { get; set; }
    
    public bool IsAllowSameOriginEnabled { get; set; }
    
    public bool IsAllowScriptsEnabled { get; set; }
    
    public bool IsAllowStorageAccessByUserEnabled { get; set; }
    
    public bool IsAllowTopNavigationEnabled { get; set; }
    
    public bool IsAllowTopNavigationByUserEnabled { get; set; }
    
    public bool IsAllowTopNavigationToCustomProtocolEnabled {  get; set; }
}