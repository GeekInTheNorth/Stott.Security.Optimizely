using System;
using System.Diagnostics.CodeAnalysis;

namespace Stott.Security.Optimizely.Extensions;

internal static class GuidExtensions
{
    public static Guid? GetSanitizedSiteId(this Guid? value)
    {
        return value.IsValidGuid() ? value : null;
    }

    public static bool IsValidGuid([NotNullWhen(true)] this Guid? value)
    {
        if (value == null || value == Guid.Empty)
        {
            return false;
        }
        return true;
    }
}