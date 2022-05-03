using System;
using System.Threading.Tasks;

using EPiServer.Logging;

using Stott.Optimizely.Csp.Common;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public class WhitelistService : IWhitelistService
    {
        private readonly ICspWhitelistOptions _whiteListOptions;

        private readonly IWhitelistRepository _whitelistRepository;

        public WhitelistService(
            ICspWhitelistOptions whiteListOptions, 
            IWhitelistRepository whitelistRepository)
        {
            _whiteListOptions = whiteListOptions ?? throw new ArgumentNullException(nameof(whiteListOptions));
            _whitelistRepository = whitelistRepository ?? throw new ArgumentNullException(nameof(whitelistRepository));
        }

        private ILogger _logger = LogManager.GetLogger(typeof(WhitelistService));

        public void AddToWhitelist(string violationSource, string directive)
        {
            // TODO
        }

        public async Task<bool> IsOnWhitelist(string violationSource, string violationDirective)
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

                var whitelist = await _whitelistRepository.GetWhitelist(_whiteListOptions.WhitelistUrl);

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
