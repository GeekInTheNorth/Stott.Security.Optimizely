using System;
using System.Globalization;
using System.Linq;

using EPiServer;
using EPiServer.Applications;
using EPiServer.Core;
using EPiServer.DataAbstraction.Migration;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;

using OptimizelyTwelveTest.Features.Home;
using OptimizelyTwelveTest.Features.NotFound;
using OptimizelyTwelveTest.Features.Settings;

namespace OptimizelyTwelveTest.Features.Configuration
{
    public class SetupMigrationStep : MigrationStep
    {
        public override void AddChanges()
        {
            try
            {
                var appRepository = ServiceLocator.Current.GetInstance<IApplicationRepository>();
                if (!ConfigurationExists(appRepository))
                {
                    var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
                    SetUpSystem(appRepository, contentRepository, 1, 5000, 5001);
                    SetUpSystem(appRepository, contentRepository, 2, 5002, 5003);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error encountered during initial configuration: {ex.Message}");
            }
        }

        private static bool ConfigurationExists(IApplicationRepository appRepository)
        {
            var sites = appRepository.List();
            return sites.Any();
        }

        private static void SetUpSystem(IApplicationRepository appRepository, IContentRepository contentRepository, int siteNumber, int portOne, int portTwo)
        {
            var culture = new CultureInfo("en");

            // Create HomePage
            var newHomePage = contentRepository.GetDefault<HomePage>(ContentReference.RootPage, culture);
            newHomePage.Name = $"Home {siteNumber}";
            newHomePage.Heading = $"Home {siteNumber}";
            newHomePage.MetaTitle = $"Home {siteNumber}";

            var homePageReference = contentRepository.Save(newHomePage, SaveAction.Publish, AccessLevel.NoAccess);

            // Create Not Found Page
            var newNotFoundPage = contentRepository.GetDefault<NotFoundPage>(homePageReference, culture);
            newNotFoundPage.Name = $"Not Found {siteNumber}";
            newNotFoundPage.MetaTitle = $"Not Found {siteNumber}";

            var notFoundReference = contentRepository.Save(newNotFoundPage, SaveAction.Publish, AccessLevel.NoAccess);

            // Create SiteSettings
            var newSiteSettings = contentRepository.GetDefault<SiteSettingsPage>(homePageReference, culture);
            newSiteSettings.Name = $"[Site Settings {siteNumber}]";
            newSiteSettings.NotFoundPage = notFoundReference.ToReferenceWithoutVersion();
            newSiteSettings.SiteName = $"Site {siteNumber}";

            var siteSettingsReference = contentRepository.Save(newSiteSettings, SaveAction.Publish, AccessLevel.NoAccess);

            // Update Home Page
            var existingHomePage = contentRepository.Get<HomePage>(homePageReference, culture);
            var editableHomePage = existingHomePage.CreateWritableClone() as HomePage;
            editableHomePage.SiteSettings = siteSettingsReference.ToReferenceWithoutVersion();

            homePageReference = contentRepository.Save(editableHomePage, SaveAction.Publish, AccessLevel.NoAccess);

            // Create Site
            var newSite = new Website($"TestWebsite{siteNumber}", homePageReference.ToReferenceWithoutVersion())
            {
                DisplayName = $"Test Website {siteNumber}"
            };

            newSite.Hosts.Add(new ApplicationHost($"localhost:{portOne}")
            {
                UseSecureConnection = true,
                Type = ApplicationHostType.Primary
            });

            newSite.Hosts.Add(new ApplicationHost($"localhost:{portTwo}")
            {
                UseSecureConnection = true,
                Type = ApplicationHostType.Edit
            });

            appRepository.SaveAsync(newSite).GetAwaiter().GetResult();
        }
    }
}
