using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Applications;
using Stott.Security.Optimizely.Features.SecurityTxt;

namespace Stott.Security.Optimizely.Features.Applications;

public sealed class ApplicationDefinitionService(IApplicationRepository applicationRepository) : IApplicationDefinitionService
{
    public async Task<IEnumerable<ApplicationViewModel>> GetAllApplicationsAsync()
    {
        var data = await applicationRepository.ListAsync();
        return ToModels(data);
    }

    public async Task<ApplicationViewModel?> GetApplicationByIdAsync(string? appId)
    {
        if (string.IsNullOrWhiteSpace(appId))
        {
            return null;
        }

        var application = await applicationRepository.GetAsync(appId);

        return application switch
        {
            Website website => ToModel(website),
            RemoteWebsite remoteWebsite => ToModel(remoteWebsite),
            _ => null
        };
    }

    private static IEnumerable<ApplicationViewModel> ToModels(IEnumerable<Application> applications)
    {
        if (!applications.Any())
        {
            yield break;
        }

        foreach (var application in applications)
        {
            if (application is Website website)
            {
                yield return ToModel(website);
            }
            else if (application is RemoteWebsite remoteWebsite)
            {
                yield return ToModel(remoteWebsite);
            }
        }
    }

    private static ApplicationViewModel ToModel(Website website)
    {
        return new ApplicationViewModel
        {
            AppId = website.Name,
            AppName = website.DisplayName,
            AvailableHosts = [.. SecurityTxtHelpers.CreateHostSummaries(website.Hosts)]
        };
    }

    private static ApplicationViewModel ToModel(RemoteWebsite website)
    {
        return new ApplicationViewModel
        {
            AppId = website.Name,
            AppName = website.DisplayName,
            AvailableHosts = SecurityTxtHelpers.CreateHostSummaries("All Hosts")
        };
    }
}