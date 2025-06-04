namespace Stott.Security.Optimizely.Features.Csp.Settings;

using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Web;

public sealed class CspSummaryModel
{
    public Guid Id { get; }

    public string Name { get; }

    public string Scope { get; }
    
    public string Status { get; }

    public CspSummaryModel(Entities.CspSettings settings, List<SiteDefinition> siteDefinitions)
    {
        var specificSite = siteDefinitions.FirstOrDefault();
        var behaviour = "Content";
        var descriptions = new List<string>();

        if (specificSite is not null)
        {
            descriptions.Add($"Applied for site: {specificSite.Name}");
        }
        else
        {
            descriptions.Add("Applied globally");
        }

        if (behaviour == "Content")
        {
            descriptions.Add("for content routes");
        }
        else if (behaviour == "NonContent")
        {
            descriptions.Add("for non-content routes");
        }
        else
        {
            descriptions.Add("for all routes");
        }

        Id = settings.Id;
        Name = "Default Policy";
        Scope = string.Join(", ", descriptions);

        if (settings.IsEnabled && settings.IsReportOnly)
        {
            Status = "Report Only";
        }
        else if (settings.IsEnabled)
        {
            Status = "On";
        }
        else
        {
            Status = "Off";
        }
    }
}