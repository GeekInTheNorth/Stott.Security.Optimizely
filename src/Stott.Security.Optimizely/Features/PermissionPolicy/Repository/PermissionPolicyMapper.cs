using System;
using System.Linq;

using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

internal static class PermissionPolicyMapper
{
    internal static PermissionPolicyDirectiveModel ToModel(Entities.PermissionPolicy entity)
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

    internal static void ToEntity(SavePermissionPolicyModel model, Entities.PermissionPolicy entity, string modifiedBy)
    {
        entity.Directive = model.Name;
        entity.EnabledState = model.EnabledState.ToString();
        entity.Origins = string.Join(',', model.Sources.Where(x => !string.IsNullOrWhiteSpace(x)));
        entity.Modified = DateTime.UtcNow;
        entity.ModifiedBy = modifiedBy;
    }

    internal static Entities.PermissionPolicy ToEntity(PermissionPolicyDirectiveModel model, string modifiedBy, DateTime modified)
    {
        return new Entities.PermissionPolicy
        {
            Directive = model.Name,
            EnabledState = model.EnabledState.ToString(),
            Origins = string.Join(',', model.Sources.Select(x => x.Url)),
            Modified = modified,
            ModifiedBy = modifiedBy
        };
    }

    internal static string ToPolicyFragment(Entities.PermissionPolicy entity)
    {
        var enabledState = Enum.TryParse<PermissionPolicyEnabledState>(entity.EnabledState, out var state) ? state : PermissionPolicyEnabledState.None;

        return enabledState switch
        {
            PermissionPolicyEnabledState.None => $"{entity.Directive}=()",
            PermissionPolicyEnabledState.All => $"{entity.Directive}=*",
            PermissionPolicyEnabledState.ThisSite => $"{entity.Directive}=(self)",
            PermissionPolicyEnabledState.ThisAndSpecificSites => $"{entity.Directive}=(self {ToPolicyFragmentSources(entity.Origins ?? string.Empty)})",
            PermissionPolicyEnabledState.SpecificSites => $"{entity.Directive}=({ToPolicyFragmentSources(entity.Origins ?? string.Empty)})",
            _ => string.Empty,
        };
    }

    internal static string ToPolicyFragmentSources(string origins)
    {
        var originsList = origins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        var formattedOrigins = originsList.Select(x => $"\"{x}\"");

        return string.Join(' ', formattedOrigins);
    }
}
