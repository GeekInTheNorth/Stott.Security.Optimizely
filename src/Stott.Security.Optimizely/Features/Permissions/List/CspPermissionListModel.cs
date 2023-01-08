namespace Stott.Security.Optimizely.Features.Permissions.List;

using System;

using Stott.Security.Optimizely.Entities;

public class CspPermissionListModel
{
    public Guid Id { get; set; }

    public string Source { get; set; }

    public string Directives { get; set; }

    public CspPermissionListModel(CspSource cspSource)
    {
        Id = cspSource.Id;
        Source = cspSource.Source ?? string.Empty;
        Directives = cspSource.Directives ?? string.Empty;
    }

    public CspPermissionListModel(string? source, string? directives)
    {
        Id = Guid.Empty;
        Source = source ?? string.Empty;
        Directives = directives ?? string.Empty;
    }
}