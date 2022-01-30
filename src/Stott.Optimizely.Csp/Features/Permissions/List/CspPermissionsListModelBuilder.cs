using System;
using System.Collections.Generic;
using System.Linq;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Permissions.Repository;

namespace Stott.Optimizely.Csp.Features.Permissions.List
{
    public class CspPermissionsListModelBuilder : ICspPermissionsListModelBuilder
    {
        private readonly ICspPermissionRepository _repository;

        public CspPermissionsListModelBuilder(ICspPermissionRepository repository)
        {
            _repository = repository;
        }

        public CspPermissionsListModel Build()
        {
            return new CspPermissionsListModel
            {
                AllowedDirectives = CspConstants.AllDirectives,
                Permissions = GetPermissions()
            };
        }

        private List<CspPermissionListModel> GetPermissions()
        {
            var cspSources = _repository.Get() ?? Enumerable.Empty<CspSource>();
            var permissions = cspSources.Select(x => new CspPermissionListModel
            {
                Id = x.Id.ExternalId,
                Source = x.Source,
                Directives = x.Directives
            }).ToList();

            if (!permissions.Any(x => x.Source.Equals(CspConstants.Sources.Self)))
            {
                permissions.Add(new CspPermissionListModel
                {
                    Id = Guid.Empty,
                    Source = CspConstants.Sources.Self,
                    Directives = string.Join(", ", new[] { CspConstants.Directives.DefaultSource })
                });
            }

            return permissions;
        }
    }
}
