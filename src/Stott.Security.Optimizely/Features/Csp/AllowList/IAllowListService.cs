namespace Stott.Security.Optimizely.Features.Csp.AllowList;

using System;
using System.Threading.Tasks;

public interface IAllowListService
{
    Task AddFromAllowListToCspAsync(string? violationSource, string? violationDirective, Guid? siteId, string? hostName);

    Task<bool> IsOnAllowListAsync(string? violationSource, string? violationDirective, Guid? siteId, string? hostName);

    Task<bool> IsAllowListValidAsync(string? allowListUrl);
}
