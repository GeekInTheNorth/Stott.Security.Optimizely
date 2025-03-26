using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

public interface IPermissionPolicyRepository
{
    Task<PermissionPolicySettingsModel> GetSettingsAsync();

    Task<List<PermissionPolicyDirectiveModel>> ListDirectivesAsync();

    Task<List<string>> ListDirectiveFragments();

    Task SaveDirectiveAsync(SavePermissionPolicyModel model, string modifiedBy);

    Task SaveSettingsAsync(IPermissionPolicySettings settings, string modifiedBy);
}
