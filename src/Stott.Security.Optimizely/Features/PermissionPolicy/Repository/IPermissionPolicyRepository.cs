using System.Collections.Generic;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

public interface IPermissionPolicyRepository
{
    Task<List<PermissionPolicyDirectiveModel>> List();

    Task<List<string>> ListFragments();

    Task Save(SavePermissionPolicyModel model, string modifiedBy);
}
