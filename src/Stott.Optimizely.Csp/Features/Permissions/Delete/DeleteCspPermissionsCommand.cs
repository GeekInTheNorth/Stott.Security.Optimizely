using System;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Features.Permissions.Delete
{

    public class DeleteCspPermissionsCommand : IDeleteCspPermissionsCommand
    {
        private readonly DynamicDataStore _cspSourceStore;

        public DeleteCspPermissionsCommand(DynamicDataStoreFactory dataStoreFactory)
        {
            _cspSourceStore = dataStoreFactory.CreateStore(typeof(CspSource));
        }

        public void Execute(Guid id)
        {
            var identity = Identity.NewIdentity(id);

            _cspSourceStore.Delete(identity);
        }
    }
}
