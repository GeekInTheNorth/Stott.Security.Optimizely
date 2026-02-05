using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Stott.Security.Optimizely.Features.Cors;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class SettingsModel : IValidatableObject
{
    public CspSettingsModel? Csp { get; set; }

    public CorsConfiguration? Cors { get; set; }

    public PermissionPolicyModel? PermissionPolicy { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext? validationContext)
    {
        if (Csp is not null && Csp.Sandbox is null)
        {
            yield return new ValidationResult($"{nameof(Csp)}.{nameof(Csp.Sandbox)} has not been defined.");
        }

        if (Csp is not null && Csp.Sources is null)
        {
            yield return new ValidationResult($"{nameof(Csp)}.{nameof(Csp.Sources)} has not been defined.");
        }
    }

    public IEnumerable<string> GetSettingsToUpdate()
    {
        if (Csp is not null)
        {
            yield return "CSP";
        }

        if (Cors is not null)
        {
            yield return "CORS";
        }

        if (PermissionPolicy is not null)
        {
            yield return "Permission Policy";
        }
    }
}