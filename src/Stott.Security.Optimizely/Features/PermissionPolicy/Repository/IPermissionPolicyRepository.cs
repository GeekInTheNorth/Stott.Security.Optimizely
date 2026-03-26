using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

public interface IPermissionPolicyRepository
{
    Task<PermissionPolicySettingsModel> GetSettingsAsync(string? appId, string? hostName);

    Task<PermissionPolicySettingsModel?> GetSettingsByContextAsync(string? appId, string? hostName);

    Task SaveSettingsAsync(IPermissionPolicySettings settings, string modifiedBy, string? appId, string? hostName);

    Task<List<PermissionPolicyDirectiveModel>> ListDirectivesAsync(string? appId, string? hostName);

    Task<List<PermissionPolicyDirectiveModel>?> ListDirectivesByContextAsync(string? appId, string? hostName);

    Task<List<string>> ListDirectiveFragments(string? appId, string? hostName);

    Task SaveDirectiveAsync(SavePermissionPolicyModel model, string modifiedBy, string? appId, string? hostName);

    Task CreateOverrideAsync(string? sourceAppId, string? sourceHostName, string? targetAppId, string? targetHostName, string modifiedBy);

    Task DeleteByContextAsync(string? appId, string? hostName, string deletedBy);
}
