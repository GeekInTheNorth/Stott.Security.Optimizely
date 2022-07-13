using Stott.Security.Core.Attributes;

namespace Stott.Security.Core.Features.SecurityHeaders.Enums;

public enum XFrameOptions
{
    [SecurityHeaderValue(null)]
    None,

    [SecurityHeaderValue("SAMEORIGIN")]
    SameOrigin,

    [SecurityHeaderValue("DENY")]
    Deny
}
