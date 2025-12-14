using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EPiServer.Web;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.SecurityTxt.Repository;
using Stott.Security.Optimizely.Features.Sites;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Service;

public class DefaultSecurityTxtContentService : ISecurityTxtContentService
{
    private readonly ISiteDefinitionRepository siteDefinitionRepository;

    private readonly ISecurityTxtContentRepository securityTxtContentRepository;

    public DefaultSecurityTxtContentService(
        ISiteDefinitionRepository siteDefinitionRepository,
        ISecurityTxtContentRepository securityTxtContentRepository)
    {
        this.siteDefinitionRepository = siteDefinitionRepository;
        this.securityTxtContentRepository = securityTxtContentRepository;
    }

    public async Task DeleteAsync(Guid id, string? modifiedBy)
    {
        if (Guid.Empty.Equals(id))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentException($"{nameof(modifiedBy)} must not be null or empty.", nameof(modifiedBy));
        }

        await securityTxtContentRepository.DeleteAsync(id, modifiedBy);
    }

    public bool DoesConflictExists(SaveSecurityTxtModel model)
    {
        var existingConfigurations = securityTxtContentRepository.GetAll() ?? new List<SecurityTxtEntity>(0);
        return existingConfigurations.Any(x => IsConflict(model, x));
    }

    public SiteSecurityTxtViewModel? Get(Guid id)
    {
        var securityTxt = securityTxtContentRepository.Get(id);
        if (securityTxt == null)
        {
            return null;
        }

        var sites = siteDefinitionRepository.List();
        var site = sites.FirstOrDefault(x => x.Id.Equals(securityTxt.SiteId));

        return ToModel(securityTxt, site);
    }

    public IList<SiteSecurityTxtViewModel> GetAll()
    {
        var allRecords = securityTxtContentRepository.GetAll();
        var sites = siteDefinitionRepository.List();
        var models = new List<SiteSecurityTxtViewModel>();

        foreach (var securityTxtRecord in allRecords)
        {
            var site = sites.FirstOrDefault(x => x.Id.Equals(securityTxtRecord.SiteId));
            models.Add(ToModel(securityTxtRecord, site));
        }

        return models.OrderBy(x => x.SiteName).ThenBy(x => x.SpecificHost).ToList();
    }

    public string? GetSecurityTxtContent(Guid siteId, string? host)
    {
        var data = securityTxtContentRepository.GetAll();
        var matchingConfig = data.FirstOrDefault(x => siteId.Equals(x.SiteId) && string.Equals(x.SpecificHost, host, StringComparison.OrdinalIgnoreCase)) ??
                             data.FirstOrDefault(x => siteId.Equals(x.SiteId) && string.IsNullOrWhiteSpace(x.SpecificHost)) ??
                             data.FirstOrDefault(x => Guid.Empty.Equals(x.SiteId));

        return matchingConfig?.Content;
    }

    public async Task SaveAsync(SaveSecurityTxtModel model, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentException($"{nameof(modifiedBy)} must not be null or empty.", nameof(modifiedBy));
        }

        if (model.SiteId != Guid.Empty)
        {
            var existingSite = siteDefinitionRepository.Get(model.SiteId);
            if (existingSite == null)
            {
                throw new ArgumentException($"{nameof(model)}.{nameof(model.SiteId)} does not correlate to a known site.", nameof(model));
            }
        }

        await securityTxtContentRepository.SaveAsync(model, modifiedBy);
    }

    private static SiteSecurityTxtViewModel ToModel(SecurityTxtEntity entity, SiteDefinition? siteDefinition)
    {
        var model = new SiteSecurityTxtViewModel
        {
            Id = entity.Id.ExternalId,
            SiteId = entity.SiteId,
            IsForWholeSite = entity.IsForWholeSite || string.IsNullOrWhiteSpace(entity.SpecificHost),
            SpecificHost = entity.SpecificHost,
            Content = entity.Content,
            IsEditable = true
        };

        if (siteDefinition != null)
        {
            model.SiteName = siteDefinition.Name;
            model.AvailableHosts = siteDefinition.Hosts.ToHostSummaries().ToList();
        }
        else if (entity.SiteId == Guid.Empty)
        {
            model.SiteName = "All Sites";
            model.AvailableHosts = SecurityTxtHelpers.CreateHostSummaries("All Hosts");
        }
        else
        {
            model.SiteName = "Unknown Site";
            model.IsEditable = false;
            model.AvailableHosts = SecurityTxtHelpers.CreateHostSummaries("Unknown Host");
        }

        return model;
    }

    private static bool IsConflict(SaveSecurityTxtModel model, SecurityTxtEntity entity)
    {
        var modelHost = model.SpecificHost ?? string.Empty;
        var entityHost = entity.SpecificHost ?? string.Empty;

        return Equals(model.SiteId, entity.SiteId) && !Equals(model.Id, entity.Id.ExternalId) &&
               string.Equals(modelHost, entityHost, StringComparison.OrdinalIgnoreCase);
    }
}
