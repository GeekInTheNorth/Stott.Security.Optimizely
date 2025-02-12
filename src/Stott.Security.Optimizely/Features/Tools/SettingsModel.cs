using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.SecurityHeaders;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class SettingsModel : IValidatableObject
{
    public CspSettingsModel? Csp { get; set; }

    public CorsConfiguration? Cors { get; set; }

    public SecurityHeaderModel? Headers { get; set; }

    public List<PermissionPolicyDirectiveModel>? PermissionPolicy { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext? validationContext)
    {
        if (Csp is null)
        {
            yield return new ValidationResult($"{nameof(Csp)} has not been defined.");
        }

        if (Csp?.Sandbox is null)
        {
            yield return new ValidationResult($"{nameof(Csp)}.{nameof(Csp.Sandbox)} has not been defined.");
        }

        if (Csp?.Sources is null)
        {
            yield return new ValidationResult($"{nameof(Csp)}.{nameof(Csp.Sources)} has not been defined.");
        }

        if (Cors is null)
        {
            yield return new ValidationResult($"{nameof(Cors)} has not been defined.");
        }

        if (PermissionPolicy is null)
        {
            yield return new ValidationResult($"{nameof(PermissionPolicy)} has not been defined.");
        }

        if (Headers is null)
        {
            yield return new ValidationResult($"{nameof(Headers)} has not been defined.");
            yield break;
        }

        if (!Enum.TryParse<XContentTypeOptions>(Headers.XContentTypeOptions, true, out var _))
        {
            yield return new ValidationResult($"{nameof(Headers)}.{nameof(Headers.XContentTypeOptions)} has an invalid value of '{Headers.XContentTypeOptions}'.");
        }

        if (!Enum.TryParse<XssProtection>(Headers.XXssProtection, true, out var _))
        {
            yield return new ValidationResult($"{nameof(Headers)}.{nameof(Headers.XXssProtection)} has an invalid value of '{Headers.XXssProtection}'.");
        }

        if (!Enum.TryParse<ReferrerPolicy>(Headers.ReferrerPolicy, true, out var _))
        {
            yield return new ValidationResult($"{nameof(Headers)}.{nameof(Headers.ReferrerPolicy)} has an invalid value of '{Headers.ReferrerPolicy}'.");
        }

        if (!Enum.TryParse<XFrameOptions>(Headers.XFrameOptions, true, out var _))
        {
            yield return new ValidationResult($"{nameof(Headers)}.{nameof(Headers.XFrameOptions)} has an invalid value of '{Headers.XFrameOptions}'.");
        }

        if (!Enum.TryParse<CrossOriginEmbedderPolicy>(Headers.CrossOriginEmbedderPolicy, true, out var _))
        {
            yield return new ValidationResult($"{nameof(Headers)}.{nameof(Headers.CrossOriginEmbedderPolicy)} has an invalid value of '{Headers.CrossOriginEmbedderPolicy}'.");
        }

        if (!Enum.TryParse<CrossOriginOpenerPolicy>(Headers.CrossOriginOpenerPolicy, true, out var _))
        {
            yield return new ValidationResult($"{nameof(Headers)}.{nameof(Headers.CrossOriginOpenerPolicy)} has an invalid value of '{Headers.CrossOriginOpenerPolicy}'.");
        }

        if (!Enum.TryParse<CrossOriginResourcePolicy>(Headers.CrossOriginResourcePolicy, true, out var _))
        {
            yield return new ValidationResult($"{nameof(Headers)}.{nameof(Headers.CrossOriginResourcePolicy)} has an invalid value of '{Headers.CrossOriginResourcePolicy}'.");
        }

        if (Headers.StrictTransportSecurityMaxAge < 1 || Headers.StrictTransportSecurityMaxAge > CspConstants.TwoYearsInSeconds)
        {
            yield return new ValidationResult($"{nameof(Headers)}.{nameof(Headers.StrictTransportSecurityMaxAge)} has an invalid value of '{Headers.StrictTransportSecurityMaxAge}'.");
        }
    }
}