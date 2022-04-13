using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using Stott.Optimizely.Csp.Common;

namespace Stott.Optimizely.Csp.Features.Permissions.Save
{
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

            if (CspConstants.AllSources.Contains(this.Source))
            {
                return true;
            }

            return Regex.IsMatch(Source, "^([a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{3,}\\.{1}[a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{3,})$");
        }
    }
}
