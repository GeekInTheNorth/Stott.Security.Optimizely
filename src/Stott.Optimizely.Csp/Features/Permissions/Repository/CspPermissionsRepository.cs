using System.Collections.Generic;

using EPiServer.Data.Dynamic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Permissions.Repository
{
    public class CspPermissionsRepository : ICspPermissionsRepository
    {
        private readonly DynamicDataStore _cspSourceStore;

        public CspPermissionsRepository(DynamicDataStoreFactory dataStoreFactory)
        {
            _cspSourceStore = dataStoreFactory.CreateStore(typeof(CspSource));
        }

        public IEnumerable<CspSource> List()
        {
            return _cspSourceStore.LoadAll<CspSource>();
        }
    }
}
