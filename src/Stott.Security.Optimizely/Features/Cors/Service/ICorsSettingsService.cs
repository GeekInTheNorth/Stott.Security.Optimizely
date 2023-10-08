namespace Stott.Security.Optimizely.Features.Cors.Service;

using System.Threading.Tasks;

public interface ICorsSettingsService
{
    Task<CorsConfiguration> GetAsync();

    Task SaveAsync(CorsConfiguration? model, string? modifiedBy);
}