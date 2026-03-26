namespace Stott.Security.Optimizely.Features.Csp.Settings.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

public interface ICspSettingsService
{
    /// <summary>
    /// Gets settings based on a specific context (appId and hostName).
    /// If no settings exist for the specified context, it falls back to inherited settings (e.g., global settings).
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="hostName"></param>
    /// <returns></returns>
    Task<CspSettings> GetAsync(string? appId, string? hostName);

    /// <summary>
    /// Asynchronously saves the specified Content Security Policy (CSP) settings to the data store.
    /// </summary>
    /// <param name="cspSettings">The CSP settings to be saved. Cannot be null.</param>
    /// <param name="modifiedBy">The identifier of the user or process making the modification. May be null if not applicable.</param>
    /// <param name="appId">The application identifier associated with the CSP settings. May be null if not applicable.</param>
    /// <param name="hostName">The host name for which the CSP settings apply. May be null if not applicable.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveAsync(ICspSettings cspSettings, string? modifiedBy, string? appId, string? hostName);

    /// <summary>
    /// Deletes records associated with the specified application and host context asynchronously.
    /// </summary>
    /// <param name="appId">The identifier of the application whose records are to be deleted. Can be null to target all applications.</param>
    /// <param name="hostName">The name of the host associated with the records to delete. Can be null to target all hosts.</param>
    /// <param name="deletedBy">The user or process responsible for initiating the deletion. Can be null if not specified.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy);

    /// <summary>
    /// Determines whether content security policy settings exist for a specific context (appId and hostName) that would override inherited settings.
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="hostName"></param>
    /// <returns></returns>
    Task<bool> ExistsForContextAsync(string? appId, string? hostName);
}
