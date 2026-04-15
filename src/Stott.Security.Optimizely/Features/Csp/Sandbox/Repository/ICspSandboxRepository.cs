namespace Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Features.Csp.Sandbox;

public interface ICspSandboxRepository
{
    /// <summary>
    /// Returns the resolved sandbox settings for the given context, walking the Host > Site > Global fallback chain.
    /// </summary>
    Task<SandboxModel> GetAsync(Guid? siteId, string? hostName);

    /// <summary>
    /// Returns the exact record stored against the given context, or null when the context is currently inheriting.
    /// </summary>
    Task<SandboxModel?> GetByContextAsync(Guid? siteId, string? hostName);

    /// <summary>
    /// Upserts the sandbox settings for the given context.
    /// </summary>
    Task SaveAsync(SandboxModel model, string modifiedBy, Guid? siteId, string? hostName);

    /// <summary>
    /// Removes the sandbox settings stored for the given context (cannot be used against the Global scope).
    /// </summary>
    Task DeleteByContextAsync(Guid? siteId, string? hostName, string deletedBy);
}
