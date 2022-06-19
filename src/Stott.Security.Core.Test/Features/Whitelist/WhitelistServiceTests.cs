namespace Stott.Security.Core.Test.Features.Whitelist;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Permissions.Service;
using Stott.Security.Core.Features.Whitelist;

[TestFixture]
public class WhitelistServiceTests
{
    private Mock<ICspWhitelistOptions> _mockOptions;

    private Mock<IWhitelistRepository> _mockRepository;

    private Mock<ICspPermissionService> _mockCspPermissionService;

    private Mock<ILoggingProviderFactory> _mockLoggingProviderFactory;

    private Mock<ILoggingProvider> _mockLoggingProvider;

    private WhitelistService _whitelistService;

    [SetUp]
    public void SetUp()
    {
        _mockOptions = new Mock<ICspWhitelistOptions>();
        _mockRepository = new Mock<IWhitelistRepository>();
        _mockCspPermissionService = new Mock<ICspPermissionService>();

        _mockLoggingProvider = new Mock<ILoggingProvider>();
        _mockLoggingProviderFactory = new Mock<ILoggingProviderFactory>();
        _mockLoggingProviderFactory.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(_mockLoggingProvider.Object);

        _whitelistService = new WhitelistService(
            _mockOptions.Object,
            _mockRepository.Object,
            _mockCspPermissionService.Object,
            _mockLoggingProviderFactory.Object);
    }

    [Test]
    public void Constructor_GivenANullWhiteListOptionsThenAnArgumentNullExceptionIsThrown()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => { new WhitelistService(null, _mockRepository.Object, _mockCspPermissionService.Object, _mockLoggingProviderFactory.Object); });
    }

    [Test]
    public void Constructor_GivenANullWhitelistRepositoryThenAnArgumentNullExceptionIsThrown()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => { new WhitelistService(_mockOptions.Object, null, _mockCspPermissionService.Object, _mockLoggingProviderFactory.Object); });
    }

    [Test]
    public void Constructor_GivenANullCspPermissionServiceThenAnArgumentNullExceptionIsThrown()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => { new WhitelistService(_mockOptions.Object, _mockRepository.Object, null, _mockLoggingProviderFactory.Object); });
    }

    [Test]
    public void Constructor_GivenAValidWhiteListOptionsThenAnExceptionIsNotThrown()
    {
        // Assert
        Assert.DoesNotThrow(() => { new WhitelistService(_mockOptions.Object, _mockRepository.Object, _mockCspPermissionService.Object, _mockLoggingProviderFactory.Object); });
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
    public async Task AddFromWhiteListToCsp_DoesNotProcessTheWhiteListWhenWhiteListOptionsIsDisabled(string violationSource, string violationDirective)
    {
        // Arrange
        _mockOptions.Setup(x => x.UseWhitelist).Returns(false);

        // Act
        await _whitelistService.AddFromWhiteListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetWhitelistAsync(It.IsAny<string>()), Times.Never);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.InvalidWhitelistTests))]
    public async Task AddFromWhiteListToCsp_DoesNotProcessTheWhiteListWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string violationDirective)
    {
        // Arrange
        _mockOptions.Setup(x => x.UseWhitelist).Returns(true);

        // Act
        await _whitelistService.AddFromWhiteListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetWhitelistAsync(It.IsAny<string>()), Times.Never);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
    public async Task AddFromWhiteListToCsp_WhenGivenAValidSourceAndDomainAndWhitelistIsEnabled_ThenTheWhiteListIsRetrieved(string violationSource, string violationDirective)
    {
        // Arrange
        _mockOptions.Setup(x => x.UseWhitelist).Returns(true);
        _mockRepository.Setup(x => x.GetWhitelistAsync(It.IsAny<string>()))
                       .ReturnsAsync(new WhitelistCollection(new List<WhitelistEntry>(0)));

        // Act
        await _whitelistService.AddFromWhiteListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetWhitelistAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
    public async Task AddFromWhiteListToCsp_WhenGivenAValidSourceAndDomainAndAnEmptyWhitelist_ThenTheCspIsNotUpdated(string violationSource, string violationDirective)
    {
        // Arrange
        _mockOptions.Setup(x => x.UseWhitelist).Returns(true);
        _mockRepository.Setup(x => x.GetWhitelistAsync(It.IsAny<string>()))
                       .ReturnsAsync(new WhitelistCollection(new List<WhitelistEntry>(0)));

        // Act
        await _whitelistService.AddFromWhiteListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetWhitelistAsync(It.IsAny<string>()), Times.Once);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
    public async Task AddFromWhiteListToCsp_WhenGivenAValidSourceAndDomainThatAreNotOnTheWhitelist_ThenTheCspIsNotUpdated(string violationSource, string violationDirective)
    {
        // Arrange
        var whitelistEntries = new List<WhitelistEntry>
        {
            new WhitelistEntry
            {
                SourceUrl = "https://www.a-different-domain.com",
                Directives = CspConstants.AllDirectives
            }
        };
        var whitelistCollection = new WhitelistCollection(whitelistEntries);

        _mockOptions.Setup(x => x.UseWhitelist).Returns(true);
        _mockRepository.Setup(x => x.GetWhitelistAsync(It.IsAny<string>()))
                       .ReturnsAsync(whitelistCollection);

        // Act
        await _whitelistService.AddFromWhiteListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetWhitelistAsync(It.IsAny<string>()), Times.Once);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
    public async Task AddFromWhiteListToCsp_WhenGivenAValidSourceAndDomainThatAreOnTheWhitelist_ThenTheCspIsUpdated(string violationSource, string violationDirective)
    {
        // Arrange
        var whitelistEntries = new List<WhitelistEntry>
        {
            new WhitelistEntry
            {
                SourceUrl = "https://www.a-different-domain.com",
                Directives = CspConstants.AllDirectives
            },
            new WhitelistEntry
            {
                SourceUrl = violationSource,
                Directives = new List<string> { violationDirective }
            }
        };
        var whitelistCollection = new WhitelistCollection(whitelistEntries);

        _mockOptions.Setup(x => x.UseWhitelist).Returns(true);
        _mockRepository.Setup(x => x.GetWhitelistAsync(It.IsAny<string>()))
                       .ReturnsAsync(whitelistCollection);

        // Act
        await _whitelistService.AddFromWhiteListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetWhitelistAsync(It.IsAny<string>()), Times.Once);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
    public async Task IsOnWhitelist_ReturnsFalseWhenWhiteListOptionsIsDisabled(string violationSource, string violationDirective)
    {
        // Arrange
        _mockOptions.Setup(x => x.UseWhitelist).Returns(false);

        // Act
        var isOnWhiteList = await _whitelistService.IsOnWhitelistAsync(violationSource, violationDirective);

        // Assert
        Assert.That(isOnWhiteList, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.InvalidWhitelistTests))]
    public async Task IsOnWhitelist_ReturnsFalseWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string violationDirective)
    {
        // Arrange
        _mockOptions.Setup(x => x.UseWhitelist).Returns(true);

        // Act
        var isOnWhiteList = await _whitelistService.IsOnWhitelistAsync(violationSource, violationDirective);

        // Assert
        Assert.That(isOnWhiteList, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
    public async Task IsOnWhitelist_DoesNotAttemptToGetAWhitelistWhenWhiteListOptionsIsDisabled(string violationSource, string violationDirective)
    {
        // Arrange
        _mockOptions.Setup(x => x.UseWhitelist).Returns(false);

        // Act
        var isOnWhiteList = await _whitelistService.IsOnWhitelistAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetWhitelistAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.InvalidWhitelistTests))]
    public async Task IsOnWhitelist_DoesNotAttemptToGetAWhitelistWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string violationDirective)
    {
        // Arrange
        _mockOptions.Setup(x => x.UseWhitelist).Returns(true);

        // Act
        var isOnWhiteList = await _whitelistService.IsOnWhitelistAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetWhitelistAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(WhitelistServiceTestCases), nameof(WhitelistServiceTestCases.ValidWhitelistTests))]
    public async Task IsOnWhiteList_WhenGivenAValidSourceAndDomainAndWhitelistIsEnabled_ThenTheWhiteListIsRetrieved(string violationSource, string directive)
    {
        // Arrange
        _mockOptions.Setup(x => x.UseWhitelist).Returns(true);

        // Act
        var isOnWhiteList = await _whitelistService.IsOnWhitelistAsync(violationSource, directive);

        // Assert
        _mockRepository.Verify(x => x.GetWhitelistAsync(It.IsAny<string>()), Times.Once);
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
        _mockRepository.Setup(x => x.GetWhitelistAsync(It.IsAny<string>()))
                       .Returns(Task.FromResult(whitelistCollection));

        // Act
        var isOnWhiteList = await _whitelistService.IsOnWhitelistAsync(violatedSource, violatedDirective);

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
