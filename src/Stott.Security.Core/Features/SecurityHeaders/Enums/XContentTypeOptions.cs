using Stott.Security.Core.Attributes;

namespace Stott.Security.Core.Features.SecurityHeaders.Enums;

public enum XContentTypeOptions
{
    [SecurityHeaderValue(null)]
    None,

    [SecurityHeaderValue("nosniff")]
    NoSniff
}
