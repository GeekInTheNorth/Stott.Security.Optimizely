using System.Collections.Generic;
using System.Text.Json.Serialization;
using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.Tools.Models;

public sealed class PermissionPolicyMigrationModel
{
    public bool IsEnabled { get; set; }

    public List<PermissionPolicyDirectiveMigrationModel> Directives { get; set; } = new List<PermissionPolicyDirectiveMigrationModel>();
}

public sealed class PermissionPolicyDirectiveMigrationModel
{   
    public string? Name { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PermissionPolicyEnabledState EnabledState { get; set; }

    public List<PermissionPolicyUrl> Sources { get; set; } = new List<PermissionPolicyUrl>();
}