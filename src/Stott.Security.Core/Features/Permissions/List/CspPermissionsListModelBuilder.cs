using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Permissions.Repository;

namespace Stott.Security.Core.Features.Permissions.List
{
    public class CspPermissionsListModelBuilder : ICspPermissionsListModelBuilder
    {
        private readonly ICspPermissionRepository _repository;

        public CspPermissionsListModelBuilder(ICspPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<CspPermissionsListModel> BuildAsync()
        {
            return new CspPermissionsListModel
            {
                AllowedDirectives = CspConstants.AllDirectives,
                Permissions = await GetPermissionsAsync()
            };
        }

        private async Task<List<CspPermissionListModel>> GetPermissionsAsync()
        {
            var cspSources = await _repository.GetAsync() ?? Enumerable.Empty<CspSource>();
            var permissions = cspSources.Select(x => new CspPermissionListModel
            {
                Id = x.Id,
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
