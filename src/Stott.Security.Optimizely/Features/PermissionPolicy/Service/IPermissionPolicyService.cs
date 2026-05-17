using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Service;

public interface IPermissionPolicyService
{
    Task<IPermissionPolicySettings> GetPermissionPolicySettingsAsync(Guid? siteId, string? hostName);

    Task<IPermissionPolicySettings?> GetPermissionPolicySettingsByContextAsync(Guid? siteId, string? hostName);

    Task SaveSettingsAsync(IPermissionPolicySettings? settings, string? modifiedBy, Guid? siteId, string? hostName);

    Task<IList<PermissionPolicyDirectiveModel>> ListDirectivesAsync(Guid? siteId, string? hostName, string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter);

    Task SaveDirectiveAsync(SavePermissionPolicyModel? model, string? modifiedBy, Guid? siteId, string? hostName);

    Task<bool> ExistsForContextAsync(Guid? siteId, string? hostName);

    Task CreateOverrideAsync(Guid? siteId, string? hostName, string? modifiedBy);

    Task DeleteByContextAsync(Guid? siteId, string? hostName, string? deletedBy);

    Task<IEnumerable<HeaderDto>> GetCompiledHeaders(Guid? siteId, string? hostName);
}
