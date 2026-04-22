using System;

namespace Stott.Security.Optimizely.Extensions;

internal static class UriExtensions
{
    internal static string? GetSanitizedHostDomain(this Uri? uri)
    {
        if (uri is null)
        {
            return null;
        }

        if (!uri.IsAbsoluteUri)
        {
            return null;
        }

        return uri.Host + (uri.IsDefaultPort ? string.Empty : ":" + uri.Port);
    }
}
