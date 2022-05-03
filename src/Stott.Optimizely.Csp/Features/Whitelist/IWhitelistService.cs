using System.Threading.Tasks;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public interface IWhitelistService
    {
        Task AddToWhitelist(string violationSource, string violationDirective);

        Task<bool> IsOnWhitelist(string violationSource, string violationDirective);
    }
}
