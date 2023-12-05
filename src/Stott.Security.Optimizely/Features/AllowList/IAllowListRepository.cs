namespace Stott.Security.Optimizely.Features.AllowList;

using System.Threading.Tasks;

public interface IAllowListRepository
{
    Task<AllowListCollection> GetAllowListAsync(string allowListUrl);
}