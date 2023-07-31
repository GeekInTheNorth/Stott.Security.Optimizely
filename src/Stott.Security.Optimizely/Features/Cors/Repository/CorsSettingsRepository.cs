namespace Stott.Security.Optimizely.Features.Cors.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Stott.Security.Optimizely.Entities;

public class CorsSettingsRepository : ICorsSettingsRepository
{
    private readonly ICspDataContext _context;

    public CorsSettingsRepository(ICspDataContext context)
    {
        _context = context;
    }

    public async Task<CorsConfiguration> GetAsync()
    {
        var settings = await _context.CorsSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
        if (settings == null)
        {
            return new CorsConfiguration();
        }

        var allowMethods = string.IsNullOrWhiteSpace(settings.AllowMethods) ? "*" : settings.AllowMethods;

        return new CorsConfiguration
        {
            IsEnabled = settings.IsEnabled,
            AllowCredentials = settings.AllowCredentials,
            AllowMethods = new CorsConfigurationMethods
            {
                IsAllowGetMethods = allowMethods.Equals("*") || allowMethods.Contains("GET"),
                IsAllowHeadMethods = allowMethods.Equals("*") || allowMethods.Contains("HEAD"),
                IsAllowConnectMethods = allowMethods.Equals("*") || allowMethods.Contains("CONNECT"),
                IsAllowDeleteMethods = allowMethods.Equals("*") || allowMethods.Contains("DELETE"),
                IsAllowOptionsMethods = allowMethods.Equals("*") || allowMethods.Contains("OPTIONS"),
                IsAllowPatchMethods = allowMethods.Equals("*") || allowMethods.Contains("PATCH"),
                IsAllowPostMethods = allowMethods.Equals("*") || allowMethods.Contains("POST"),
                IsAllowPutMethods = allowMethods.Equals("*") || allowMethods.Contains("PUT"),
                IsAllowTraceMethods = allowMethods.Equals("*") || allowMethods.Contains("TRACE"),
            },
            AllowHeaders = SplitValues(settings.AllowHeaders),
            AllowOrigins = SplitValues(settings.AllowOrigins),
            ExposeHeaders = SplitValues(settings.ExposeHeaders),
            MaxAge = settings.MaxAge,
        };
    }

    public async Task SaveAsync(CorsConfiguration? model, string? modifiedBy)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentNullException(nameof(modifiedBy));
        }

        var recordToSave = await _context.CorsSettings.OrderBy(x => x.Id).FirstOrDefaultAsync();
        if (recordToSave == null)
        {
            recordToSave = new CorsSettings();
            _context.CorsSettings.Add(recordToSave);
        }

        recordToSave.IsEnabled = model.IsEnabled;
        recordToSave.AllowCredentials = model.AllowCredentials;
        recordToSave.AllowHeaders = ConcatenateHeaders(model.AllowHeaders);
        recordToSave.AllowMethods = ConcatenateMethods(model.AllowMethods);
        recordToSave.AllowOrigins = ConcatenateOrigins(model.AllowOrigins);
        recordToSave.ExposeHeaders = ConcatenateHeaders(model.ExposeHeaders);
        recordToSave.MaxAge = model.MaxAge;

        await _context.SaveChangesAsync();
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
        if (origins is not {  Count: > 0 })
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
        if (headers is not {  Count: > 0 })
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
}