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
    /// Gets all custom headers ordered by header name.
    /// </summary>
    Task<IEnumerable<CustomHeader>> GetAllAsync();

    /// <summary>
    /// Gets a custom header by its unique identifier.
    /// </summary>
    Task<CustomHeader?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets a custom header by its header name (for duplicate check).
    /// </summary>
    Task<CustomHeader?> GetByHeaderNameAsync(string headerName);

    /// <summary>
    /// Saves a custom header (creates new or updates existing).
    /// </summary>
    Task SaveAsync(ICustomHeader model, string modifiedBy);

    /// <summary>
    /// Deletes a custom header by its unique identifier.
    /// </summary>
    Task DeleteAsync(Guid id);
}
