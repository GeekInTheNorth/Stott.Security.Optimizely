using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Repository;

public interface IPermissionPolicyRepository
{
    Task<PermissionPolicySettingsModel> GetSettingsAsync(Guid? siteId, string? hostName);

    Task<PermissionPolicySettingsModel?> GetSettingsByContextAsync(Guid? siteId, string? hostName);

    Task SaveSettingsAsync(IPermissionPolicySettings settings, string modifiedBy, Guid? siteId, string? hostName);

    Task<List<PermissionPolicyDirectiveModel>> ListDirectivesAsync(Guid? siteId, string? hostName);

    Task<List<PermissionPolicyDirectiveModel>?> ListDirectivesByContextAsync(Guid? siteId, string? hostName);

    Task<List<string>> ListDirectiveFragments(Guid? siteId, string? hostName);

    Task SaveDirectiveAsync(SavePermissionPolicyModel model, string modifiedBy, Guid? siteId, string? hostName);

    Task CreateOverrideAsync(Guid? sourceSiteId, string? sourceHostName, Guid? targetSiteId, string? targetHostName, string modifiedBy);

    Task DeleteByContextAsync(Guid? siteId, string? hostName, string deletedBy);
}
