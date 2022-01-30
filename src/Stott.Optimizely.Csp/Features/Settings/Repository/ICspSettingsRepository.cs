using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Settings.Repository
{
    public interface ICspSettingsRepository
    {
        CspSettings Get();

        void Save(bool isEnabled, bool isReportOnly);
    }
}
