using System.Threading.Tasks;

namespace Stott.Security.Core.Features.Permissions.List
{
    public interface ICspPermissionsListModelBuilder
    {
        Task<CspPermissionsListModel> BuildAsync();
    }
}
