using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Models;

public sealed class SavePermissionPolicyModel : IValidatableObject
{
    private const string SourceRegEx = @"^(http|ws)[s]{0,1}:\/\/(\*\.){0,1}([a-z0-9\-]{1,}\.){1,}([a-z0-9\-]{1,}(:[0-9]{1,5}){0,1}\/{0,1}){1}$";

    [Required]
    public string? Name { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PermissionPolicyEnabledState EnabledState { get; set; }

    public List<string?> Sources { get; set; } = new List<string?>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PermissionPolicyConstants.AllDirectives.All(x => !string.Equals(x, Name, StringComparison.OrdinalIgnoreCase)))
        {
            yield return new ValidationResult("The name must be a valid directive.", new[] { nameof(Name) });
        }

        if (EnabledState == PermissionPolicyEnabledState.SpecificSites || EnabledState == PermissionPolicyEnabledState.ThisAndSpecificSites)
        {
            var validSources = Sources?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!).ToList() ?? new List<string>();

            if (validSources is not { Count: > 0 })
            {
                yield return new ValidationResult("At least one valid source must be provided when this permission policy targets specific sites.", new[] { nameof(Sources) });
            }

            foreach (var source in validSources)
            {
                if (!Regex.IsMatch(source, SourceRegEx, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)))
                {
                    yield return new ValidationResult($"'{source}' is not a valid source.", new[] { nameof(Sources) });
                }
            }
        }
    }
}
