using System.Collections.Generic;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Service;

public interface IPermissionPolicyService
{
    Task<IList<PermissionPolicyDirectiveModel>> List(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter);

    Task<IEnumerable<KeyValuePair<string, string>>> GetCompiledHeaders();

    Task Save(SavePermissionPolicyModel model, string? modifiedBy);
}
