using System.Collections.Generic;

using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.Tools;

public sealed class PermissionPolicyModel : IPermissionPolicySettings
{
    public bool IsEnabled { get; set; }

    public List<PermissionPolicyDirectiveModel> Directives { get; set; } = new List<PermissionPolicyDirectiveModel>();
}
