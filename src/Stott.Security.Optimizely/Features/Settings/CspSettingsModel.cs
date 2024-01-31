namespace Stott.Security.Optimizely.Features.Settings;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Stott.Security.Optimizely.Features.AllowList;

public sealed class CspSettingsModel : IValidatableObject, ICspSettings
{
    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    public bool IsAllowListEnabled { get; set; }

    public string? AllowListUrl { get; set; }

    public bool IsUpgradeInsecureRequestsEnabled { get; set; }
    
    public bool IsNonceEnabled { get; set; }
    
    public bool IsStrictDynamicEnabled { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (IsAllowListEnabled)
        {
            if (string.IsNullOrWhiteSpace(AllowListUrl))
            {
                yield return new ValidationResult($"Allow List Address has not been defined.", new[] { nameof(AllowListUrl) });
            }
            else if (!Uri.IsWellFormedUriString(AllowListUrl, UriKind.Absolute))
            {
                yield return new ValidationResult($"Allow List Address is not a valid URI.", new[] { nameof(AllowListUrl) });
            }
            else if (!IsAllowListUrlValid(validationContext))
            {
                yield return new ValidationResult($"Allow List Address does not provide a valid response.", new[] { nameof(AllowListUrl) });
            }
        }
    }

    private bool IsAllowListUrlValid(ValidationContext validationContext)
    {
        var allowListService = validationContext.GetService(typeof(IAllowListService)) as IAllowListService;
        var validationTask = allowListService?.IsAllowListValidAsync(AllowListUrl);

        return validationTask?.Result ?? false;
    }
}