using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Features.Whitelist;

namespace Stott.Optimizely.Csp.Test.Features.Whitelist
{
    [TestFixture]
    public class WhitelistServiceTests
    {
        private Mock<ICspWhitelistOptions> _mockOptions;

        private Mock<IWhitelistRepository> _mockRepository;

        private WhitelistService _whitelistService;

        [SetUp]
        public void SetUp()
        {
            _mockOptions = new Mock<ICspWhitelistOptions>();
            _mockRepository = new Mock<IWhitelistRepository>();

            _whitelistService = new WhitelistService(_mockOptions.Object, _mockRepository.Object);
        }

        [Test]
        public void Constructor_GivenANullWhiteListOptionsThenAnArgumentNullExceptionIsThrown()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => { new WhitelistService(null, _mockRepository.Object); });
        }

        [Test]
        public void Constructor_GivenANullWhitelistRepositoryThenAnArgumentNullExceptionIsThrown()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => { new WhitelistService(_mockOptions.Object, null); });
        }

        [Test]
        public void Constructor_GivenAValidWhiteListOptionsThenAnExceptionIsNotThrown()
        {
            // Assert
            Assert.DoesNotThrow(() => { new WhitelistService(_mockOptions.Object, _mockRepository.Object); });
        }

        [Test]
        [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
        public async Task IsOnWhitelist_ReturnsFalseWhenWhiteListOptionsIsDisabled(string violationSource, string directive)
        {
            // Arrange
            _mockOptions.Setup(x => x.UseWhitelist).Returns(false);

            // Act
            var isOnWhiteList = await _whitelistService.IsOnWhitelist(violationSource, directive);

            // Assert
            Assert.That(isOnWhiteList, Is.False);
        }

        [Test]
        [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.InvalidWhitelistTests))]
        public async Task IsOnWhitelist_ReturnsFalseWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string directive)
        {
            // Arrange
            _mockOptions.Setup(x => x.UseWhitelist).Returns(true);

            // Act
            var isOnWhiteList = await _whitelistService.IsOnWhitelist(violationSource, directive);

            // Assert
            Assert.That(isOnWhiteList, Is.False);
        }

        [Test]
        [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
        public async Task IsOnWhitelist_DoesNotAttemptToGetAWhitelistWhenWhiteListOptionsIsDisabled(string violationSource, string directive)
        {
            // Arrange
            _mockOptions.Setup(x => x.UseWhitelist).Returns(false);

            // Act
            var isOnWhiteList = await _whitelistService.IsOnWhitelist(violationSource, directive);

            // Assert
            _mockRepository.Verify(x => x.GetWhitelist(It.IsAny<string>()), Times.Never);
        }

        [Test]
        [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.InvalidWhitelistTests))]
        public async Task IsOnWhitelist_DoesNotAttemptToGetAWhitelistWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string directive)
        {
            // Arrange
            _mockOptions.Setup(x => x.UseWhitelist).Returns(true);

            // Act
            var isOnWhiteList = await _whitelistService.IsOnWhitelist(violationSource, directive);

            // Assert
            _mockRepository.Verify(x => x.GetWhitelist(It.IsAny<string>()), Times.Never);
        }

        [Test]
        [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
        public async Task IsOnWhiteList_WhenGivenAValidSourceAndDomainAndWhitelistIsEnabled_ThenTheWhiteListIsRetrieved(string violationSource, string directive)
        {
            // Arrange
            _mockOptions.Setup(x => x.UseWhitelist).Returns(true);

            // Act
            var isOnWhiteList = await _whitelistService.IsOnWhitelist(violationSource, directive);

            // Assert
            _mockRepository.Verify(x => x.GetWhitelist(It.IsAny<string>()), Times.Once);
        }

        [Test]
        [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.WhitelistTests))]
        public async Task IsOnWhiteList_WhenDomainMatchesAWhiteListEntry_ThenReturnsTrue(
            string violatedSource,
            string violatedDirective,
            string allowedDomain,
            string allowedDirective,
            bool expectedResult)
        {
            // Arrange
            var whitelistEntries = new List<WhitelistEntry>
            {
                CreateWhiteListEntry("https://www.example.com", CspConstants.Directives.DefaultSource),
                CreateWhiteListEntry(allowedDomain, allowedDirective)
            };
            var whitelistCollection = new WhitelistCollection(whitelistEntries);

            _mockOptions.Setup(x => x.UseWhitelist).Returns(true);
            _mockRepository.Setup(x => x.GetWhitelist(It.IsAny<string>()))
                           .Returns(Task.FromResult(whitelistCollection));

            // Act
            var isOnWhiteList = await _whitelistService.IsOnWhitelist(violatedSource, violatedDirective);

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
