namespace Stott.Security.Core.Features.SecurityHeaders.Repository;

using System.Threading.Tasks;

using Stott.Security.Core.Entities;

public interface ISecurityHeaderRepository
{
    Task<SecurityHeaderSettings> GetAsync();

    Task SaveAsync(SecurityHeaderSettings settingsToSave);
}
