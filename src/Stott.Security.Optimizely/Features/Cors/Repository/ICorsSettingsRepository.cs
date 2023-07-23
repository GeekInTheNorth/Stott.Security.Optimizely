namespace Stott.Security.Optimizely.Features.Cors.Repository;

using System.Threading.Tasks;

public interface ICorsSettingsRepository
{
    Task<CorsConfiguration> GetAsync();

    Task SaveAsync(CorsConfiguration? model, string? modifiedBy);
}