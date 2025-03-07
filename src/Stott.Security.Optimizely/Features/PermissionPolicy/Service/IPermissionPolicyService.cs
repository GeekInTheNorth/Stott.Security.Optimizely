using System.Collections.Generic;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Service;

public interface IPermissionPolicyService
{
    Task<IPermissionPolicySettings> GetPermissionPolicySettingsAsync();

    Task<IList<PermissionPolicyDirectiveModel>> ListDirectivesAsync(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter);

    Task<IEnumerable<KeyValuePair<string, string>>> GetCompiledHeaders();

    Task SaveDirectiveAsync(SavePermissionPolicyModel? model, string? modifiedBy);

    Task SaveSettingsAsync(IPermissionPolicySettings? settings, string? modifiedBy);
}
