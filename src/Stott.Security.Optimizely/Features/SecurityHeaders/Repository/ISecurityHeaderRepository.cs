namespace Stott.Security.Optimizely.Features.SecurityHeaders.Repository;

using System.Threading.Tasks;

using Stott.Security.Optimizely.Entities;

public interface ISecurityHeaderRepository
{
    Task<SecurityHeaderSettings> GetAsync();

    Task SaveAsync(SecurityHeaderSettings settingsToSave);
}