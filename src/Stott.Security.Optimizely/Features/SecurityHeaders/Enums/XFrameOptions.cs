namespace Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

using Stott.Security.Optimizely.Attributes;

public enum XFrameOptions
{
    [SecurityHeaderValue(null)]
    None,

    [SecurityHeaderValue("SAMEORIGIN")]
    SameOrigin,

    [SecurityHeaderValue("DENY")]
    Deny
}
