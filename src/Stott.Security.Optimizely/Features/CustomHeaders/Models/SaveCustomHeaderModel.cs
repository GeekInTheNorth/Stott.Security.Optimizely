namespace Stott.Security.Optimizely.Features.CustomHeaders.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.CustomHeaders.Repository;

/// <summary>
/// Model for saving a custom header with validation.
/// </summary>
public sealed partial class SaveCustomHeaderModel : IValidatableObject, ICustomHeader
{
    /// <summary>
    /// Gets or sets the unique identifier for the custom header.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the HTTP header.
    /// </summary>
    public string HeaderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the behavior for the header (Add or Remove).
    /// </summary>
    public CustomHeaderBehavior Behavior { get; set; }

    /// <summary>
    /// Gets or sets the value of the header (required when Behavior is Add).
    /// </summary>
    public string? HeaderValue { get; set; }

    /// <summary>
    /// Validates the model.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Required field validation
        if (string.IsNullOrWhiteSpace(HeaderName))
        {
            yield return new ValidationResult("Header Name is required.", [nameof(HeaderName)]);
            yield break;  // Stop further validation if header name is missing
        }

        // Header name format validation
        if (!IsValidHeaderName(HeaderName))
        {
            yield return new ValidationResult(
                "Header Name contains invalid characters. Only alphanumeric characters, hyphens, and underscores are allowed.",
                [nameof(HeaderName)]);
        }

        // Conditional value requirement
        if (Behavior == CustomHeaderBehavior.Add && string.IsNullOrWhiteSpace(HeaderValue))
        {
            yield return new ValidationResult(
                "Header Value is required when adding a header.",
                [nameof(HeaderValue)]);
        }

        // Duplicate header validation
        var repository = validationContext.GetService(typeof(ICustomHeaderRepository)) as ICustomHeaderRepository;
        if (repository != null)
        {
            var existingHeader = repository.GetByHeaderNameAsync(HeaderName).Result;
            if (existingHeader != null && existingHeader.Id != Id)
            {
                yield return new ValidationResult(
                    $"A custom header with the name '{HeaderName}' already exists.",
                    [nameof(HeaderName)]);
            }
        }

        // Check against built-in security headers
        if (IsBuiltInSecurityHeader(HeaderName))
        {
            yield return new ValidationResult(
                $"'{HeaderName}' is managed by other features in this addon. Please use the dedicated settings page for this header.",
                [nameof(HeaderName)]);
        }
    }

    private static bool IsValidHeaderName(string headerName)
    {
        // RFC 7230: field-name = token
        // token = 1*tchar
        // tchar = "!" / "#" / "$" / "%" / "&" / "'" / "*" / "+" / "-" / "." /
        //         "0"-"9" / "A"-"Z" / "^" / "_" / "`" / "a"-"z" / "|" / "~"
        return HeaderNameRegEx().IsMatch(headerName);
    }

    private static bool IsBuiltInSecurityHeader(string headerName)
    {
        var builtInHeaders = new[]
        {
            "Content-Security-Policy",
            "Content-Security-Policy-Report-Only",
            "Reporting-Endpoints",
            "X-Content-Type-Options",
            "X-Xss-Protection",
            "Referrer-Policy",
            "X-Frame-Options",
            "Cross-Origin-Embedder-Policy",
            "Cross-Origin-Opener-Policy",
            "Cross-Origin-Resource-Policy",
            "Strict-Transport-Security",
            "Permissions-Policy",
            "Access-Control-Allow-Origin",
            "Access-Control-Allow-Methods",
            "Access-Control-Allow-Headers",
            "Access-Control-Allow-Credentials",
            "Access-Control-Max-Age",
            "Access-Control-Expose-Headers"
        };

        return builtInHeaders.Any(x => x.Equals(headerName, StringComparison.OrdinalIgnoreCase));
    }

    [GeneratedRegex(@"^[a-zA-Z0-9\-_]+$")]
    private static partial Regex HeaderNameRegEx();
}
