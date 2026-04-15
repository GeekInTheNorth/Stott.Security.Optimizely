namespace Stott.Security.Optimizely.Features.Csp.Permissions.List;

using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Extensions;

public sealed class CspPermissionListModel
{
    public Guid Id { get; set; }

    public string Source { get; set; }

    public string Directives { get; set; }

    public bool IsInherited { get; set; }

    public bool IsDescendant { get; set; }

    public string InheritedLabel { get; set; } = string.Empty;

    public string DescendantLabel { get; set; } = string.Empty;

    public int InheritanceLevel
    {
        get { return IsInherited ? 0 : (IsDescendant ? 2 : 1); }
    }

    [JsonIgnore]
    public IList<string> DirectiveList { get; set; }

    [JsonIgnore]
    internal string SortSource { get; set; }

    public CspPermissionListModel(CspSource cspSource, Guid? reqSiteId, string? reqHostName)
    {
        Id = cspSource.Id;
        Source = cspSource.Source ?? string.Empty;
        Directives = cspSource.Directives ?? string.Empty;
        DirectiveList = cspSource.Directives.SplitByComma();

        var hasSiteId = reqSiteId.HasValue && reqSiteId.Value != Guid.Empty;

        if (hasSiteId && cspSource.SiteId == null)
        {
            IsInherited = true;
            InheritedLabel = "Inherited from: 'All Sites'";
        }
        else if (!string.IsNullOrWhiteSpace(reqHostName) && string.IsNullOrWhiteSpace(cspSource.HostName))
        {
            IsInherited = true;
            InheritedLabel = $"Inherited from: '{reqSiteId}'";
        }
        else if (string.IsNullOrWhiteSpace(reqHostName) && !string.IsNullOrWhiteSpace(cspSource.HostName))
        {
            IsDescendant = true;
            DescendantLabel = $"Applies to host: '{cspSource.HostName}'";
        }
        else if (!hasSiteId && cspSource.SiteId != null)
        {
            IsDescendant = true;
            DescendantLabel = $"Applies to site: '{cspSource.SiteId}'";
        }

        SortSource = GetSortSource(Source);
    }

    private static string GetSortSource(string? source)
    {
        try
        {
            var sortSource = source ?? string.Empty;

            if (CspConstants.AllSources.Contains(sortSource))
            {
                return sortSource;
            }

            if (sortSource.Contains('*'))
            {
                sortSource = sortSource.Replace("*", "0");
            }

            if (Uri.IsWellFormedUriString(sortSource, UriKind.Absolute))
            {
                var host = new Uri(sortSource).Host;
                var components = host.Split('.');
                var primaryDomain = components.Length > 2 ? string.Join(".", components.Skip(components.Length - 2)) : host;

                return $"{primaryDomain} : {sortSource}";
            }

            return sortSource;
        }
        catch (Exception)
        {
            return source ?? string.Empty;
        }
    }
}
