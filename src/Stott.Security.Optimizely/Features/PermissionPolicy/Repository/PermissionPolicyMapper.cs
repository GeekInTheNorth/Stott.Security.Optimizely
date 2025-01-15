using System;
using System.Linq;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

public static class PermissionPolicyMapper
{
    public static PermissionPolicyDirectiveModel ToModel(Entities.PermissionPolicy entity)
    {
        var origins = entity.Origins ?? string.Empty;
        var enabledState = Enum.TryParse<PermissionPolicyEnabledState>(entity.EnabledState, out var state) ? state : PermissionPolicyEnabledState.None;

        return new PermissionPolicyDirectiveModel
        {
            Name = entity.Directive,
            EnabledState = enabledState,
            Sources = origins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                             .Select(x => new PermissionPolicyUrl { Id = Guid.NewGuid(), Url = x })
                             .ToList()
        };
    }

    public static void ToEntity(SavePermissionPolicyModel model, Entities.PermissionPolicy entity, string modifiedBy)
    {
        entity.Directive = model.Name;
        entity.EnabledState = model.EnabledState.ToString();
        entity.Origins = string.Join(',', model.Sources.Where(x => !string.IsNullOrWhiteSpace(x)));
        entity.Modified = DateTime.UtcNow;
        entity.ModifiedBy = modifiedBy;
    }
}
