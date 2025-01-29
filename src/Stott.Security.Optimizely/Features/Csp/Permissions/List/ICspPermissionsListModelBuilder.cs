using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Csp.Permissions.List
{
    public interface ICspPermissionsListModelBuilder
    {
        ICspPermissionsListModelBuilder WithSourceFilter(string? source);

        ICspPermissionsListModelBuilder WithDirectiveFilter(string? directive);

        Task<CspPermissionsListModel> BuildAsync();
    }
}
