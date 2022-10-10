using System.Threading.Tasks;

namespace Stott.Security.Core.Features.Whitelist
{
    public interface IWhitelistService
    {
        Task AddFromWhiteListToCspAsync(string violationSource, string violationDirective);

        Task<bool> IsOnWhitelistAsync(string violationSource, string violationDirective);

        Task<bool> IsWhitelistValidAsync(string whitelistUrl);
    }
}
