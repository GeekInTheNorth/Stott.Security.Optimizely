using System.Collections.Generic;
using System.Linq;

using EPiServer.Data.Dynamic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Permissions.List
{
    public class CspPermissionsQuery : ICspPermissionsQuery
    {
        private readonly DynamicDataStore _cspSourceStore;

        public CspPermissionsQuery(DynamicDataStoreFactory dataStoreFactory)
        {
            _cspSourceStore = dataStoreFactory.CreateStore(typeof(CspSource));
        }

        public IList<CspSource> Get()
        {
            return _cspSourceStore.LoadAll<CspSource>()?.ToList() ?? new List<CspSource>(0);
        }
    }
}
