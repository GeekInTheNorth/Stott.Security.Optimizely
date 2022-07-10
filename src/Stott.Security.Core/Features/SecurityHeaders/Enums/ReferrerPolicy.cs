namespace Stott.Security.Core.Features.SecurityHeaders.Enums;

public enum ReferrerPolicy
{
    None,
    NoReferrer,
    NoReferrerWhenDowngrade,
    Origin,
    OriginWhenCrossOrigin,
    SameOrigin,
    StrictOrigin,
    StrictOriginWhenCrossOrigin,
    UnsafeUrl
}
