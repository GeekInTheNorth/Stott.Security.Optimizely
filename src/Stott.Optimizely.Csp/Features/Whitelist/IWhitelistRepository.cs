using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public interface IWhitelistRepository
    {
        Task<WhitelistCollection> GetWhitelistAsync(string whitelistUrl);
    }
}
