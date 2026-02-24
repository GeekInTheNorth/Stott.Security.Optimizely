namespace Stott.Security.Optimizely.Features.CustomHeaders.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;

/// <summary>
/// Mapper for converting between CustomHeader entities and models.
/// </summary>
internal static class CustomHeaderMapper
{
    // Change from 'const' to 'static readonly' to fix CS0133, and use collection initializer for IDE0300.
    internal static List<string> FixedHeaders = new()
    {
        CspConstants.HeaderNames.XssProtection,
        CspConstants.HeaderNames.FrameOptions,
        CspConstants.HeaderNames.ContentTypeOptions,
        CspConstants.HeaderNames.ReferrerPolicy,
        CspConstants.HeaderNames.CrossOriginEmbedderPolicy,
        CspConstants.HeaderNames.CrossOriginOpenerPolicy,
        CspConstants.HeaderNames.CrossOriginResourcePolicy,
        CspConstants.HeaderNames.StrictTransportSecurity,
    };

    /// <summary>
    /// Maps a CustomHeader entity to a CustomHeaderModel.
    /// </summary>
    internal static CustomHeaderModel ToModel(CustomHeader entity)
    {
        return new CustomHeaderModel
        {
            Id = entity.Id,
            HeaderName = entity.HeaderName,
            Behavior = entity.Behavior,
            HeaderValue = entity.HeaderValue,
            Description = GetDescriptionForHeaderName(entity.HeaderName),
            AllowedValues = GetAllowedValues(entity.HeaderName),
            PropertyType = GetPropertyType(entity.HeaderName),
            IsHeaderNameEditable = !FixedHeaders.Contains(entity.HeaderName, StringComparer.OrdinalIgnoreCase),
            CanDelete = true
        };
    }

    internal static CustomHeaderModel ToModel(string headerName)
    {
        return new CustomHeaderModel
        {
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Disabled,
            Description = GetDescriptionForHeaderName(headerName),
            AllowedValues = GetAllowedValues(headerName),
            PropertyType = GetPropertyType(headerName),
            IsHeaderNameEditable = !FixedHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase),
            CanDelete = false
        };
    }

    internal static string? GetPropertyType(string? headerName)
    {
        if (CspConstants.HeaderNames.StrictTransportSecurity.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return "hsts";
        }
        else if (FixedHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase))
        {
            return "select";
        }
        else
        {
            return "string";
        }
    }

    internal static string? GetDescriptionForHeaderName(string? headerName)
    {
        if (CspConstants.HeaderNames.XssProtection.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return "Configures the X-XSS-Protection header to instruct browsers to use XSS filters. Please note that modern browsers have either retired or will not implement XSS filtering. Legacy browsers have been known to contain vulnerabilities within their XSS filters that can compromise otherwise safe websites. It is recommended to set the header to 'Disabled' and to configure a Content Security Policy header. Only enable the X-XSS-Protection header if you must support legacy browsers.";
        }
        else if (CspConstants.HeaderNames.ContentTypeOptions.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return "Configures the X-Content-Type-Options header to prevent styles or scripts being loaded with the incorrect mime types.";
        }
        else if (CspConstants.HeaderNames.ReferrerPolicy.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return "Configures the Referrer-Policy header which instructs the browser on what information it should send in the Referrer header on subsequent requests.";
        }
        else if (CspConstants.HeaderNames.FrameOptions.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return "Configures the X-Frame-Options header to restrict the embedding of pages within frames on third party sites.";
        }
        else if (CspConstants.HeaderNames.StrictTransportSecurity.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return "Enforces secure (HTTP over SSL/TLS) connections to the server.";
        }
        else if (CspConstants.HeaderNames.CrossOriginEmbedderPolicy.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return "Configures the Cross-Origin-Embedder-Policy header which is used to prevent third party resources being loaded that have not explicitly granted cross origin permissions.";
        }
        else if (CspConstants.HeaderNames.CrossOriginOpenerPolicy.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return "Configures the Cross-Origin-Opener-Policy header which is used to prevent sharing context with cross origin documents.";
        }
        else if (CspConstants.HeaderNames.CrossOriginResourcePolicy.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return "Configures the Cross-Origin-Resource-Policy header which is used to limit what resources can consume the current site.";
        }

        return null;
    }

    internal static IList<CustomHeaderAllowedValue>? GetAllowedValues(string? headerName)
    {
        if (CspConstants.HeaderNames.XssProtection.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return new List<CustomHeaderAllowedValue>
            {
                new() { Value = string.Empty, Description = "Select a header value..." },
                new() { Value = "0", Description = "Disabled" },
                new() { Value = "1", Description = "Enabled" },
                new() { Value = "1; mode=block", Description = "Enabled With Blocking" }
            };
        }
        else if (CspConstants.HeaderNames.ContentTypeOptions.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return new List<CustomHeaderAllowedValue>
            {
                new() { Value = string.Empty, Description = "Select a header value..." },
                new() { Value = "nosniff", Description = "No Sniff" }
            };
        }
        else if (CspConstants.HeaderNames.ReferrerPolicy.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return new List<CustomHeaderAllowedValue>
            {
                new() { Value = string.Empty, Description = "Select a header value..." },
                new() { Value = "no-referrer", Description = "No Referrer" },
                new() { Value = "no-referrer-when-downgrade", Description = "No referrer When Downgrading (e.g. HTTP &#8594; HTTPS)" },
                new() { Value = "origin", Description = "Origin" },
                new() { Value = "origin-when-cross-origin", Description = "Origin When Cross Origin" },
                new() { Value = "same-origin", Description = "Same Origin" },
                new() { Value = "strict-origin", Description = "Strict Origin" },
                new() { Value = "strict-origin-when-cross-origin", Description = "Strict Origin When Cross Origin" },
                new() { Value = "unsafe-url", Description = "Unsafe Url" }
            };
        }
        else if (CspConstants.HeaderNames.FrameOptions.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return new List<CustomHeaderAllowedValue>
            {
                new() { Value = string.Empty, Description = "Select a header value..." },
                new() { Value = "SAMEORIGIN", Description = "Allow Framing only by this site (SAMEORIGIN)" },
                new() { Value = "DENY", Description = "Disallow Framing (DENY)" }
            };
        }
        else if (CspConstants.HeaderNames.CrossOriginEmbedderPolicy.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return new List<CustomHeaderAllowedValue>
            {
                new() { Value = string.Empty, Description = "Select a header value..." },
                new() { Value = "unsafe-none", Description = "Unsafe None" },
                new() { Value = "require-corp", Description = "Requires CORP" }
            };
        }
        else if (CspConstants.HeaderNames.CrossOriginOpenerPolicy.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return new List<CustomHeaderAllowedValue>
            {
                new() { Value = string.Empty, Description = "Select a header value..." },
                new() { Value = "unsafe-none", Description = "Unsafe None" },
                new() { Value = "same-origin", Description = "Same Origin" },
                new() { Value = "same-origin-allow-popups", Description = "Same Origin Allow Popups" }
            };
        }
        else if (CspConstants.HeaderNames.CrossOriginResourcePolicy.Equals(headerName, StringComparison.OrdinalIgnoreCase))
        {
            return new List<CustomHeaderAllowedValue>
            {
                new() { Value = string.Empty, Description = "Select a header value..." },
                new() { Value = "same-origin", Description = "Same Origin" },
                new() { Value = "same-site", Description = "Same Site" },
                new() { Value = "cross-origin", Description = "Cross Origin" }
            };
        }

        return null;
    }

    /// <summary>
    /// Maps a SaveCustomHeaderModel to a CustomHeader entity.
    /// </summary>
    internal static void ToEntity(ICustomHeader model, CustomHeader entity)
    {
        entity.HeaderName = model.HeaderName;
        entity.Behavior = model.Behavior;
        entity.HeaderValue = model.HeaderValue;
    }
}
