namespace Stott.Security.Optimizely.Features.CustomHeaders.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.CustomHeaders.Models;
using Stott.Security.Optimizely.Features.Header;

/// <summary>
/// Service interface for custom header business logic.
/// </summary>
public interface ICustomHeaderService
{
    /// <summary>
    /// Gets all custom headers for the given context using the fallback chain.
    /// </summary>
    Task<IList<CustomHeaderModel>> GetAllAsync(Guid? siteId, string? hostName);

    /// <summary>
    /// Gets compiled headers for middleware application for the given context.
    /// </summary>
    Task<IList<HeaderDto>> GetCompiledHeaders(Guid? siteId, string? hostName);

    /// <summary>
    /// Checks whether the exact context has its own custom header records (override exists).
    /// </summary>
    Task<bool> ExistsForContextAsync(Guid? siteId, string? hostName);

    /// <summary>
    /// Creates an override by copying resolved headers from the parent context to the target context.
    /// </summary>
    Task CreateOverrideAsync(Guid? siteId, string? hostName, string? modifiedBy);

    /// <summary>
    /// Deletes all custom headers for the exact context (revert to inherited).
    /// </summary>
    Task DeleteByContextAsync(Guid? siteId, string? hostName, string? deletedBy);

    /// <summary>
    /// Saves a custom header (creates new or updates existing) within the given context.
    /// </summary>
    Task SaveAsync(ICustomHeader? model, string? modifiedBy, Guid? siteId, string? hostName);

    /// <summary>
    /// Deletes a custom header by its unique identifier.
    /// </summary>
    Task DeleteAsync(Guid id);
}
