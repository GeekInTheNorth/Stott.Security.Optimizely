namespace Stott.Security.Optimizely.Features.Csp.Settings;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Stott.Security.Optimizely.Features.Csp.AllowList;
using Stott.Security.Optimizely.Features.Csp.Reporting.Service;

public sealed class CspSettingsModel : IValidatableObject, ICspSettings
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string ScopeType => ScopeSiteId.HasValue && Guid.Empty != ScopeSiteId.Value ? "Site" : "Global";

    public Guid? ScopeSiteId { get; set; }

    public string? ScopeBehaviour { get; set; }

    public string? ScopeName { get; set; }

    public string[]? ScopePaths { get; set; }

    public string[]? ScopeExclusions { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsReportOnly { get; set; }

    public bool UseInternalReporting { get; set; }

    public bool UseExternalReporting { get; set; }

    public string? ExternalReportToUrl { get; set; }

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

        if (UseExternalReporting)
        {
            var validationService = validationContext.GetService(typeof(IReportingEndpointValidator)) as IReportingEndpointValidator;

            if (!string.IsNullOrWhiteSpace(ExternalReportToUrl) && !validationService!.IsValidReportToEndPoint(ExternalReportToUrl, out var reportToError))
            {
                yield return new ValidationResult(reportToError, new[] { nameof(ExternalReportToUrl) });
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