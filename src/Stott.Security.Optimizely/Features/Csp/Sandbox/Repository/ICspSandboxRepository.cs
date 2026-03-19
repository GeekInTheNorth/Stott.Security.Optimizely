namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

using System.Threading.Tasks;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

/// <summary>
/// Handles CRUD operations for CSP sandbox settings.
/// </summary>
public interface ICspSandboxRepository
{
    /// <summary>
    /// Gets sandbox settings, using fallbacks for inherited settings of host > application > global
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="hostName"></param>
    /// <returns></returns>
    Task<SandboxModel> GetAsync(string? appId, string? hostName);

    /// <summary>
    /// Gets sandbox settings without using fallbacks.
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="hostName"></param>
    /// <returns></returns>
    Task<SandboxModel?> GetByContextAsync(string? appId, string? hostName);

    /// <summary>
    /// Saves a specific sandbox settings for a given application and host context.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="modifiedBy"></param>
    /// <param name="appId"></param>
    /// <param name="hostName"></param>
    /// <returns></returns>
    Task SaveAsync(SandboxModel model, string modifiedBy, string? appId, string? hostName);

    /// <summary>
    /// Deletes a specific sandbox settings for a given application and host context.
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="hostName"></param>
    /// <param name="deletedBy"></param>
    /// <returns></returns>
    Task DeleteByContextAsync(string? appId, string? hostName, string deletedBy);
}
