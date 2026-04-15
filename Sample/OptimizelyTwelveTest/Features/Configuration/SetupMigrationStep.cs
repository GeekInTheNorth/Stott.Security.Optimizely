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
                var siteRepository = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
                if (siteRepository.List().Any())
                {
                    return;
                }

                var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

                // Site 1 — primary on 44344, CMS/edit on 44345
                SetUpSite(siteRepository, contentRepository, siteNumber: 1, primaryHost: "localhost:44344", editHost: "localhost:44345");

                // Site 2 — primary on 44346, CMS/edit on 44347
                SetUpSite(siteRepository, contentRepository, siteNumber: 2, primaryHost: "localhost:44346", editHost: "localhost:44347");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error encountered during initial configuration: {ex.Message}");
            }
        }

        private static void SetUpSite(
            ISiteDefinitionRepository siteRepository,
            IContentRepository contentRepository,
            int siteNumber,
            string primaryHost,
            string editHost)
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
            newSiteSettings.SiteName = $"Stott Security Dev Site {siteNumber}";

            var siteSettingsReference = contentRepository.Save(newSiteSettings, SaveAction.Publish, AccessLevel.NoAccess);

            // Update Home Page with Site Settings reference
            var existingHomePage = contentRepository.Get<HomePage>(homePageReference, culture);
            var editableHomePage = existingHomePage.CreateWritableClone() as HomePage;
            editableHomePage.SiteSettings = siteSettingsReference.ToReferenceWithoutVersion();

            homePageReference = contentRepository.Save(editableHomePage, SaveAction.Publish, AccessLevel.NoAccess);

            // Create Site with two hosts: a primary (front-end) and an edit (CMS) host
            var newSite = SiteDefinition.Empty.CreateWritableClone();
            newSite.Name = $"Test Website {siteNumber}";
            newSite.StartPage = homePageReference.ToReferenceWithoutVersion();
            newSite.SiteUrl = new Uri($"https://{primaryHost}/");
            newSite.Hosts.Add(new HostDefinition
            {
                Name = primaryHost,
                Type = HostDefinitionType.Primary,
                UseSecureConnection = true
            });
            newSite.Hosts.Add(new HostDefinition
            {
                Name = editHost,
                Type = HostDefinitionType.Edit,
                UseSecureConnection = true
            });

            siteRepository.Save(newSite);
        }
    }
}
