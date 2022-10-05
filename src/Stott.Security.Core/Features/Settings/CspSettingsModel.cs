namespace Stott.Security.Core.Features.Settings;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class CspSettingsModel : IValidatableObject
{
    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    public bool IsWhitelistEnabled { get; set; }

    public string WhitelistAddress { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IsWhitelistEnabled)
        {
            if (string.IsNullOrEmpty(WhitelistAddress))
            {
                yield return new ValidationResult($"Whitelist Address has not been defined.", new[] { nameof(WhitelistAddress) });
            }
            else if (Uri.IsWellFormedUriString(WhitelistAddress, UriKind.Absolute))
            {
                yield return new ValidationResult($"Whitelist Address is not a valid URI.", new[] { nameof(WhitelistAddress) });
            }
        }
    }
}
