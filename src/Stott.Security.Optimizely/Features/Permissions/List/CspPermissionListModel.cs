namespace Stott.Security.Optimizely.Features.Permissions.List;

using System;
using System.Linq;

using Newtonsoft.Json;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;

public sealed class CspPermissionListModel
{
    public Guid Id { get; set; }

    public string Source { get; set; }

    public string Directives { get; set; }

    [JsonIgnore]
    internal string SortSource { get; set; }

    public CspPermissionListModel(CspSource cspSource)
    {
        Id = cspSource.Id;
        Source = cspSource.Source ?? string.Empty;
        Directives = cspSource.Directives ?? string.Empty;

        SortSource = GetSortSource(Source);
    }

    public CspPermissionListModel(string? source, string? directives)
    {
        Id = Guid.Empty;
        Source = source ?? string.Empty;
        Directives = directives ?? string.Empty;

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
        catch(Exception) 
        {
            return source ?? string.Empty;
        }
    }
}