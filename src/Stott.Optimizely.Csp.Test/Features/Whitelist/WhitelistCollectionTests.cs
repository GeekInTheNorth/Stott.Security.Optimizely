using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Whitelist;

namespace Stott.Optimizely.Csp.Test.Features.Whitelist
{
    [TestFixture]
    public class WhitelistCollectionTests
    {
        [Test]
        [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.InvalidWhitelistTests))]
        public void IsOnWhitelist_ReturnsFalseWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string directive)
        {
            // Arrange
            var whiteListEntries = new List<WhitelistEntry>
            {
                CreateWhiteListEntry("https://www.example.com", CspConstants.Directives.DefaultSource),
                CreateWhiteListEntry("https://www.exampleTwo.com", CspConstants.Directives.DefaultSource)
            };

            var whitelistCollection = new WhitelistCollection(whiteListEntries);

            // Act
            var isOnWhiteList = whitelistCollection.IsOnWhitelist(violationSource, directive);

            // Assert
            Assert.That(isOnWhiteList, Is.False);
        }

        [Test]
        [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.WhitelistTests))]
        public void IsOnWhiteList_WhenDomainMatchesAWhiteListEntry_ThenReturnsTrue(
            string violatedSource,
            string violatedDirective,
            string allowedDomain,
            string allowedDirective,
            bool expectedResult)
        {
            // Arrange
            var whiteListEntries = new List<WhitelistEntry>
            {
                CreateWhiteListEntry("https://www.example.com", CspConstants.Directives.DefaultSource),
                CreateWhiteListEntry(allowedDomain, allowedDirective)
            };

            var whitelistCollection = new WhitelistCollection(whiteListEntries);

            // Act
            var isOnWhiteList = whitelistCollection.IsOnWhitelist(violatedSource, violatedDirective);

            // Assert
            Assert.That(isOnWhiteList, Is.EqualTo(expectedResult));
        }

        private static WhitelistEntry CreateWhiteListEntry(string sourceUrl, string directive)
        {
            return new WhitelistEntry
            {
                SourceUrl = sourceUrl,
                Directives = new List<string>
                {
                    directive
                }
            };
        }
    }
}
