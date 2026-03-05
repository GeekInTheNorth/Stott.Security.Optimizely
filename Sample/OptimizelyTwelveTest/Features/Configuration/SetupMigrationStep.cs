using System;
using System.Globalization;
using System.Linq;

using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction.Migration;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;

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
                SetUpSystem();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error encountered during initial configuration: {ex.Message}");
            }
        }

        private void SetUpSystem()
        {
            var siteRepository = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
            var sites = siteRepository.List();
            if (sites.Any())
            {
                return;
            }

            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var culture = new CultureInfo("en");

            // Create HomePage
            var newHomePage = contentRepository.GetDefault<HomePage>(ContentReference.RootPage, culture);
            newHomePage.Name = "Home";
            newHomePage.Heading = "Home";
            newHomePage.MetaTitle = "Home";

            var homePageReference = contentRepository.Save(newHomePage, SaveAction.Publish, AccessLevel.NoAccess);

            // Create Not Found Page
            var newNotFoundPage = contentRepository.GetDefault<NotFoundPage>(ContentReference.RootPage, culture);
            newNotFoundPage.Name = "Not Found";
            newNotFoundPage.MetaTitle = "Home";

            var notFoundReference = contentRepository.Save(newNotFoundPage, SaveAction.Publish, AccessLevel.NoAccess);

            // Create SiteSettings
            var newSiteSettings = contentRepository.GetDefault<SiteSettingsPage>(homePageReference, culture);
            newSiteSettings.Name = "[Site Settings]";
            newSiteSettings.NotFoundPage = notFoundReference.ToReferenceWithoutVersion();
            newSiteSettings.SiteName = "Stott Security Dev Site";

            var siteSettingsReference = contentRepository.Save(newSiteSettings, SaveAction.Publish, AccessLevel.NoAccess);

            // Update Home Page
            var existingHomePage = contentRepository.Get<HomePage>(homePageReference, culture);
            var editableHomePage = existingHomePage.CreateWritableClone() as HomePage;
            editableHomePage.SiteSettings = siteSettingsReference.ToReferenceWithoutVersion();

            homePageReference = contentRepository.Save(editableHomePage, SaveAction.Publish, AccessLevel.NoAccess);

            // Create Site
            var newSite = SiteDefinition.Empty;
            var editableSite = newSite.CreateWritableClone();

            editableSite.Name = "Test Website";
            editableSite.StartPage = homePageReference.ToReferenceWithoutVersion();
            editableSite.SiteUrl = new Uri("https://localhost:44344/");
            editableSite.Hosts.Add(new HostDefinition
            {
                Name = "localhost:44344",
                Type = HostDefinitionType.Primary,
                UseSecureConnection = true
            });

            siteRepository.Save(editableSite);
        }
    }
}
