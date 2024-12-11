using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public interface IPermissionPolicyService
{
    IList<PermissionPolicyDirectiveModel> GetAll(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter);
}
