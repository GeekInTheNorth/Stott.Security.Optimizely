namespace Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

using Stott.Security.Optimizely.Attributes;

public enum XContentTypeOptions
{
    [SecurityHeaderValue(null)]
    None,

    [SecurityHeaderValue("nosniff")]
    NoSniff
}
