using System.Threading.Tasks;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Settings.Repository
{
    public interface ICspSettingsRepository
    {
        Task<CspSettings> GetAsync();

        Task SaveAsync(bool isEnabled, bool isReportOnly);
    }
}
