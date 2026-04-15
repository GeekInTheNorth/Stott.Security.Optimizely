namespace Stott.Security.Optimizely.Features.Csp.Settings.Service;

using System;
using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

public interface ICspSettingsService
{
    Task<CspSettings> GetAsync(Guid? siteId, string? hostName);

    Task SaveAsync(ICspSettings? cspSettings, string? modifiedBy, Guid? siteId, string? hostName);

    Task DeleteByContextAsync(Guid? siteId, string? hostName, string? deletedBy);

    Task<bool> ExistsForContextAsync(Guid? siteId, string? hostName);
}
