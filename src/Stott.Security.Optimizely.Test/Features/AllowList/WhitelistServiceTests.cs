namespace Stott.Security.Optimizely.Test.Features.AllowList;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.AllowList;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Permissions.Service;
using Stott.Security.Optimizely.Features.Settings.Service;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public class AllowListServiceTests
{
    private Mock<ICspSettingsService> _mockSettingsService;

    private Mock<IAllowListRepository> _mockRepository;

    private Mock<ICspPermissionService> _mockCspPermissionService;

    private Mock<ILogger<IAllowListService>> _mockLogger;

    private Mock<ICacheWrapper> _mockCacheWrapper;

    private AllowListService _allowListService;

    [SetUp]
    public void SetUp()
    {
        _mockSettingsService = new Mock<ICspSettingsService>();
        _mockRepository = new Mock<IAllowListRepository>();
        _mockCspPermissionService = new Mock<ICspPermissionService>();
        _mockLogger = new Mock<ILogger<IAllowListService>>();
        _mockCacheWrapper = new Mock<ICacheWrapper>();

        _allowListService = new AllowListService(
            _mockSettingsService.Object,
            _mockRepository.Object,
            _mockCspPermissionService.Object,
            _mockCacheWrapper.Object,
            _mockLogger.Object);
    }

    [Test]
    public void Constructor_GivenANullCspSettingsRepositoryThenAnArgumentNullExceptionIsThrown()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => 
        { 
            new AllowListService(
                null, 
                _mockRepository.Object, 
                _mockCspPermissionService.Object, 
                _mockCacheWrapper.Object,
                _mockLogger.Object);
        });
    }

    [Test]
    public void Constructor_GivenANullAllowListRepositoryThenAnArgumentNullExceptionIsThrown()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => 
        { 
            new AllowListService(
                _mockSettingsService.Object, 
                null, 
                _mockCspPermissionService.Object, 
                _mockCacheWrapper.Object,
                _mockLogger.Object);
        });
    }

    [Test]
    public void Constructor_GivenANullCspPermissionServiceThenAnArgumentNullExceptionIsThrown()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => 
        { 
            new AllowListService(
                _mockSettingsService.Object, 
                _mockRepository.Object, 
                null, 
                _mockCacheWrapper.Object,
                _mockLogger.Object);
        });
    }

    [Test]
    public void Constructor_GivenANullCacheWrapperThenAnArgumentNullExceptionIsThrown()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() =>
        {
            new AllowListService(
                _mockSettingsService.Object,
                _mockRepository.Object,
                _mockCspPermissionService.Object,
                null,
                _mockLogger.Object);
        });
    }

    [Test]
    public void Constructor_GivenAValidAllowListOptionsThenAnExceptionIsNotThrown()
    {
        // Assert
        Assert.DoesNotThrow(() => 
        { 
            new AllowListService(
                _mockSettingsService.Object, 
                _mockRepository.Object, 
                _mockCspPermissionService.Object, 
                _mockCacheWrapper.Object,
                _mockLogger.Object); 
        });
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.ValidAllowListTests))]
    public async Task AddFromAllowListToCsp_DoesNotProcessTheAllowListWhenAllowListOptionsIsDisabled(string violationSource, string violationDirective)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = false });

        // Act
        await _allowListService.AddFromAllowListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetAllowListAsync(It.IsAny<string>()), Times.Never);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.InvalidAllowListTests))]
    public async Task AddFromAllowListToCsp_DoesNotProcessTheAllowListWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string violationDirective)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = true });

        // Act
        await _allowListService.AddFromAllowListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetAllowListAsync(It.IsAny<string>()), Times.Never);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.ValidAllowListTests))]
    public async Task AddFromAllowListToCsp_WhenGivenAValidSourceAndDomainAndAllowListIsEnabled_ThenTheAllowListIsRetrieved(string violationSource, string violationDirective)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = true });
        
        _mockRepository.Setup(x => x.GetAllowListAsync(It.IsAny<string>()))
                       .ReturnsAsync(new AllowListCollection(new List<AllowListEntry>(0)));

        // Act
        await _allowListService.AddFromAllowListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetAllowListAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.ValidAllowListTests))]
    public async Task AddFromAllowListToCsp_WhenGivenAValidSourceAndDomainAndAnEmptyAllowList_ThenTheCspIsNotUpdated(string violationSource, string violationDirective)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = true });

        _mockRepository.Setup(x => x.GetAllowListAsync(It.IsAny<string>()))
                       .ReturnsAsync(new AllowListCollection(new List<AllowListEntry>(0)));

        // Act
        await _allowListService.AddFromAllowListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetAllowListAsync(It.IsAny<string>()), Times.Once);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.ValidAllowListTests))]
    public async Task AddFromAllowListToCsp_WhenGivenAValidSourceAndDomainThatAreNotOnTheAllowList_ThenTheCspIsNotUpdated(string violationSource, string violationDirective)
    {
        // Arrange
        var allowListEntries = new List<AllowListEntry>
        {
            new AllowListEntry
            {
                SourceUrl = "https://www.a-different-domain.com",
                Directives = CspConstants.AllDirectives
            }
        };
        var allowListCollection = new AllowListCollection(allowListEntries);

        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = true });

        _mockRepository.Setup(x => x.GetAllowListAsync(It.IsAny<string>()))
                       .ReturnsAsync(allowListCollection);

        // Act
        await _allowListService.AddFromAllowListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetAllowListAsync(It.IsAny<string>()), Times.Once);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.ValidAllowListTests))]
    public async Task AddFromAllowListToCsp_WhenGivenAValidSourceAndDomainThatAreOnTheAllowList_ThenTheCspIsUpdated(string violationSource, string violationDirective)
    {
        // Arrange
        var allowListEntries = new List<AllowListEntry>
        {
            new AllowListEntry
            {
                SourceUrl = "https://www.a-different-domain.com",
                Directives = CspConstants.AllDirectives
            },
            new AllowListEntry
            {
                SourceUrl = violationSource,
                Directives = new List<string> { violationDirective }
            }
        };
        var allowListCollection = new AllowListCollection(allowListEntries);

        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = true });

        _mockRepository.Setup(x => x.GetAllowListAsync(It.IsAny<string>()))
                       .ReturnsAsync(allowListCollection);

        // Act
        await _allowListService.AddFromAllowListToCspAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetAllowListAsync(It.IsAny<string>()), Times.Once);
        _mockCspPermissionService.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.ValidAllowListTests))]
    public async Task IsOnAllowList_ReturnsFalseWhenAllowListOptionsIsDisabled(string violationSource, string violationDirective)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = false });

        // Act
        var isOnAllowList = await _allowListService.IsOnAllowListAsync(violationSource, violationDirective);

        // Assert
        Assert.That(isOnAllowList, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.InvalidAllowListTests))]
    public async Task IsOnAllowList_ReturnsFalseWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string violationDirective)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = true });

        // Act
        var isOnAllowList = await _allowListService.IsOnAllowListAsync(violationSource, violationDirective);

        // Assert
        Assert.That(isOnAllowList, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.ValidAllowListTests))]
    public async Task IsOnAllowList_DoesNotAttemptToGetAAllowListWhenAllowListOptionsIsDisabled(string violationSource, string violationDirective)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = false });

        // Act
        var isOnAllowList = await _allowListService.IsOnAllowListAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetAllowListAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.InvalidAllowListTests))]
    public async Task IsOnAllowList_DoesNotAttemptToGetAAllowListWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string violationDirective)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = true });

        // Act
        var isOnAllowList = await _allowListService.IsOnAllowListAsync(violationSource, violationDirective);

        // Assert
        _mockRepository.Verify(x => x.GetAllowListAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.ValidAllowListTests))]
    public async Task IsOnAllowList_WhenGivenAValidSourceAndDomainAndAllowListIsEnabled_ThenTheAllowListIsRetrieved(string violationSource, string directive)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = true });

        // Act
        var isOnAllowList = await _allowListService.IsOnAllowListAsync(violationSource, directive);

        // Assert
        _mockRepository.Verify(x => x.GetAllowListAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.AllowListTests))]
    public async Task IsOnAllowList_WhenDomainMatchesAAllowListEntry_ThenReturnsTrue(
        string violatedSource,
        string violatedDirective,
        string allowedDomain,
        string allowedDirective,
        bool expectedResult)
    {
        // Arrange
        var allowListEntries = new List<AllowListEntry>
        {
            CreateAllowListEntry("https://www.example.com", CspConstants.Directives.DefaultSource),
            CreateAllowListEntry(allowedDomain, allowedDirective)
        };
        var allowListCollection = new AllowListCollection(allowListEntries);

        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsAllowListEnabled = true });

        _mockRepository.Setup(x => x.GetAllowListAsync(It.IsAny<string>()))
                       .Returns(Task.FromResult(allowListCollection));

        // Act
        var isOnAllowList = await _allowListService.IsOnAllowListAsync(violatedSource, violatedDirective);

        // Assert
        Assert.That(isOnAllowList, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task IsAllowListValidAsync_GivenAValidAllowListCollectionIsReturned_ThenReturnsTrue()
    {
        // Arrange
        var allowListEntries = new List<AllowListEntry>
        {
            CreateAllowListEntry("https://www.example.com", CspConstants.Directives.DefaultSource),
            CreateAllowListEntry("ws://www.example.com", CspConstants.Directives.DefaultSource)
        };
        var allowListCollection = new AllowListCollection(allowListEntries);

        _mockRepository.Setup(x => x.GetAllowListAsync(It.IsAny<string>()))
                       .Returns(Task.FromResult(allowListCollection));

        // Act
        var isValid = await _allowListService.IsAllowListValidAsync("https://www.example.com/AllowList.json");

        // Assert
        Assert.That(isValid, Is.True);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task IsAllowListValidAsync_ReturnsFalseGivenANullEmptyOrWhitespaceUrl(string AllowListUrl)
    {
        // Act
        var isValid = await _allowListService.IsAllowListValidAsync(AllowListUrl);

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    public async Task IsAllowListValidAsync_GivenAAllowListCollectionWithNoItemsIsReturned_ThenReturnsFalse()
    {
        // Arrange
        var allowListCollection = new AllowListCollection(new List<AllowListEntry>(0));

        _mockRepository.Setup(x => x.GetAllowListAsync(It.IsAny<string>()))
                       .Returns(Task.FromResult(allowListCollection));

        // Act
        var isValid = await _allowListService.IsAllowListValidAsync("https://www.example.com/AllowList.json");

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task IsAllowListValidAsync_GivenAAllowListCollectionWithAnEntryWithoutAUrl_ThenReturnsFalse(string emptyUrl)
    {
        // Arrange
        var allowListEntries = new List<AllowListEntry>
        {
            CreateAllowListEntry("https://www.example.com", CspConstants.Directives.DefaultSource),
            CreateAllowListEntry(emptyUrl, CspConstants.Directives.DefaultSource)
        };
        var allowListCollection = new AllowListCollection(allowListEntries);

        _mockRepository.Setup(x => x.GetAllowListAsync(It.IsAny<string>()))
                       .Returns(Task.FromResult(allowListCollection));

        // Act
        var isValid = await _allowListService.IsAllowListValidAsync("https://www.example.com/AllowList.json");

        // Assert
        Assert.That(isValid, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task IsAllowListValidAsync_GivenAAllowListCollectionWithAnEntryWithAnEmptyPermission_ThenReturnsFalse(string emptyPermission)
    {
        // Arrange
        var allowListEntries = new List<AllowListEntry>
        {
            CreateAllowListEntry("https://www.example.com", CspConstants.Directives.DefaultSource),
            CreateAllowListEntry("ws://www.example.com", emptyPermission)
        };
        var allowListCollection = new AllowListCollection(allowListEntries);

        _mockRepository.Setup(x => x.GetAllowListAsync(It.IsAny<string>()))
                       .Returns(Task.FromResult(allowListCollection));

        // Act
        var isValid = await _allowListService.IsAllowListValidAsync("https://www.example.com/AllowList.json");

        // Assert
        Assert.That(isValid, Is.False);
    }

    private static AllowListEntry CreateAllowListEntry(string sourceUrl, string directive)
    {
        return new AllowListEntry
        {
            SourceUrl = sourceUrl,
            Directives = new List<string>
            {
                directive
            }
        };
    }
}