namespace Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

using Stott.Security.Optimizely.Attributes;

public enum XssProtection
{
    [SecurityHeaderValue("0")]
    None,

    [SecurityHeaderValue("1")]
    Enabled,

    [SecurityHeaderValue("1; mode=block")]
    EnabledWithBlocking
}
