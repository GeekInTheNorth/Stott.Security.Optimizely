using Stott.Security.Core.Attributes;

namespace Stott.Security.Core.Features.SecurityHeaders.Enums;

public enum CrossOriginEmbedderPolicy
{
    [SecurityHeaderValue(null)]
    None,

    [SecurityHeaderValue("unsafe-none")]
    UnsafeNone,

    [SecurityHeaderValue("require-corp")]
    RequireCorp
}
