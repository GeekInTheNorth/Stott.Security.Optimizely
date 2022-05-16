using System;
using System.Threading.Tasks;

using EPiServer.Logging;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Features.Permissions.Repository;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public class WhitelistService : IWhitelistService
    {
        private readonly ICspWhitelistOptions _whiteListOptions;

        private readonly IWhitelistRepository _whitelistRepository;

        private readonly ICspPermissionRepository _cspPermissionRepository;

        private readonly ILogger _logger = LogManager.GetLogger(typeof(WhitelistService));

        public WhitelistService(
            ICspWhitelistOptions whiteListOptions,
            IWhitelistRepository whitelistRepository, 
            ICspPermissionRepository cspPermissionRepository)
        {
            _whiteListOptions = whiteListOptions ?? throw new ArgumentNullException(nameof(whiteListOptions));
            _whitelistRepository = whitelistRepository ?? throw new ArgumentNullException(nameof(whitelistRepository));
            _cspPermissionRepository = cspPermissionRepository ?? throw new ArgumentNullException(nameof(cspPermissionRepository));
        }

        public async Task AddFromWhiteListToCspAsync(string violationSource, string violationDirective)
        {
            if (!_whiteListOptions.UseWhitelist
                || string.IsNullOrWhiteSpace(violationSource)
                || string.IsNullOrWhiteSpace(violationDirective)
                || !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
            {
                return;
            }

            try
            {
                var whitelist = await _whitelistRepository.GetWhitelistAsync(_whiteListOptions.WhitelistUrl);
                var whitelistMatch = whitelist.GetWhitelistMatch(violationSource, violationDirective);

                if (whitelistMatch != null)
                {
                    await _cspPermissionRepository.AppendDirectiveAsync(whitelistMatch.SourceUrl, violationDirective);
                }
            }
            catch(Exception exception)
            {
                var errorMessage = $"{CspConstants.LogPrefix} Error encountered when adding '{violationSource}' and '{violationDirective}' to the whitelist.";
                _logger.Error(errorMessage, exception);

                throw new WhitelistException(errorMessage, exception);
            }
        }

        public async Task<bool> IsOnWhitelistAsync(string violationSource, string violationDirective)
        {
            if (!_whiteListOptions.UseWhitelist
                || string.IsNullOrWhiteSpace(violationSource)
                || string.IsNullOrWhiteSpace(violationDirective)
                || !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
            {
                return false;
            }

            try
            {
                _logger.Information($"{CspConstants.LogPrefix} Checking if '{violationSource}' and '{violationDirective}' is on the external whitelist.");

                var whitelist = await _whitelistRepository.GetWhitelistAsync(_whiteListOptions.WhitelistUrl);

                return whitelist?.IsOnWhitelist(violationSource, violationDirective) ?? false;
            }
            catch(Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Error encountered when checking if '{violationSource}' and '{violationDirective}' is on the external whitelist.", exception);

                return false;
            }
        }
    }
}
