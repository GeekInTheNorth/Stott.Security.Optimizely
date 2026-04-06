namespace Stott.Security.Optimizely.Features.Csp.Permissions.List;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Extensions;

public sealed class CspPermissionListModel
{
    public Guid Id { get; set; }

    public string Source { get; set; }

    public string Directives { get; set; }

    public bool IsInherited { get; set; } = false;

    public bool IsDescendant { get; set; } = false;

    public string InheritedLabel { get; set; } = string.Empty;

    public string DescendantLabel { get; set; } = string.Empty;

    public int InheritanceLevel => IsInherited ? 0 : (IsDescendant ? 2 : 1);

    [JsonIgnore]
    public IList<string> DirectiveList { get; set; }

    [JsonIgnore]
    internal string SortSource { get; set; }

    public CspPermissionListModel(CspSource cspSource, string? reqAppId, string? reqHostName)
    {
        Id = cspSource.Id;
        Source = cspSource.Source ?? string.Empty;
        Directives = cspSource.Directives ?? string.Empty;
        DirectiveList = cspSource.Directives.SplitByComma();

        if (!string.IsNullOrWhiteSpace(reqAppId) && string.IsNullOrWhiteSpace(cspSource.AppId))
        {
            IsInherited = true;
            InheritedLabel = "Inherited from 'All Applications'";
        }
        else if (!string.IsNullOrWhiteSpace(reqHostName) && string.IsNullOrWhiteSpace(cspSource.HostName))
        {
            IsInherited = true;
            InheritedLabel = $"Inherited from '{reqAppId}'";
        }
        else if (string.IsNullOrWhiteSpace(reqHostName) && !string.IsNullOrWhiteSpace(cspSource.HostName))
        {
            IsDescendant = true;
            DescendantLabel = $"Applies to '{cspSource.HostName}' only";
        }
        else if (string.IsNullOrWhiteSpace(reqAppId) && !string.IsNullOrWhiteSpace(cspSource.AppId))
        {
            IsDescendant = true;
            DescendantLabel = $"Applies to all hosts for '{cspSource.AppId}' only";
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