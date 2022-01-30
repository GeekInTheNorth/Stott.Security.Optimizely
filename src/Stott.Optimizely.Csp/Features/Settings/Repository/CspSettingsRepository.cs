using System.Linq;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Settings.Repository
{
    public class CspSettingsRepository : ICspSettingsRepository
    {
        private readonly DynamicDataStore _cspSettingsStore;

        public CspSettingsRepository(DynamicDataStoreFactory dataStoreFactory)
        {
            _cspSettingsStore = dataStoreFactory.CreateStore(typeof(CspSettings));
        }

        public CspSettings Get()
        {
            return _cspSettingsStore.LoadAll<CspSettings>()?.FirstOrDefault() ?? new CspSettings();
        }

        public void Save(bool isEnabled, bool isReportOnly)
        {
            var recordToSave = _cspSettingsStore.LoadAll<CspSettings>()?.FirstOrDefault();
            if (recordToSave == null)
            {
                recordToSave = new CspSettings { Id = Identity.NewIdentity() };
            }

            recordToSave.IsEnabled = isEnabled;
            recordToSave.IsReportOnly = isReportOnly;

            _cspSettingsStore.Save(recordToSave);
        }
    }
}
