namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Csp.Sandbox;

/// <summary>
/// Handles CRUD operations for CSP sandbox settings, including caching and fallback logic for inherited settings based on application and host context.
/// </summary>
public interface ICspSandboxService
{
    /// <summary>
    /// Gets sandbox settings, using fallbacks for inherited settings of host > application > global
    /// </summary>
    /// <param name="siteId"></param>
    /// <param name="hostName"></param>
    /// <returns></returns>
    Task<SandboxModel> GetAsync(Guid? siteId, string? hostName);

    /// <summary>
    /// Saves a specific sandbox settings for a given application and host context.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    Task SaveAsync(SandboxModel model, string? modifiedBy);

    /// <summary>
    /// Deletes a specific sandbox settings for a given application and host context.
    /// </summary>
    /// <param name="siteId"></param>
    /// <param name="hostName"></param>
    /// <param name="deletedBy"></param>
    /// <returns></returns>
    Task DeleteByContextAsync(Guid? siteId, string? hostName, string? deletedBy);

    /// <summary>
    /// Determines whether sandbox settings exist for a specific context (appId and hostName) that would override inherited settings.
    /// </summary>
    /// <param name="siteId"></param>
    /// <param name="hostName"></param>
    /// <returns></returns>
    Task<bool> ExistsForContextAsync(Guid? siteId, string? hostName);
}
