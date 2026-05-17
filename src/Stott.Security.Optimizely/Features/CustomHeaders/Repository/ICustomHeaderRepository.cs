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
    /// Gets all custom headers for the given context using the fallback chain: Host -> Site -> Global.
    /// </summary>
    Task<IEnumerable<CustomHeader>> GetAllAsync(Guid? siteId, string? hostName);

    /// <summary>
    /// Gets all custom headers for the exact context match only. Returns null if no records exist at that exact level.
    /// </summary>
    Task<IEnumerable<CustomHeader>?> GetAllByContextAsync(Guid? siteId, string? hostName);

    /// <summary>
    /// Gets a custom header by its unique identifier.
    /// </summary>
    Task<CustomHeader?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets a custom header by its header name scoped to the given context (for duplicate check).
    /// </summary>
    Task<CustomHeader?> GetByHeaderNameAsync(string headerName, Guid? siteId, string? hostName);

    /// <summary>
    /// Saves a custom header (creates new or updates existing) within the given context.
    /// </summary>
    Task SaveAsync(ICustomHeader model, string modifiedBy, Guid? siteId, string? hostName);

    /// <summary>
    /// Copies resolved headers from source context to target context.
    /// </summary>
    Task CreateOverrideAsync(Guid? sourceSiteId, string? sourceHostName, Guid? targetSiteId, string? targetHostName, string modifiedBy);

    /// <summary>
    /// Deletes all custom headers for the exact context. Prevents deletion of global records.
    /// </summary>
    Task DeleteByContextAsync(Guid? siteId, string? hostName, string deletedBy);

    /// <summary>
    /// Deletes a custom header by its unique identifier.
    /// </summary>
    Task DeleteAsync(Guid id);
}
