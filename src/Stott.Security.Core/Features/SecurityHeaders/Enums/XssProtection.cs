using Stott.Security.Core.Attributes;

namespace Stott.Security.Core.Features.SecurityHeaders.Enums;

public enum XssProtection
{
    [SecurityHeaderValue("0")]
    None,

    [SecurityHeaderValue("1")]
    Enabled,

    [SecurityHeaderValue("1; mode=block")]
    EnabledWithBlocking
}
