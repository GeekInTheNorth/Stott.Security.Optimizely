namespace Stott.Security.Optimizely.Features.Whitelist;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Permissions.Service;
using Stott.Security.Optimizely.Features.Settings.Repository;

public class WhitelistService : IWhitelistService
{
    private readonly ICspSettingsRepository _cspSettingsRepository;

    private readonly IWhitelistRepository _whitelistRepository;

    private readonly ICspPermissionService _cspPermissionService;

    private readonly ICacheWrapper _cacheWrapper;

    private readonly ILogger<WhitelistService> _logger;

    public WhitelistService(
        ICspSettingsRepository cspSettingsRepository,
        IWhitelistRepository whitelistRepository,
        ICspPermissionService cspPermissionService,
        ICacheWrapper cacheWrapper,
        ILogger<WhitelistService> logger)
    {
        _cspSettingsRepository = cspSettingsRepository ?? throw new ArgumentNullException(nameof(cspSettingsRepository));
        _whitelistRepository = whitelistRepository ?? throw new ArgumentNullException(nameof(whitelistRepository));
        _cspPermissionService = cspPermissionService ?? throw new ArgumentNullException(nameof(cspPermissionService));
        _cacheWrapper = cacheWrapper ?? throw new ArgumentNullException(nameof(cacheWrapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task AddFromWhiteListToCspAsync(string violationSource, string violationDirective)
    {
        var settings = await _cspSettingsRepository.GetAsync();
        if (!settings.IsWhitelistEnabled
            || string.IsNullOrWhiteSpace(violationSource)
            || string.IsNullOrWhiteSpace(violationDirective)
            || !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
        {
            return;
        }

        try
        {
            var whitelist = await GetWhitelistAsync(settings.WhitelistUrl);
            var whitelistMatch = whitelist.GetWhitelistMatch(violationSource, violationDirective);

            if (whitelistMatch != null)
            {
                await _cspPermissionService.AppendDirectiveAsync(whitelistMatch.SourceUrl, violationDirective);
            }
        }
        catch (Exception exception)
        {
            var errorMessage = $"{CspConstants.LogPrefix} Error encountered when adding '{violationSource}' and '{violationDirective}' to the whitelist.";
            _logger.LogError(exception, errorMessage);

            throw new WhitelistException(errorMessage, exception);
        }
    }

    public async Task<bool> IsOnWhitelistAsync(string violationSource, string violationDirective)
    {
        var settings = await _cspSettingsRepository.GetAsync();
        if (!settings.IsWhitelistEnabled
            || string.IsNullOrWhiteSpace(violationSource)
            || string.IsNullOrWhiteSpace(violationDirective)
            || !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
        {
            return false;
        }

        try
        {
            _logger.LogInformation($"{CspConstants.LogPrefix} Checking if '{violationSource}' and '{violationDirective}' is on the external whitelist.");

            var whitelist = await GetWhitelistAsync(settings.WhitelistUrl);

            return whitelist?.IsOnWhitelist(violationSource, violationDirective) ?? false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{CspConstants.LogPrefix} Error encountered when checking if '{violationSource}' and '{violationDirective}' is on the external whitelist.");

            return false;
        }
    }

    public async Task<bool> IsWhitelistValidAsync(string whitelistUrl)
    {
        try
        {
            var whitelist = await GetWhitelistAsync(whitelistUrl);

            return (whitelist?.Items?.Any() ?? false) && whitelist.Items.All(IsWhiteListEntryValid);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool IsWhiteListEntryValid(WhitelistEntry entry)
    {
        return !string.IsNullOrWhiteSpace(entry?.SourceUrl)
            && (entry?.Directives?.Any() ?? false)
            && (entry?.Directives?.All(x => !string.IsNullOrWhiteSpace(x)) ?? false);
    }

    private async Task<WhitelistCollection> GetWhitelistAsync(string whitelistUrl)
    {
        var cacheKey = $"csp-whitelist-{GetChecksum(whitelistUrl)}";
        var cachedWhitelist = _cacheWrapper.Get<WhitelistCollection>(cacheKey);
        if (cachedWhitelist != null)
        {
            return cachedWhitelist;
        }

        var whitelist = await _whitelistRepository.GetWhitelistAsync(whitelistUrl);
        _cacheWrapper.Add(cacheKey, whitelist);

        return whitelist;
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
