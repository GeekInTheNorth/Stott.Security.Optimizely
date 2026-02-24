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
    /// Gets all custom headers.
    /// </summary>
    Task<IList<CustomHeaderModel>> GetAllAsync();

    /// <summary>
    /// Gets compiled headers for middleware application.
    /// </summary>
    Task<IList<HeaderDto>> GetCompiledHeaders();

    /// <summary>
    /// Saves a custom header (creates new or updates existing).
    /// </summary>
    Task SaveAsync(ICustomHeader? model, string? modifiedBy);

    /// <summary>
    /// Deletes a custom header by its unique identifier.
    /// </summary>
    Task DeleteAsync(Guid id);
}
