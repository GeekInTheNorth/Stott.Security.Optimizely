namespace Stott.Security.Optimizely.Features.SecurityHeaders;

public sealed class SecurityHeaderModel
{
    public string? XContentTypeOptions { get; set; }

    public string? XXssProtection { get; set; }

    public string? XFrameOptions { get; set; }

    public string? ReferrerPolicy { get; set; }

    public string? CrossOriginEmbedderPolicy { get; set; }
    
    public string? CrossOriginOpenerPolicy { get; set; }
    
    public string? CrossOriginResourcePolicy { get; set; }

    public bool IsStrictTransportSecurityEnabled { get; set; }
    
    public bool IsStrictTransportSecuritySubDomainsEnabled { get; set; }
    
    public int StrictTransportSecurityMaxAge { get; set; }
    
    public bool ForceHttpRedirect { get; set; }
}