namespace Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

using Stott.Security.Optimizely.Attributes;

public enum CrossOriginOpenerPolicy
{
    [SecurityHeaderValue(null)]
    None,

    [SecurityHeaderValue("unsafe-none")]
    UnsafeNone,

    [SecurityHeaderValue("same-origin")]
    SameOrigin,

    [SecurityHeaderValue("same-origin-allow-popups")]
    SameOriginAllowPopups
}
