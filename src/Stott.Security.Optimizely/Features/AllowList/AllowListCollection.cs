namespace Stott.Security.Optimizely.Features.AllowList;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public sealed class AllowListCollection
{
    private readonly IList<AllowListEntry> _allowListEntries;

    public AllowListCollection(IList<AllowListEntry> allowListEntries)
    {
        _allowListEntries = allowListEntries ?? new List<AllowListEntry>(0);
    }

    public IList<AllowListEntry> Items => _allowListEntries;

    public bool IsOnAllowList(string? violationSource, string? violationDirective)
    {
        if (string.IsNullOrWhiteSpace(violationSource) || 
            string.IsNullOrWhiteSpace(violationDirective) || 
            !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
        {
            return false;
        }

        return _allowListEntries?.Any(x => IsAllowListMatch(x, violationSource, violationDirective)) ?? false;
    }

    public AllowListEntry? GetAllowListMatch(string? violationSource, string? violationDirective)
    {
        if (string.IsNullOrWhiteSpace(violationSource) || 
            string.IsNullOrWhiteSpace(violationDirective) || 
            !Uri.IsWellFormedUriString(violationSource, UriKind.Absolute))
        {
            return null;
        }

        return _allowListEntries?.FirstOrDefault(x => IsAllowListMatch(x, violationSource, violationDirective));
    }

    private static bool IsAllowListMatch(AllowListEntry allowListEntry, string violationSource, string violationDirective)
    {
        if (string.IsNullOrWhiteSpace(allowListEntry.SourceUrl) ||
            allowListEntry.Directives == null ||
            !allowListEntry.Directives.Contains(violationDirective))
        {
            return false;
        }

        var violationDomain = new Uri(violationSource, UriKind.Absolute).GetLeftPart(UriPartial.Authority);

        if (allowListEntry.SourceUrl.Contains('*'))
        {
            var regEx = @"([A-Za-z0-9_.\-~]{1,50})";
            var regExUrl = allowListEntry.SourceUrl.Replace("*", regEx);

            return Regex.IsMatch(violationDomain, regExUrl, RegexOptions.IgnoreCase);
        }

        var allowedDomain = new Uri(allowListEntry.SourceUrl, UriKind.Absolute).GetLeftPart(UriPartial.Authority);

        return string.Equals(violationDomain, allowedDomain, StringComparison.OrdinalIgnoreCase);
    }
}