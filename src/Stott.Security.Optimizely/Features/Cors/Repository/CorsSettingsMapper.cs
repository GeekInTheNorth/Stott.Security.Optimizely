namespace Stott.Security.Optimizely.Features.Cors.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http;

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
        target.AllowMethods.IsAllowGetMethods = allowMethods.Equals("*") || allowMethods.Contains(HttpMethods.Get.ToString(), StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowHeadMethods = allowMethods.Equals("*") || allowMethods.Contains(HttpMethods.Head.ToString(), StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowConnectMethods = allowMethods.Equals("*") || allowMethods.Contains(HttpMethods.Connect.ToString(), StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowDeleteMethods = allowMethods.Equals("*") || allowMethods.Contains(HttpMethods.Delete.ToString(), StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowOptionsMethods = allowMethods.Equals("*") || allowMethods.Contains(HttpMethods.Options.ToString(), StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowPatchMethods = allowMethods.Equals("*") || allowMethods.Contains(HttpMethods.Patch.ToString(), StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowPostMethods = allowMethods.Equals("*") || allowMethods.Contains(HttpMethods.Post.ToString(), StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowPutMethods = allowMethods.Equals("*") || allowMethods.Contains(HttpMethods.Put.ToString(), StringComparison.OrdinalIgnoreCase);
        target.AllowMethods.IsAllowTraceMethods = allowMethods.Equals("*") || allowMethods.Contains(HttpMethods.Trace.ToString(), StringComparison.OrdinalIgnoreCase);
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

        return Uri.TryCreate(item.Value, UriKind.Absolute, out var _) && Uri.IsWellFormedUriString(item.Value, UriKind.Absolute);
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
        if (methods.IsAllowConnectMethods) yield return HttpMethods.Connect.ToString();
        if (methods.IsAllowDeleteMethods) yield return HttpMethods.Delete.ToString();
        if (methods.IsAllowGetMethods) yield return HttpMethods.Get.ToString();
        if (methods.IsAllowHeadMethods) yield return HttpMethods.Head.ToString();
        if (methods.IsAllowOptionsMethods) yield return HttpMethods.Options.ToString();
        if (methods.IsAllowPatchMethods) yield return HttpMethods.Patch.ToString();
        if (methods.IsAllowPostMethods) yield return HttpMethods.Post.ToString();
        if (methods.IsAllowPutMethods) yield return HttpMethods.Put.ToString();
        if (methods.IsAllowTraceMethods) yield return HttpMethods.Trace.ToString();
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