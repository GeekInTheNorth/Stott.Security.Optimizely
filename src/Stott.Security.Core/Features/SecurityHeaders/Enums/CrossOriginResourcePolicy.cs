using Stott.Security.Core.Attributes;

namespace Stott.Security.Core.Features.SecurityHeaders.Enums;

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
