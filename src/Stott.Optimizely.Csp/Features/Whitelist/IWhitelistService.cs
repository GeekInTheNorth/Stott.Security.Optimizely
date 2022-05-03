using System.Threading.Tasks;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public interface IWhitelistService
    {
        void AddToWhitelist(string violationSource, string directive);

        Task<bool> IsOnWhitelist(string violationSource, string directive);
    }
}
