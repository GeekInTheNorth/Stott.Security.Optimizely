using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Permissions.List
{
    public interface ICspPermissionsListModelBuilder
    {
        ICspPermissionsListModelBuilder WithSourceFilter(string? source);

        ICspPermissionsListModelBuilder WithDirectiveFilter(string? directive);

        Task<CspPermissionsListModel> BuildAsync();
    }
}
