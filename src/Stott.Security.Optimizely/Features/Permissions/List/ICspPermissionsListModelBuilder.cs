using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Permissions.List
{
    public interface ICspPermissionsListModelBuilder
    {
        Task<CspPermissionsListModel> BuildAsync();
    }
}
