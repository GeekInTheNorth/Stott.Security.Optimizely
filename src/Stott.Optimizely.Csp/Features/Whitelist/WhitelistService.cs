using System;
using System.Collections.Generic;

using EPiServer.Logging;

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

        public bool IsOnWhitelist(string violationSource, string directive)
        {
            if (!_whiteListOptions.UseWhitelist
                || string.IsNullOrWhiteSpace(violationSource)
                || string.IsNullOrWhiteSpace(directive))
            {
                return false;
            }

            var whitelist = _whitelistRepository.GetWhitelist(_whiteListOptions.WhitelistUrl);

            return true;
        }
    }

    public class WhiteListEntry
    {
        public string SourceUrl { get; set; }

        public List<string> Directives { get; set; }
    }

    public interface IWhitelistRepository
    {
        IList<WhiteListEntry> GetWhitelist(string whitelistUrl);
    }

    public class WhitelistRepository : IWhitelistRepository
    {
        public IList<WhiteListEntry> GetWhitelist(string whitelistUrl)
        {
            throw new NotImplementedException();
        }
    }
}
