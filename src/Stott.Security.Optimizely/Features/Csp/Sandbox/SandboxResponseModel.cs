namespace Stott.Security.Optimizely.Features.Csp.Sandbox;

public sealed class SandboxResponseModel : SandboxModel, ISandboxSettings
{
    public bool IsInherited { get; set; }
}
