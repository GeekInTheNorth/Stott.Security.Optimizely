using System;
using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Csp.Permissions.List
{
    public interface ICspPermissionsListModelBuilder
    {
        ICspPermissionsListModelBuilder WithSourceFilter(string? source);

        ICspPermissionsListModelBuilder WithDirectiveFilter(string? directive);

        ICspPermissionsListModelBuilder WithSiteId(Guid? siteId);

        ICspPermissionsListModelBuilder WithHostName(string? hostName);

        Task<CspPermissionsListModel> BuildAsync();
    }
}
