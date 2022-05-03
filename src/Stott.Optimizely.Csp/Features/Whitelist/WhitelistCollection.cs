using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public class WhitelistCollection
    {
        private readonly IList<WhitelistEntry> _whitelistEntries;

        public WhitelistCollection(IList<WhitelistEntry> whitelistEntries)
        {
            _whitelistEntries = whitelistEntries ?? new List<WhitelistEntry>(0);
        }

        public IList<WhitelistEntry> Items => _whitelistEntries;

        public bool IsOnWhitelist(string violationSource, string violationDirective)
        {
            if (string.IsNullOrWhiteSpace(violationSource)
                || string.IsNullOrWhiteSpace(violationDirective)
                || !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
            {
                return false;
            }

            return _whitelistEntries?.Any(x => IsWhiteListMatch(x, violationSource, violationDirective)) ?? false;
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
