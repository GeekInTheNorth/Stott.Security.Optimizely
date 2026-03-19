using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Service;

public interface IPermissionPolicyService
{
    Task<IPermissionPolicySettings> GetPermissionPolicySettingsAsync(string? appId, string? hostName);

    Task<IPermissionPolicySettings?> GetPermissionPolicySettingsByContextAsync(string? appId, string? hostName);

    Task SaveSettingsAsync(IPermissionPolicySettings? settings, string? modifiedBy, string? appId, string? hostName);

    Task DeleteSettingsByContextAsync(string? appId, string? hostName, string? deletedBy);

    Task<IList<PermissionPolicyDirectiveModel>> ListDirectivesAsync(string? appId, string? hostName, string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter);

    Task<bool> HasDirectiveOverrideAsync(string? appId, string? hostName);

    Task SaveDirectiveAsync(SavePermissionPolicyModel? model, string? modifiedBy, string? appId, string? hostName);

    Task CreateDirectiveOverrideAsync(string? appId, string? hostName, string? modifiedBy);

    Task DeleteDirectivesByContextAsync(string? appId, string? hostName, string? deletedBy);

    Task<IEnumerable<HeaderDto>> GetCompiledHeaders(string? appId, string? hostName);
}
