using System.Threading.Tasks;

namespace Stott.Security.Core.Features.Whitelist
{
    public interface IWhitelistRepository
    {
        Task<WhitelistCollection> GetWhitelistAsync(string whitelistUrl);
    }
}
