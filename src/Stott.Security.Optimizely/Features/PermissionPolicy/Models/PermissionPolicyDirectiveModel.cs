using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Models;

public sealed class PermissionPolicyDirectiveModel
{
    /// <summary>
    /// Creates an empty model.  Used when binding data provided by an API consumer, where the
    /// directive metadata is resolved as the directive name is applied.
    /// </summary>
    [JsonConstructor]
    public PermissionPolicyDirectiveModel()
    {
    }

    /// <summary>
    /// Creates a model representing a directive which has not been configured.
    /// </summary>
    public PermissionPolicyDirectiveModel(PermissionPolicyDirective directive)
    {
        Name = directive.Name;
        Title = directive.Title;
        Description = directive.Description;
        IsDeprecated = directive.IsDeprecated;
        EnabledState = PermissionPolicyEnabledState.Disabled;
    }

    /// <summary>
    /// Creates a model representing a configured directive.
    /// </summary>
    /// <param name="entity">The stored configuration for the directive.</param>
    /// <param name="directive">The metadata for the directive, which is null when the stored directive is not recognised.</param>
    public PermissionPolicyDirectiveModel(Entities.PermissionPolicy entity, PermissionPolicyDirective? directive)
    {
        Name = entity.Directive;
        Title = directive?.Title ?? entity.Directive;
        Description = directive?.Description ?? string.Empty;
        IsDeprecated = directive?.IsDeprecated ?? false;

        EnabledState = Enum.TryParse<PermissionPolicyEnabledState>(entity.EnabledState, out var state) ? state : PermissionPolicyEnabledState.None;
        Sources = (entity.Origins ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                                  .Select(x => new PermissionPolicyUrl { Id = Guid.NewGuid(), Url = x })
                                                  .ToList();
    }

    public string? Name { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PermissionPolicyEnabledState EnabledState { get; set; }

    public List<PermissionPolicyUrl> Sources { get; set; } = [];

    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool IsDeprecated { get; set; }
}
