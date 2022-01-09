using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

using Stott.Optimizely.Csp.Common;

namespace Stott.Optimizely.Csp.Features.Permissions.Save
{
    public class SavePermissionModel : IValidatableObject
    {
        public Guid Id { get; set; }

        public string Source { get; set; }

        public List<string> Directives { get; set; }

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
        }

        private bool IsDirectivesValid(out string errorMessage)
        {
            errorMessage = null;

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

            if (CspConstants.AllSources.Contains(this.Source))
            {
                return true;
            }

            return Regex.IsMatch(Source, "^([a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{3,}\\.{1}[a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{3,})$");
        }
    }
}
