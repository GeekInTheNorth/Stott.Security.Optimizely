using Stott.Security.Core.Attributes;

namespace Stott.Security.Core.Features.SecurityHeaders.Enums;

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
