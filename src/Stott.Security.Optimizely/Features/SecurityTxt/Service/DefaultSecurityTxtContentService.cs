using System;
using System.Collections.Generic;
using System.Linq;

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

    public void Delete(Guid id, string? modifiedBy)
    {
        if (Guid.Empty.Equals(id))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentException($"{nameof(modifiedBy)} must not be null or empty.", nameof(modifiedBy));
        }

        securityTxtContentRepository.Delete(id, modifiedBy);
    }

    public bool DoesConflictExists(SaveSecurityTxtModel model)
    {
        var existingConfigurations = securityTxtContentRepository.GetAll() ?? new List<SecurityTxtEntity>(0);
        return existingConfigurations.Any(x => IsConflict(model, x));
    }

    public SiteSecurityTxtViewModel Get(Guid id)
    {
        var securityTxt = securityTxtContentRepository.Get(id);
        if (securityTxt == null)
        {
            throw new SecurityEntityNotFoundException(id);
        }

        var sites = siteDefinitionRepository.List();
        var site = sites.FirstOrDefault(x => x.Id.Equals(securityTxt.SiteId));
        if (site == null)
        {
            throw new SecurityEntityNotFoundException($"Security.Txt entity with id '{id}' not match any site definitions.");
        }

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
            if (site != null)
            {
                models.Add(ToModel(securityTxtRecord, site));
            }
        }

        return models.OrderBy(x => x.SiteName).ThenBy(x => x.SpecificHost).ToList();
    }

    public SiteSecurityTxtViewModel GetDefault(Guid siteId)
    {
        var site = siteDefinitionRepository.Get(siteId);
        if (site == null)
        {
            throw new ArgumentException($"{nameof(siteId)} does not correlate to a known site.", nameof(siteId));
        }

        return ToModel(site);
    }

    public string? GetDefaultSecurityTxtContent()
    {
        return string.Empty;
    }

    public string? GetSecurityTxtContent(Guid siteId, string? host)
    {
        var data = securityTxtContentRepository.GetAllForSite(siteId) ?? new List<SecurityTxtEntity>(0);
        var matchingConfig = data.FirstOrDefault(x => string.Equals(x.SpecificHost, host, StringComparison.OrdinalIgnoreCase)) ??
                           data.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.SpecificHost));

        return matchingConfig?.Content;
    }

    public void Save(SaveSecurityTxtModel model, string? modifiedBy)
    {
        if (Guid.Empty.Equals(model.SiteId))
        {
            throw new ArgumentException($"{nameof(model)}.{nameof(model.SiteId)} must not be null or empty.", nameof(model));
        }

        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentException($"{nameof(modifiedBy)} must not be null or empty.", nameof(modifiedBy));
        }

        var existingSite = siteDefinitionRepository.Get(model.SiteId);
        if (existingSite == null)
        {
            throw new ArgumentException($"{nameof(model)}.{nameof(model.SiteId)} does not correlate to a known site.", nameof(model));
        }

        securityTxtContentRepository.Save(model, modifiedBy);
    }

    private static SiteSecurityTxtViewModel ToModel(SecurityTxtEntity entity, SiteDefinition siteDefinition)
    {
        return new SiteSecurityTxtViewModel
        {
            Id = entity.Id.ExternalId,
            SiteId = entity.SiteId,
            IsForWholeSite = entity.IsForWholeSite || string.IsNullOrWhiteSpace(entity.SpecificHost),
            SpecificHost = entity.SpecificHost,
            Content = entity.Content,
            SiteName = siteDefinition.Name,
            AvailableHosts = siteDefinition.Hosts.ToHostSummaries().ToList()
        };
    }

    private SiteSecurityTxtViewModel ToModel(SiteDefinition siteDefinition)
    {
        return new SiteSecurityTxtViewModel
        {
            Id = Guid.Empty,
            SiteId = siteDefinition.Id,
            IsForWholeSite = true,
            Content = GetDefaultSecurityTxtContent(),
            SiteName = siteDefinition.Name,
            AvailableHosts = siteDefinition.Hosts.ToHostSummaries().ToList()
        };
    }

    private static bool IsConflict(SaveSecurityTxtModel model, SecurityTxtEntity entity)
    {
        var modelHost = model.SpecificHost ?? string.Empty;
        var entityHost = entity.SpecificHost ?? string.Empty;

        return Equals(model.SiteId, entity.SiteId) && !Equals(model.Id, entity.Id.ExternalId) &&
               string.Equals(modelHost, entityHost, StringComparison.OrdinalIgnoreCase);
    }
}
