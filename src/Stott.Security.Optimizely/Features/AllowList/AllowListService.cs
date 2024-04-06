namespace Stott.Security.Optimizely.Features.AllowList;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Permissions.Service;
using Stott.Security.Optimizely.Features.Settings.Service;

internal sealed class AllowListService : IAllowListService
{
    private readonly ICspSettingsService _cspSettingsService;

    private readonly IAllowListRepository _allowListRepository;

    private readonly ICspPermissionService _cspPermissionService;

    private readonly ICacheWrapper _cacheWrapper;

    private readonly ILogger<IAllowListService> _logger;

    public AllowListService(
        ICspSettingsService cspSettingsService,
        IAllowListRepository allowListRepository,
        ICspPermissionService cspPermissionService,
        ICacheWrapper cacheWrapper,
        ILogger<IAllowListService> logger)
    {
        _cspSettingsService = cspSettingsService ?? throw new ArgumentNullException(nameof(cspSettingsService));
        _allowListRepository = allowListRepository ?? throw new ArgumentNullException(nameof(allowListRepository));
        _cspPermissionService = cspPermissionService ?? throw new ArgumentNullException(nameof(cspPermissionService));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task AddFromAllowListToCspAsync(string? violationSource, string? violationDirective)
    {
        var settings = await _cspSettingsService.GetAsync();
        if (!settings.IsAllowListEnabled
            || string.IsNullOrWhiteSpace(violationSource)
            || string.IsNullOrWhiteSpace(violationDirective)
            || !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
        {
            return;
        }

        try
        {
            var allowList = await GetAllowListAsync(settings.AllowListUrl);
            var allowListMatch = allowList.GetAllowListMatch(violationSource, violationDirective);

            if (allowListMatch != null)
            {
                await _cspPermissionService.AppendDirectiveAsync(allowListMatch.SourceUrl, violationDirective, "Allow List Automation");
            }
        }
        catch (Exception exception)
        {
            var errorMessage = $"{CspConstants.LogPrefix} Error encountered when adding '{violationSource}' and '{violationDirective}' to the allow list.";
            _logger.LogError(exception, errorMessage);

            throw new AllowListException(errorMessage, exception);
        }
    }

    public async Task<bool> IsOnAllowListAsync(string? violationSource, string? violationDirective)
    {
        var settings = await _cspSettingsService.GetAsync();
        if (!settings.IsAllowListEnabled
            || string.IsNullOrWhiteSpace(violationSource)
            || string.IsNullOrWhiteSpace(violationDirective)
            || !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
        {
            return false;
        }

        try
        {
            _logger.LogInformation($"{CspConstants.LogPrefix} Checking if '{violationSource}' and '{violationDirective}' is on the external allow list.");

            var allowList = await GetAllowListAsync(settings.AllowListUrl);

            return allowList?.IsOnAllowList(violationSource, violationDirective) ?? false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Error encountered when checking if '{violationSource}' and '{violationDirective}' is on the external allow list.");

            return false;
        }
    }

    public async Task<bool> IsAllowListValidAsync(string? allowListUrl)
    {
        if (string.IsNullOrWhiteSpace(allowListUrl))
        {
            return false;
        }

        try
        {
            var allowList = await GetAllowListAsync(allowListUrl);

            return (allowList?.Items?.Any() ?? false) && allowList.Items.All(IsAllowListEntryValid);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool IsAllowListEntryValid(AllowListEntry entry)
    {
        return !string.IsNullOrWhiteSpace(entry?.SourceUrl)
            && (entry?.Directives?.Any() ?? false)
            && (entry?.Directives?.All(x => !string.IsNullOrWhiteSpace(x)) ?? false);
    }

    private async Task<AllowListCollection> GetAllowListAsync(string allowListUrl)
    {
        var cacheKey = $"csp-allowlist-{GetChecksum(allowListUrl)}";
        var cachedAllowList = _cacheWrapper.Get<AllowListCollection>(cacheKey);
        if (cachedAllowList != null)
        {
            return cachedAllowList;
        }

        var allowList = await _allowListRepository.GetAllowListAsync(allowListUrl);
        _cacheWrapper.Add(cacheKey, allowList);

        return allowList;
    }

    private static string GetChecksum(string stringToHash)
    {
        if (string.IsNullOrWhiteSpace(stringToHash))
        {
            return "not-defined";
        }

        using var md5 = MD5.Create();

        var stringBytes = Encoding.ASCII.GetBytes(stringToHash);

        return BitConverter.ToString(md5.ComputeHash(stringBytes));
    }
}