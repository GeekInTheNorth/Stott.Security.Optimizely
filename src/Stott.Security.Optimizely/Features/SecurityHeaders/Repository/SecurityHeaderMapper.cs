using System;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

namespace Stott.Security.Optimizely.Features.SecurityHeaders.Repository;

internal static class SecurityHeaderMapper
{
    public static SecurityHeaderModel ToModel(SecurityHeaderSettings? entity)
    {
        return new SecurityHeaderModel
        {
            XContentTypeOptions = (entity?.XContentTypeOptions ?? XContentTypeOptions.None).ToString(),
            XXssProtection = (entity?.XssProtection ?? XssProtection.None).ToString(),
            XFrameOptions = (entity?.FrameOptions ?? XFrameOptions.None).ToString(),
            ReferrerPolicy = (entity?.ReferrerPolicy ?? ReferrerPolicy.None).ToString(),
            CrossOriginEmbedderPolicy = (entity?.CrossOriginEmbedderPolicy ?? CrossOriginEmbedderPolicy.None).ToString(),
            CrossOriginOpenerPolicy = (entity?.CrossOriginOpenerPolicy ?? CrossOriginOpenerPolicy.None).ToString(),
            CrossOriginResourcePolicy = (entity?.CrossOriginResourcePolicy ?? CrossOriginResourcePolicy.None).ToString(),
            IsStrictTransportSecurityEnabled = entity?.IsStrictTransportSecurityEnabled ?? false,
            IsStrictTransportSecuritySubDomainsEnabled = entity?.IsStrictTransportSecuritySubDomainsEnabled ?? false,
            StrictTransportSecurityMaxAge = entity?.StrictTransportSecurityMaxAge ?? 1
        };
    }

    public static void ToEntity(SecurityHeaderSettings? entity, SecurityHeaderModel? model)
    {
        if (entity is null || model is null)
        {
            return;
        }

        if (Enum.TryParse<XContentTypeOptions>(model.XContentTypeOptions, true, out var xContentTypeOptions))
        {
            entity.XContentTypeOptions = xContentTypeOptions;
        }

        if (Enum.TryParse<XssProtection>(model.XXssProtection, true, out var xssProtection))
        {
            entity.XssProtection = xssProtection;
        }

        if (Enum.TryParse<ReferrerPolicy>(model.ReferrerPolicy, true, out var referrerPolicy))
        {
            entity.ReferrerPolicy = referrerPolicy;
        }

        if (Enum.TryParse<XFrameOptions>(model.XFrameOptions, true, out var xFrameOptions))
        {
            entity.FrameOptions = xFrameOptions;
        }

        if (Enum.TryParse<CrossOriginEmbedderPolicy>(model.CrossOriginEmbedderPolicy, true, out var crossOriginEmbedderPolicy))
        {
            entity.CrossOriginEmbedderPolicy = crossOriginEmbedderPolicy;
        }

        if (Enum.TryParse<CrossOriginOpenerPolicy>(model.CrossOriginOpenerPolicy, true, out var crossOriginOpenerPolicy))
        {
            entity.CrossOriginOpenerPolicy = crossOriginOpenerPolicy;
        }

        if (Enum.TryParse<CrossOriginResourcePolicy>(model.CrossOriginResourcePolicy, true, out var crossOriginResourcePolicy))
        {
            entity.CrossOriginResourcePolicy = crossOriginResourcePolicy;
        }

        entity.IsStrictTransportSecurityEnabled = model.IsStrictTransportSecurityEnabled;
        entity.IsStrictTransportSecuritySubDomainsEnabled = model.IsStrictTransportSecuritySubDomainsEnabled;
        entity.ForceHttpRedirect = model.ForceHttpRedirect;
        entity.StrictTransportSecurityMaxAge = GetRestrainedMaxAge(model.StrictTransportSecurityMaxAge);
    }

    private static int GetRestrainedMaxAge(int providedValue)
    {
        if (providedValue < 1)
        {
            return 1;
        }

        if (providedValue > CspConstants.TwoYearsInSeconds)
        {
            return CspConstants.TwoYearsInSeconds;
        }

        return providedValue;
    }
}