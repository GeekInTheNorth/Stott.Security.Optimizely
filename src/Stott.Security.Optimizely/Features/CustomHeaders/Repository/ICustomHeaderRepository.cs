namespace Stott.Security.Optimizely.Features.CustomHeaders.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

/// <summary>
/// Repository interface for custom header data access.
/// </summary>
public interface ICustomHeaderRepository
{
    /// <summary>
    /// Gets all custom headers for the given context using the fallback chain: Host -> App -> Global.
    /// </summary>
    Task<IEnumerable<CustomHeader>> GetAllAsync(string? appId, string? hostName);

    /// <summary>
    /// Gets all custom headers for the exact context match only. Returns null if no records exist at that exact level.
    /// </summary>
    Task<IEnumerable<CustomHeader>?> GetAllByContextAsync(string? appId, string? hostName);

    /// <summary>
    /// Gets a custom header by its unique identifier.
    /// </summary>
    Task<CustomHeader?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets a custom header by its header name scoped to the given context (for duplicate check).
    /// </summary>
    Task<CustomHeader?> GetByHeaderNameAsync(string headerName, string? appId, string? hostName);

    /// <summary>
    /// Saves a custom header (creates new or updates existing) within the given context.
    /// </summary>
    Task SaveAsync(ICustomHeader model, string modifiedBy, string? appId, string? hostName);

    /// <summary>
    /// Copies resolved headers from source context to target context.
    /// </summary>
    Task CreateOverrideAsync(string? sourceAppId, string? sourceHostName, string? targetAppId, string? targetHostName, string modifiedBy);

    /// <summary>
    /// Deletes all custom headers for the exact context. Prevents deletion of global records.
    /// </summary>
    Task DeleteByContextAsync(string? appId, string? hostName, string deletedBy);

    /// <summary>
    /// Deletes a custom header by its unique identifier.
    /// </summary>
    Task DeleteAsync(Guid id);
}
