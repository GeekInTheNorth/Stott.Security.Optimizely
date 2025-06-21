namespace Stott.Security.Optimizely.Features.Cors;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

public sealed class CorsConfiguration : IValidatableObject
{
    public bool IsEnabled { get; set; }

    public CorsConfigurationMethods AllowMethods { get; set; } = new();

    public List<CorsConfigurationItem> AllowOrigins { get; set; } = new();

    public List<CorsConfigurationItem> AllowHeaders { get; set; } = new();

    public List<CorsConfigurationItem> ExposeHeaders { get; set; } = new();

    public bool AllowCredentials { get; set; }

    [Range(1, 7200, ErrorMessage = "Max Age should have a value between 1 second and 2 hours.")]
    public int MaxAge { get; set; } = 1;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var headerRegEx = new Regex(@"^[a-zA-Z0-9\-_]+$");

        foreach(var origin in AllowOrigins)
        {
            if (!Uri.IsWellFormedUriString(origin.Value, UriKind.Absolute) || 
                !Uri.TryCreate(origin.Value, new UriCreationOptions(), out var uri) ||
                uri is { PathAndQuery.Length: >1 })
            {
                yield return new ValidationResult("Allow Origins contains one or more invalid values", new List<string> { nameof(AllowOrigins) });

                break;
            }
        }

        foreach (var header in AllowHeaders)
        {
            if (!headerRegEx.IsMatch(header.Value))
            {
                yield return new ValidationResult("Allow Headers contains one or more invalid values", new List<string> { nameof(AllowHeaders) });

                break;
            }
        }

        foreach (var header in ExposeHeaders)
        {
            if (!headerRegEx.IsMatch(header.Value))
            {
                yield return new ValidationResult("Expose Headers contains one or more invalid values", new List<string> { nameof(ExposeHeaders) });

                break;
            }
        }

        if (AllowCredentials && IsAnyOriginAllowed())
        {
            yield return new ValidationResult("Allow Credentials cannot be set to true when any origin is allowed.", new List<string> { nameof(AllowCredentials) });
        }
    }

    private bool IsAnyOriginAllowed()
    {
        return AllowOrigins.Count == 0 || AllowOrigins.Any(x => x.Value == "*");
    }
}