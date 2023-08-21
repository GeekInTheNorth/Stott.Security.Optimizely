namespace Stott.Security.Optimizely.Features.Cors;

public sealed class CorsConfigurationMethods
{
    public bool IsAllowAllMethods => 
        IsAllowGetMethods && 
        IsAllowHeadMethods &&
        IsAllowPostMethods &&
        IsAllowPutMethods &&
        IsAllowPatchMethods &&
        IsAllowDeleteMethods &&
        IsAllowConnectMethods &&
        IsAllowOptionsMethods &&
        IsAllowTraceMethods;

    public bool IsAllowGetMethods { get; set; }

    public bool IsAllowHeadMethods { get; set; }

    public bool IsAllowPostMethods { get; set; }

    public bool IsAllowPutMethods { get; set; }

    public bool IsAllowPatchMethods { get; set; }

    public bool IsAllowDeleteMethods { get; set; }

    public bool IsAllowConnectMethods { get; set; }

    public bool IsAllowOptionsMethods { get; set; }

    public bool IsAllowTraceMethods { get; set; }
}