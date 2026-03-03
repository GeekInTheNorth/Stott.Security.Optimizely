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
                SetUpSystem();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error encountered during initial configuration: {ex.Message}");
            }
        }

        private void SetUpSystem()
        {
            var appRepository = ServiceLocator.Current.GetInstance<IApplicationRepository>();
            var sites = appRepository.List();
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
            var newSite = new Website("TestWebsite", homePageReference.ToReferenceWithoutVersion())
            {
                DisplayName = "Test Website"
            };

            newSite.Hosts.Add(new ApplicationHost("localhost:44344")
            {
                UseSecureConnection = true,
                Type = ApplicationHostType.Primary
            });
            
            appRepository.SaveAsync(newSite).GetAwaiter().GetResult();
        }
    }
}
