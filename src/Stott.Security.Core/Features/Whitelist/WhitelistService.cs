namespace Stott.Security.Core.Features.Whitelist;

using System;
using System.Linq;
using System.Threading.Tasks;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Permissions.Service;

public class WhitelistService : IWhitelistService
{
    private readonly ICspWhitelistOptions _cspOptions;

    private readonly IWhitelistRepository _whitelistRepository;

    private readonly ICspPermissionService _cspPermissionService;

    private readonly ILoggingProvider _logger;

    public WhitelistService(
        ICspWhitelistOptions cspOptions,
        IWhitelistRepository whitelistRepository,
        ICspPermissionService cspPermissionService,
        ILoggingProviderFactory loggingProviderFactory)
    {
        _cspOptions = cspOptions ?? throw new ArgumentNullException(nameof(cspOptions));
        _whitelistRepository = whitelistRepository ?? throw new ArgumentNullException(nameof(whitelistRepository));
        _cspPermissionService = cspPermissionService ?? throw new ArgumentNullException(nameof(cspPermissionService));

        _logger = loggingProviderFactory.GetLogger(typeof(WhitelistService));
    }

    public async Task AddFromWhiteListToCspAsync(string violationSource, string violationDirective)
    {
        if (!_cspOptions.UseWhitelist
            || string.IsNullOrWhiteSpace(violationSource)
            || string.IsNullOrWhiteSpace(violationDirective)
            || !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
        {
            return;
        }

        try
        {
            var whitelist = await _whitelistRepository.GetWhitelistAsync(_cspOptions.WhitelistUrl);
            var whitelistMatch = whitelist.GetWhitelistMatch(violationSource, violationDirective);

            if (whitelistMatch != null)
            {
                await _cspPermissionService.AppendDirectiveAsync(whitelistMatch.SourceUrl, violationDirective);
            }
        }
        catch (Exception exception)
        {
            var errorMessage = $"{CspConstants.LogPrefix} Error encountered when adding '{violationSource}' and '{violationDirective}' to the whitelist.";
            _logger.Error(errorMessage, exception);

            throw new WhitelistException(errorMessage, exception);
        }
    }

    public async Task<bool> IsOnWhitelistAsync(string violationSource, string violationDirective)
    {
        if (!_cspOptions.UseWhitelist
            || string.IsNullOrWhiteSpace(violationSource)
            || string.IsNullOrWhiteSpace(violationDirective)
            || !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
        {
            return false;
        }

        try
        {
            _logger.Information($"{CspConstants.LogPrefix} Checking if '{violationSource}' and '{violationDirective}' is on the external whitelist.");

            var whitelist = await _whitelistRepository.GetWhitelistAsync(_cspOptions.WhitelistUrl);

            return whitelist?.IsOnWhitelist(violationSource, violationDirective) ?? false;
        }
        catch (Exception exception)
        {
            _logger.Error($"{CspConstants.LogPrefix} Error encountered when checking if '{violationSource}' and '{violationDirective}' is on the external whitelist.", exception);

            return false;
        }
    }

    public async Task<bool> IsWhitelistValidAsync(string whitelistUrl)
    {
        try
        {
            var whitelist = await _whitelistRepository.GetWhitelistAsync(whitelistUrl);

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
}
