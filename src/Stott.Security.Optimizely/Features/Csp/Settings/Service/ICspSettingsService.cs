namespace Stott.Security.Optimizely.Features.Csp.Settings.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

public interface ICspSettingsService
{
    CspSettings Get(string? appId, string? hostName);

    Task<CspSettings> GetAsync(string? appId, string? hostName);

    Task<CspSettings?> GetByContextAsync(string? appId, string? hostName);

    Task SaveAsync(ICspSettings cspSettings, string? modifiedBy, string? appId, string? hostName);

    Task DeleteByContextAsync(string? appId, string? hostName, string? deletedBy);
}
