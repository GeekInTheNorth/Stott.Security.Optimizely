using System.Threading.Tasks;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public interface IWhitelistService
    {
        Task AddFromWhiteListToCspAsync(string violationSource, string violationDirective);

        Task<bool> IsOnWhitelistAsync(string violationSource, string violationDirective);
    }
}
