using System.Collections.Generic;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public interface IWhitelistRepository
    {
        IList<WhitelistEntry> GetWhitelist(string whitelistUrl);
    }
}
