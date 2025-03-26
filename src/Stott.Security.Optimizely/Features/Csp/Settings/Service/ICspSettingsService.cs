namespace Stott.Security.Optimizely.Features.Csp.Settings.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

public interface ICspSettingsService
{
    CspSettings Get();

    Task<CspSettings> GetAsync();

    Task SaveAsync(ICspSettings cspSettings, string? modifiedBy);
}