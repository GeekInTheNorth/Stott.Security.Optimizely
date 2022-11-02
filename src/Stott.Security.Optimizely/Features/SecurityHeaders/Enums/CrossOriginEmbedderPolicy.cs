namespace Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

using Stott.Security.Optimizely.Attributes;

public enum CrossOriginEmbedderPolicy
{
    [SecurityHeaderValue(null)]
    None,

    [SecurityHeaderValue("unsafe-none")]
    UnsafeNone,

    [SecurityHeaderValue("require-corp")]
    RequireCorp
}
