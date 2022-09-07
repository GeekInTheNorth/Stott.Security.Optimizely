namespace Stott.Security.Core.Features.Permissions.Save;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using Stott.Security.Core.Common;

public class AppendPermissionModel : IValidatableObject
{
    public string Source { get; set; }

    public string Directive { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsSourceValid())
        {
            yield return new ValidationResult($"{nameof(Source)} is invalid.", new[] { nameof(Source) });
        }

        if (!IsDirectivesValid(out var errorMessage))
        {
            yield return new ValidationResult(errorMessage, new[] { nameof(Directive) });
        }
    }

    private bool IsDirectivesValid(out string errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(Directive))
        {
            errorMessage = $"{nameof(Directive)} must contain at least one value.";
            return false;
        }

        var allowedDirectives = CspConstants.AllDirectives;
        if (!allowedDirectives.Contains(Directive))
        {
            errorMessage = $"{nameof(Directive)} contains an invalid value directive.";
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
               Regex.IsMatch(Source, CspConstants.RegexPatterns.UrlLocalHost);
    }
}
