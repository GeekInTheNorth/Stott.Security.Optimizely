using System;
using System.Linq;
using System.Text.RegularExpressions;

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
            throw new NotImplementedException();
        }

        public bool IsOnWhitelist(string violationSource, string violationDirective)
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

                var whitelist = _whitelistRepository.GetWhitelist(_whiteListOptions.WhitelistUrl);

                return whitelist?.Any(x => IsWhiteListMatch(x, violationSource, violationDirective)) ?? false;
            }
            catch(Exception exception)
            {
                _logger.Error($"{CspConstants.LogPrefix} Error encountered when checking if '{violationSource}' and '{violationDirective}' is on the external whitelist.", exception);

                return false;
            }
        }

        private static bool IsWhiteListMatch(WhitelistEntry whiteListEntry, string violationSource, string violationDirective)
        {
            if (whiteListEntry?.Directives == null || !whiteListEntry.Directives.Contains(violationDirective))
            {
                return false;
            }

            var violationDomain = new Uri(violationSource, UriKind.Absolute).GetLeftPart(UriPartial.Authority);

            if (whiteListEntry.SourceUrl.Contains('*'))
            {
                var regEx = @"([A-Za-z0-9_.\-~]{1,50})";
                var regExUrl = whiteListEntry.SourceUrl.Replace("*", regEx);

                return Regex.IsMatch(violationDomain, regExUrl, RegexOptions.IgnoreCase);
            }

            var allowedDomain = new Uri(whiteListEntry.SourceUrl, UriKind.Absolute).GetLeftPart(UriPartial.Authority);

            return string.Equals(violationDomain, allowedDomain, StringComparison.OrdinalIgnoreCase);
        }
    }
}
