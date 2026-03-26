using System.Collections.Generic;
using System.Text.Json.Serialization;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class PermissionPolicyModel
{
    public bool IsEnabled { get; set; }

    public List<PermissionPolicyDirectiveModel> Directives { get; set; } = new List<PermissionPolicyDirectiveModel>();
}
