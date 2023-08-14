namespace Stott.Security.Optimizely.Features.Cors.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Stott.Security.Optimizely.Entities;

public static class CorsSettingsMapper
{
    public static void MapToEntity(CorsConfiguration source, CorsSettings target)
    {
        target.IsEnabled = source.IsEnabled;
        target.AllowCredentials = source.AllowCredentials;
        target.AllowHeaders = ConcatenateHeaders(source.AllowHeaders);
        target.AllowMethods = ConcatenateMethods(source.AllowMethods);
        target.AllowOrigins = ConcatenateOrigins(source.AllowOrigins);
        target.ExposeHeaders = ConcatenateHeaders(source.ExposeHeaders);
        target.MaxAge = GetMaxRange(source.MaxAge);
    }

    public static void MapToModel(CorsSettings source, CorsConfiguration target)
    {
        var allowMethods = string.IsNullOrWhiteSpace(source.AllowMethods) ? "*" : source.AllowMethods;

        target.IsEnabled = source.IsEnabled;
        target.AllowCredentials = source.AllowCredentials;
        target.AllowMethods.IsAllowGetMethods = allowMethods.Equals("*") || allowMethods.Contains("GET", StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowHeadMethods = allowMethods.Equals("*") || allowMethods.Contains("HEAD", StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowConnectMethods = allowMethods.Equals("*") || allowMethods.Contains("CONNECT", StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowDeleteMethods = allowMethods.Equals("*") || allowMethods.Contains("DELETE", StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowOptionsMethods = allowMethods.Equals("*") || allowMethods.Contains("OPTIONS", StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowPatchMethods = allowMethods.Equals("*") || allowMethods.Contains("PATCH", StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowPostMethods = allowMethods.Equals("*") || allowMethods.Contains("POST", StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowPutMethods = allowMethods.Equals("*") || allowMethods.Contains("PUT", StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowTraceMethods = allowMethods.Equals("*") || allowMethods.Contains("TRACE", StringComparison.OrdinalIgnoreCase);
        target.AllowHeaders = SplitValues(source.AllowHeaders);
        target.AllowOrigins = SplitValues(source.AllowOrigins);
        target.ExposeHeaders = SplitValues(source.ExposeHeaders);
        target.MaxAge = GetMaxRange(GetMaxRange(source.MaxAge));
    }

    private static List<CorsConfigurationItem> SplitValues(string? values)
    {
        if (string.IsNullOrWhiteSpace(values))
        {
            return new List<CorsConfigurationItem>(0);
        }

        return values.Split(new[] { ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                     .Select(x => new CorsConfigurationItem
                     {
                         Id = Guid.NewGuid(),
                         Value = x
                     })
                     .ToList();
    }

    private static string? ConcatenateOrigins(List<CorsConfigurationItem>? origins)
    {
        if (origins is not { Count: > 0 })
        {
            return "*";
        }

        return string.Join(',', origins.Where(ValidateUrl).Select(x => x.Value));
    }

    private static bool ValidateUrl(CorsConfigurationItem? item)
    {
        if (string.IsNullOrWhiteSpace(item?.Value))
        {
            return false;
        }

        return Uri.TryCreate(item.Value, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;
    }

    private static string? ConcatenateHeaders(List<CorsConfigurationItem>? headers)
    {
        if (headers is not { Count: > 0 })
        {
            return null;
        }

        var regEx = new Regex(@"^[a-zA-Z0-9\-_]+$");
        return string.Join(',', headers.Where(x => regEx.IsMatch(x.Value)).Select(x => x.Value));
    }

    private static string? ConcatenateMethods(CorsConfigurationMethods? methods)
    {
        if (methods is null)
        {
            return null;
        }

        if (methods.IsAllowAllMethods)
        {
            return "*";
        }

        return string.Join(',', GetMethodNames(methods));
    }

    private static IEnumerable<string> GetMethodNames(CorsConfigurationMethods methods)
    {
        if (methods.IsAllowConnectMethods) yield return "CONNECT";
        if (methods.IsAllowDeleteMethods) yield return "DELETE";
        if (methods.IsAllowGetMethods) yield return "GET";
        if (methods.IsAllowHeadMethods) yield return "HEAD";
        if (methods.IsAllowOptionsMethods) yield return "OPTIONS";
        if (methods.IsAllowPatchMethods) yield return "PATCH";
        if (methods.IsAllowPostMethods) yield return "POST";
        if (methods.IsAllowPutMethods) yield return "PUT";
        if (methods.IsAllowTraceMethods) yield return "TRACE";
    }

    private static int GetMaxRange(int sourceValue)
    {
        if (sourceValue < 1)
        {
            return 1;
        }

        if (sourceValue > 7200)
        {
            return 7200;
        }

        return sourceValue;
    }
}