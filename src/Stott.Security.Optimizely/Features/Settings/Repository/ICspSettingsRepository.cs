namespace Stott.Security.Optimizely.Features.Settings.Repository;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface ICspSettingsRepository
{
    Task<CspSettings> GetAsync();

    Task SaveAsync(ICspSettings settings, string modifiedBy);
}