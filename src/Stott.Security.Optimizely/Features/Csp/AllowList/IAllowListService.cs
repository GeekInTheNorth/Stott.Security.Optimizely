namespace Stott.Security.Optimizely.Features.Csp.AllowList;

using System.Threading.Tasks;

public interface IAllowListService
{
    Task AddFromAllowListToCspAsync(string? violationSource, string? violationDirective, string? appId, string? hostName);

    Task<bool> IsOnAllowListAsync(string? violationSource, string? violationDirective, string? appId, string? hostName);

    Task<bool> IsAllowListValidAsync(string? allowListUrl);
}