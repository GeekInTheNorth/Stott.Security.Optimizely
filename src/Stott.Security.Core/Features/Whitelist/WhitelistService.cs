using System;
using System.Threading.Tasks;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Permissions.Repository;

namespace Stott.Security.Core.Features.Whitelist
{
    public class WhitelistService : IWhitelistService
    {
        private readonly ICspWhitelistOptions _cspOptions;

        private readonly IWhitelistRepository _whitelistRepository;

        private readonly ICspPermissionRepository _cspPermissionRepository;

        private readonly ILoggingProvider _logger;

        public WhitelistService(
            ICspWhitelistOptions cspOptions,
            IWhitelistRepository whitelistRepository,
            ICspPermissionRepository cspPermissionRepository,
            ILoggingProviderFactory loggingProviderFactory)
        {
            _cspOptions = cspOptions ?? throw new ArgumentNullException(nameof(cspOptions));
            _whitelistRepository = whitelistRepository ?? throw new ArgumentNullException(nameof(whitelistRepository));
            _cspPermissionRepository = cspPermissionRepository ?? throw new ArgumentNullException(nameof(cspPermissionRepository));

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
                    await _cspPermissionRepository.AppendDirectiveAsync(whitelistMatch.SourceUrl, violationDirective);
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
    }
}
