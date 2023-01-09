namespace Stott.Security.Optimizely.Features.Whitelist;

using System.Threading.Tasks;

public interface IWhitelistService
{
    Task AddFromWhiteListToCspAsync(string? violationSource, string? violationDirective);

    Task<bool> IsOnWhitelistAsync(string? violationSource, string? violationDirective);

    Task<bool> IsWhitelistValidAsync(string? whitelistUrl);
}