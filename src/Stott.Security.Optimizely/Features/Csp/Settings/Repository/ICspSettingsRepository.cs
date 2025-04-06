namespace Stott.Security.Optimizely.Features.Csp.Settings.Repository;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;

public interface ICspSettingsRepository
{
    Task<CspSettings> GetAsync();

    Task SaveAsync(ICspSettings settings, string modifiedBy);
}