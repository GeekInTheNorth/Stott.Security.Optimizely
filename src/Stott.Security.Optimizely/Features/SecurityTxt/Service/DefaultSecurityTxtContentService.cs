using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Applications;
using Stott.Security.Optimizely.Features.SecurityTxt.Models;
using Stott.Security.Optimizely.Features.SecurityTxt.Repository;

namespace Stott.Security.Optimizely.Features.SecurityTxt.Service;

public class DefaultSecurityTxtContentService(
    IApplicationDefinitionService appService, 
    ISecurityTxtContentRepository securityTxtContentRepository)
    : ISecurityTxtContentService
{
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
        var existingConfigurations = securityTxtContentRepository.GetAll();
        return existingConfigurations.Any(x => IsConflict(model, x));
    }

    public async Task<SiteSecurityTxtViewModel?> GetAsync(Guid id)
    {
        var securityTxt = securityTxtContentRepository.Get(id);
        if (securityTxt == null)
        {
            return null;
        }

        var application = await appService.GetApplicationByIdAsync(securityTxt.AppId);
        
        return ToModel(securityTxt, application);
    }

    public async Task<IList<SiteSecurityTxtViewModel>> GetAllAsync()
    {
        var allRecords = securityTxtContentRepository.GetAll();
        var applications = await appService.GetAllApplicationsAsync();
        var models = new List<SiteSecurityTxtViewModel>();

        foreach (var securityTxtRecord in allRecords)
        {
            var site = applications.FirstOrDefault(x => string.Equals(x.AppId, securityTxtRecord.AppId));
            models.Add(ToModel(securityTxtRecord, site));
        }

        return models.OrderBy(x => x.AppName).ThenBy(x => x.SpecificHost).ToList();
    }

    public string? GetSecurityTxtContent(string? appId, string? host)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            return null;
        }

        var data = securityTxtContentRepository.GetAll();
        var matchingConfig = data.FirstOrDefault(x => appId.Equals(x.AppId) && string.Equals(x.SpecificHost, host, StringComparison.OrdinalIgnoreCase)) ??
                             data.FirstOrDefault(x => appId.Equals(x.AppId) && string.IsNullOrWhiteSpace(x.SpecificHost)) ??
                             data.FirstOrDefault(x => x.AppId is null);

        return matchingConfig?.Content;
    }

    public async Task SaveAsync(SaveSecurityTxtModel model, string? modifiedBy)
    {
        if (string.IsNullOrWhiteSpace(modifiedBy))
        {
            throw new ArgumentException($"{nameof(modifiedBy)} must not be null or empty.", nameof(modifiedBy));
        }

        if (!string.IsNullOrWhiteSpace(model.AppId))
        {
            var existingApp = await appService.GetApplicationByIdAsync(model.AppId);
            if (existingApp == null)
            {
                throw new ArgumentException($"{nameof(model)}.{nameof(model.AppId)} does not correlate to a known application.", nameof(model));
            }
        }

        await securityTxtContentRepository.SaveAsync(model, modifiedBy);
    }

    private static SiteSecurityTxtViewModel ToModel(SecurityTxtEntity entity, ApplicationViewModel? application)
    {
        var model = new SiteSecurityTxtViewModel
        {
            Id = entity.Id.ExternalId,
            AppId = entity.AppId,
            IsForWholeApplication = entity.IsForWholeSite || string.IsNullOrWhiteSpace(entity.SpecificHost),
            SpecificHost = entity.SpecificHost,
            Content = entity.Content,
            IsEditable = true
        };

        if (application != null)
        {
            model.AppName = application.AppName;
            model.AvailableHosts = application.AvailableHosts;
        }
        else if (entity.AppId is null)
        {
            model.AppName = "All Sites";
            model.AvailableHosts = SecurityTxtHelpers.CreateHostSummaries("All Hosts");
        }
        else
        {
            model.AppName = "Unknown Site";
            model.IsEditable = false;
            model.AvailableHosts = SecurityTxtHelpers.CreateHostSummaries("Unknown Host");
        }

        return model;
    }

    private static bool IsConflict(SaveSecurityTxtModel model, SecurityTxtEntity entity)
    {
        var modelHost = model.SpecificHost ?? string.Empty;
        var entityHost = entity.SpecificHost ?? string.Empty;

        return Equals(model.AppId, entity.AppId) && !Equals(model.Id, entity.Id.ExternalId) &&
               string.Equals(modelHost, entityHost, StringComparison.OrdinalIgnoreCase);
    }
}
