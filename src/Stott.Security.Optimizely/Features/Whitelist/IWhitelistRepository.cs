using System.Threading.Tasks;

namespace Stott.Security.Optimizely.Features.Whitelist
{
    public interface IWhitelistRepository
    {
        Task<WhitelistCollection> GetWhitelistAsync(string whitelistUrl);
    }
}
