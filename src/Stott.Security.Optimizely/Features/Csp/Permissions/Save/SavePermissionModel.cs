namespace Stott.Security.Optimizely.Features.Csp.Permissions.Save;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Permissions.Validation;

public sealed class SavePermissionModel : IValidatableObject
{
    public Guid Id { get; set; }

    public string? Source { get; set; }

    public List<string>? Directives { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsSourceValid())
        {
            yield return new ValidationResult($"{nameof(Source)} is invalid.", new[] { nameof(Source) });
        }

        if (!IsDirectivesValid(out var errorMessage))
        {
            yield return new ValidationResult(errorMessage, new[] { nameof(Directives) });
        }

        if (!string.IsNullOrWhiteSpace(Source) && Directives is { Count: > 0 })
        {
            var sourceRule = SourceRules.GetRuleForSource(Source);
            if (sourceRule is not null && !sourceRule.IsValid(Directives))
            {
                yield return new ValidationResult(sourceRule.ErrorTemplate, new[] { nameof(Source) });
            }
        }
    }

    private bool IsDirectivesValid(out string errorMessage)
    {
        errorMessage = string.Empty;

        if (Directives == null || !Directives.Any())
        {
            errorMessage = $"{nameof(Directives)} must contain at least one value.";
            return false;
        }

        var allowedDirectives = CspConstants.AllDirectives;
        if (Directives.Any(x => !allowedDirectives.Contains(x)))
        {
            errorMessage = $"{nameof(Directives)} contains invalid values.";
            return false;
        }

        return true;
    }

    private bool IsSourceValid()
    {
        if (string.IsNullOrWhiteSpace(Source))
        {
            return false;
        }

        if (CspConstants.AllSources.Contains(Source))
        {
            return true;
        }

        return Regex.IsMatch(Source, CspConstants.RegexPatterns.UrlDomain) ||
               Regex.IsMatch(Source, CspConstants.RegexPatterns.UrlLocalHost) ||
               Regex.IsMatch(Source, CspConstants.RegexPatterns.Hashes);
    }
}
