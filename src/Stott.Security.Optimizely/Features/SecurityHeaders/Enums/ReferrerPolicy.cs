namespace Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

using Stott.Security.Optimizely.Attributes;

public enum ReferrerPolicy
{
    [SecurityHeaderValue(null)]
    None,

    [SecurityHeaderValue("no-referrer")] 
    NoReferrer,
    
    [SecurityHeaderValue("no-referrer-when-downgrade")] 
    NoReferrerWhenDowngrade,

    [SecurityHeaderValue("origin")] 
    Origin,

    [SecurityHeaderValue("origin-when-cross-origin")] 
    OriginWhenCrossOrigin,

    [SecurityHeaderValue("same-origin")] 
    SameOrigin,

    [SecurityHeaderValue("strict-origin")] 
    StrictOrigin,

    [SecurityHeaderValue("strict-origin-when-cross-origin")] 
    StrictOriginWhenCrossOrigin,

    [SecurityHeaderValue("unsafe-url")] 
    UnsafeUrl
}
