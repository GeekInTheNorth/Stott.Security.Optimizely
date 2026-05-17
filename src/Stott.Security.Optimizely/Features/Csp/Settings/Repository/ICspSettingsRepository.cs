namespace Stott.Security.Optimizely.Features.Csp.Settings.Repository;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

public interface ICspSettingsRepository
{
    /// <summary>
    /// Returns the resolved CSP settings for the given context, walking the Host > Site > Global fallback chain.
    /// </summary>
    Task<CspSettings> GetAsync(Guid? siteId, string? hostName);

    /// <summary>
    /// Returns the exact record stored against the given context, or null when the context is currently inheriting.
    /// </summary>
    Task<CspSettings?> GetByContextAsync(Guid? siteId, string? hostName);

    /// <summary>
    /// Upserts the CSP settings for the given context.
    /// </summary>
    Task SaveAsync(ICspSettings settings, Guid? siteId, string? hostName, string modifiedBy);

    /// <summary>
    /// Removes the CSP settings stored for the given context (cannot be used against the Global scope).
    /// </summary>
    Task DeleteByContextAsync(Guid? siteId, string? hostName, string deletedBy);
}
