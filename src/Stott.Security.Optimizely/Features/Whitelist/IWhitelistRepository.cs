namespace Stott.Security.Optimizely.Features.Whitelist;

using System.Threading.Tasks;

public interface IWhitelistRepository
{
    Task<WhitelistCollection> GetWhitelistAsync(string whitelistUrl);
}