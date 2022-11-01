namespace Stott.Security.Optimizely.Features.Settings.Service;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface ICspSettingsService
{
    Task<CspSettings> GetAsync();

    Task SaveAsync(CspSettingsModel cspSettings, string modifiedBy);
}
