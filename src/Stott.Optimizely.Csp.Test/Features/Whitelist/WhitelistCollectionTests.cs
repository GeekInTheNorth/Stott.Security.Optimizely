using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Features.Whitelist;

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

        private WhitelistEntry CreateWhiteListEntry(string sourceUrl, string directive)
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
