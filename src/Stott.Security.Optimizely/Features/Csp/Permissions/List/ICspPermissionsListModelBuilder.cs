using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Csp.Permissions.List
{
    public interface ICspPermissionsListModelBuilder
    {
        ICspPermissionsListModelBuilder WithSourceFilter(string? source);

        ICspPermissionsListModelBuilder WithDirectiveFilter(string? directive);

        ICspPermissionsListModelBuilder WithAppId(string? appId);

        ICspPermissionsListModelBuilder WithHostName(string? hostName);

        Task<CspPermissionsListModel> BuildAsync();
    }
}
