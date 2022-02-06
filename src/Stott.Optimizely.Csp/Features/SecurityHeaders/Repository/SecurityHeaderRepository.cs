using System.Linq;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;

namespace Stott.Optimizely.Csp.Features.SecurityHeaders.Repository
{
    public class SecurityHeaderRepository : ISecurityHeaderRepository
    {
        private readonly DynamicDataStore _securityHeaderSettingsStore;

        public SecurityHeaderRepository(DynamicDataStoreFactory dataStoreFactory)
        {
            _securityHeaderSettingsStore = dataStoreFactory.CreateStore(typeof(SecurityHeaderSettings));
        }

        public SecurityHeaderSettings Get()
        {
            return _securityHeaderSettingsStore.LoadAll<SecurityHeaderSettings>()?.FirstOrDefault() ?? new SecurityHeaderSettings();
        }

        public void Save(bool isXContentTypeOptionsEnabled, bool isXXssProtectionEnabled, ReferrerPolicy referrerPolicy, XFrameOptions frameOptions)
        {
            var recordToSave = _securityHeaderSettingsStore.LoadAll<SecurityHeaderSettings>()?.FirstOrDefault();
            if (recordToSave == null)
            {
                recordToSave = new SecurityHeaderSettings { Id = Identity.NewIdentity() };
            }

            recordToSave.IsXContentTypeOptionsEnabled = isXContentTypeOptionsEnabled;
            recordToSave.IsXXssProtectionEnabled = isXXssProtectionEnabled;
            recordToSave.FrameOptions = frameOptions;
            recordToSave.ReferrerPolicy = referrerPolicy;

            _securityHeaderSettingsStore.Save(recordToSave);
        }
    }
}
