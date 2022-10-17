namespace Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

using Stott.Security.Optimizely.Attributes;

public enum CrossOriginResourcePolicy
{
    [SecurityHeaderValue(null)]
    None,

    [SecurityHeaderValue("same-site")]
    SameSite,

    [SecurityHeaderValue("same-origin")]
    SameOrigin,

    [SecurityHeaderValue("cross-origin")]
    CrossOrigin
}
