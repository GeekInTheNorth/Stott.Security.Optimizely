using System.Threading.Tasks;

using Stott.Security.Core.Entities;

namespace Stott.Security.Core.Features.Settings.Repository
{
    public interface ICspSettingsRepository
    {
        Task<CspSettings> GetAsync();

        Task SaveAsync(bool isEnabled, bool isReportOnly);
    }
}
