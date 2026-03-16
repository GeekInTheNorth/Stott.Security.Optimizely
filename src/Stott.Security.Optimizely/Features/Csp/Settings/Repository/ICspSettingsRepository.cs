namespace Stott.Security.Optimizely.Features.Csp.Settings.Repository;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

public interface ICspSettingsRepository
{
    Task<CspSettings> GetAsync(string? appId, string? hostName);

    Task<CspSettings?> GetByContextAsync(string? appId, string? hostName);

    Task SaveAsync(ICspSettings settings, string modifiedBy, string? appId, string? hostName);

    Task DeleteByContextAsync(string? appId, string? hostName, string deletedBy);
}
