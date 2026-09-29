namespace Stott.Security.Optimizely.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

public static class StringExtensions
{
    public static IList<string> SplitByComma(this string? value)
    {
        return value?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>(0);
    }

    public static string ToLowerSource(this string? value)
    {
        if (value?.StartsWith("'sha", StringComparison.OrdinalIgnoreCase) is true)        {
            return value;
        }

        return value?.ToLower() ?? string.Empty;
    }

    public static string? GetSanitizedHostDomain(this string? hostName)
    {
        if (string.IsNullOrWhiteSpace(hostName))
        {
            return null;
        }

        var sanitized = hostName.TrimEnd('/');
        var normalized = sanitized.Contains("://") ? sanitized : $"https://{sanitized}";
        if (Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
        {
            return uri.Host + (uri.IsDefaultPort ? string.Empty : ":" + uri.Port);
        }

        return sanitized;
    }
}
