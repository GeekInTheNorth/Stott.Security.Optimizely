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

    Task<IList<PermissionPolicyDirectiveModel>> ListDirectivesAsync(string? appId, string? hostName, string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter);

    Task SaveDirectiveAsync(SavePermissionPolicyModel? model, string? modifiedBy, string? appId, string? hostName);

    Task<bool> HasOverrideAsync(string? appId, string? hostName);

    Task CreateOverrideAsync(string? appId, string? hostName, string? modifiedBy);

    Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy);

    Task<IEnumerable<HeaderDto>> GetCompiledHeaders(string? appId, string? hostName);
}
