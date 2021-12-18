using System;
using System.Collections.Generic;
using System.Linq;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Permissions.Repository;

namespace Stott.Optimizely.Csp.Features.Permissions.List
{
    public class CspPermissionsViewModelBuilder : ICspPermissionsViewModelBuilder
    {
        private readonly ICspPermissionsRepository _repository;

        public CspPermissionsViewModelBuilder(ICspPermissionsRepository repository)
        {
            _repository = repository;
        }

        public CspPermissionsViewModel Build()
        {
            return new CspPermissionsViewModel
            {
                AllowedDirectives = CspConstants.AllDirectives,
                Permissions = GetPermissions()
            };
        }

        private List<CspPermissionViewModel> GetPermissions()
        {
            var cspSources = _repository.List() ?? Enumerable.Empty<CspSource>();
            var permissions = cspSources.Select(x => new CspPermissionViewModel
            {
                Id = x.Id.ExternalId,
                Source = x.Source,
                Directives = x.Directives
            }).ToList();

            if (!permissions.Any(x => x.Source.Equals(CspConstants.Sources.Self)))
            {
                permissions.Add(new CspPermissionViewModel
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
