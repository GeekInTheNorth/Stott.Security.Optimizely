using System.Threading.Tasks;

namespace Stott.Optimizely.Csp.Features.Permissions.List
{
    public interface ICspPermissionsListModelBuilder
    {
        Task<CspPermissionsListModel> BuildAsync();
    }
}
