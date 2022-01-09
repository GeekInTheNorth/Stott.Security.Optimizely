using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Entities.Exceptions;

namespace Stott.Optimizely.Csp.Features.Permissions.Save
{
    public class SaveCspPermissionsCommand : ISaveCspPermissionsCommand
    {
        private readonly DynamicDataStore _cspSourceStore;

        public SaveCspPermissionsCommand(DynamicDataStoreFactory dataStoreFactory)
        {
            _cspSourceStore = dataStoreFactory.CreateStore(typeof(CspSource));
        }

        public void Save(Guid id, string source, List<string> directives)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            if(directives == null || !directives.Any())
            {
                throw new ArgumentException($"{directives} must not be null or empty.", nameof(directives));
            }

            var combinedDirectives = string.Join(",", directives);
            Save(id, source, combinedDirectives);
        }

        private void Save(Guid id, string source, string directives)
        {
            var matchingSource = _cspSourceStore.Find<CspSource>(nameof(CspSource.Source), source).FirstOrDefault();
            if (matchingSource != null && !matchingSource.Id.ExternalId.Equals(id))
            {
                throw new EntityExistsException($"An entry already exists for the source of '{source}'.");
            }

            var recordToSave = Guid.Empty.Equals(id) ? CreateNewRecord() : _cspSourceStore.Load<CspSource>(Identity.NewIdentity(id));
            recordToSave.Source = source;
            recordToSave.Directives = directives;

            _cspSourceStore.Save(recordToSave);
        }

        private static CspSource CreateNewRecord()
        {
            return new CspSource { Id = Identity.NewIdentity() };
        }
    }
}
