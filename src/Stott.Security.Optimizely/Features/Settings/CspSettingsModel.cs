namespace Stott.Security.Optimizely.Features.Settings;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Stott.Security.Optimizely.Features.Whitelist;

public sealed class CspSettingsModel : IValidatableObject, ICspSettings
{
    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    public bool IsWhitelistEnabled { get; set; }

    public string? WhitelistUrl { get; set; }

    public bool IsUpgradeInsecureRequestsEnabled { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IsWhitelistEnabled)
        {
            if (string.IsNullOrWhiteSpace(WhitelistUrl))
            {
                yield return new ValidationResult($"Whitelist Address has not been defined.", new[] { nameof(WhitelistUrl) });
            }
            else if (!Uri.IsWellFormedUriString(WhitelistUrl, UriKind.Absolute))
            {
                yield return new ValidationResult($"Whitelist Address is not a valid URI.", new[] { nameof(WhitelistUrl) });
            }
            else if (!IsWhitelistUrlValid(validationContext))
            {
                yield return new ValidationResult($"Whitelist Address does not provide a valid response.", new[] { nameof(WhitelistUrl) });
            }
        }
    }

    private bool IsWhitelistUrlValid(ValidationContext validationContext)
    {
        var whitelistService = validationContext.GetService(typeof(IWhitelistService)) as IWhitelistService;
        var validationTask = whitelistService?.IsWhitelistValidAsync(WhitelistUrl);

        return validationTask?.Result ?? false;
    }
}