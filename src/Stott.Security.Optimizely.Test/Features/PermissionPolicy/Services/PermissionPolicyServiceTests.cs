using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;
using Stott.Security.Optimizely.Features.PermissionPolicy.Service;
using Stott.Security.Optimizely.Test.TestCases;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy.Services;

public sealed class PermissionPolicyServiceTests
{
    private Mock<ICacheWrapper> _mockCache;

    private Mock<IPermissionPolicyRepository> _mockRepository;

    private PermissionPolicyService _service;

    [SetUp]
    public void SetUp()
    {
        _mockCache = new Mock<ICacheWrapper>();
        _mockRepository = new Mock<IPermissionPolicyRepository>();
        _service = new PermissionPolicyService(_mockCache.Object, _mockRepository.Object);
    }

    [Test]
    public async Task GetPermissionPolicySettingsAsync_WhenSettingsAreInTheCache_ThenDataIsNotRetrievedFromTheRepository()
    {
        // Arrange
        var settings = new PermissionPolicySettingsModel();
        _mockCache.Setup(x => x.Get<PermissionPolicySettingsModel>(It.IsAny<string>())).Returns(settings);

        // Act
        var result = await _service.GetPermissionPolicySettingsAsync(null, null);

        // Assert
        Assert.That(result, Is.EqualTo(settings));
        _mockRepository.Verify(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GetPermissionPolicySettingsAsync_WhenSettingsAreNotInTheCache_ThenDataIsRetrievedFromTheRepository()
    {
        // Arrange
        var settings = new PermissionPolicySettingsModel();
        _mockRepository.Setup(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(settings);

        // Act
        var result = await _service.GetPermissionPolicySettingsAsync(null, null);

        // Assert
        Assert.That(result, Is.EqualTo(settings));
        _mockRepository.Verify(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), settings), Times.Once);
    }

    [Test]
    public async Task ListDirectivesAsync_WhenDirectivesAreInTheCache_ThenDataIsNotRetrievedFromTheRepository()
    {
        // Arrange
        var directives = new List<PermissionPolicyDirectiveModel>();
        _mockCache.Setup(x => x.Get<List<PermissionPolicyDirectiveModel>>(It.IsAny<string>())).Returns(directives);

        // Act
        var result = await _service.ListDirectivesAsync(null, null, null, PermissionPolicyEnabledFilter.All);

        // Assert
        Assert.That(result, Is.EqualTo(directives));
        _mockRepository.Verify(x => x.ListDirectivesAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ListDirectivesAsync_WhenDirectivesAreNotInTheCache_ThenDataIsRetrievedFromTheRepository()
    {
        // Arrange
        var directives = new List<PermissionPolicyDirectiveModel>();
        _mockRepository.Setup(x => x.ListDirectivesAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(directives);

        // Act
        var result = await _service.ListDirectivesAsync(null, null, null, PermissionPolicyEnabledFilter.All);

        // Assert
        Assert.That(result, Is.EqualTo(directives));
        _mockRepository.Verify(x => x.ListDirectivesAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), directives), Times.Once);
    }

    [Test]
    public async Task ListDirectivesAsync_WhenTheRepositoryReturnsNoDirectives_ThenDefaultDirectivesAreAdded()
    {
        // Arrange
        var directives = new List<PermissionPolicyDirectiveModel>();
        _mockRepository.Setup(x => x.ListDirectivesAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(directives);

        // Act
        var result = await _service.ListDirectivesAsync(null, null, null, PermissionPolicyEnabledFilter.All);

        // Assert
        Assert.That(result, Has.Count.EqualTo(PermissionPolicyConstants.AllDirectives.Count));
        foreach (var directive in PermissionPolicyConstants.AllDirectives)
        {
            Assert.That(result, Has.Some.Matches<PermissionPolicyDirectiveModel>(x => x.Name == directive));
        }
    }

    [Test]
    public async Task ListDirectivesAsync_WhenTheRepositoryReturnsSomeDirectives_ThenDefaultDirectivesAreAdded()
    {
        // Arrange
        var random = new Random();
        var policyOneIndex = random.Next(0, PermissionPolicyConstants.AllDirectives.Count - 1);
        var policyTwoIndex = random.Next(0, PermissionPolicyConstants.AllDirectives.Count - 1);
        while (policyOneIndex == policyTwoIndex)
        {
            policyTwoIndex = random.Next(0, PermissionPolicyConstants.AllDirectives.Count - 1);
        }

        var directives = new List<PermissionPolicyDirectiveModel>
        {
            new() {
                Name = PermissionPolicyConstants.AllDirectives[policyOneIndex],
                EnabledState = PermissionPolicyEnabledState.All,
                Sources = []
            },
            new() {
                Name = PermissionPolicyConstants.AllDirectives[policyTwoIndex],
                EnabledState = PermissionPolicyEnabledState.Disabled,
                Sources = []
            }
        };
        _mockRepository.Setup(x => x.ListDirectivesAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(directives);

        // Act
        var result = await _service.ListDirectivesAsync(null, null, null, PermissionPolicyEnabledFilter.All);

        // Assert
        Assert.That(result, Has.Count.EqualTo(PermissionPolicyConstants.AllDirectives.Count));
        foreach (var directive in PermissionPolicyConstants.AllDirectives)
        {
            Assert.That(result, Has.Some.Matches<PermissionPolicyDirectiveModel>(x => x.Name == directive));
        }
    }

    [Test]
    [TestCaseSource(typeof(PermissionPolicyServiceTestCases), nameof(PermissionPolicyServiceTestCases.FilterDirectiveTests))]
    public async Task ListDirectivesAsync_FiltersDirectivesAppropriately(string sourceFilter, PermissionPolicyEnabledFilter enabledFilter, string expectedDirectives)
    {
        // Arrange
        var directives = new List<PermissionPolicyDirectiveModel>
        {
            new() { Name = PermissionPolicyConstants.Accelerometer, EnabledState = PermissionPolicyEnabledState.Disabled, Sources = [] },
            new() { Name = PermissionPolicyConstants.AmbientLightSensor, EnabledState = PermissionPolicyEnabledState.None, Sources = [] },
            new() { Name = PermissionPolicyConstants.AttributionReporting, EnabledState = PermissionPolicyEnabledState.All, Sources = [] },
            new() { Name = PermissionPolicyConstants.Autoplay, EnabledState = PermissionPolicyEnabledState.ThisSite, Sources = [] },
            new() { Name = PermissionPolicyConstants.Bluetooth, EnabledState = PermissionPolicyEnabledState.SpecificSites, Sources = [new PermissionPolicyUrl { Url = "https://www.exampleone.com" }] },
            new() { Name = PermissionPolicyConstants.BrowsingTopics, EnabledState = PermissionPolicyEnabledState.ThisAndSpecificSites, Sources = [new PermissionPolicyUrl { Url = "https://www.exampletwo.com" }] },
        };
        _mockRepository.Setup(x => x.ListDirectivesAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(directives);

        // Act
        var result = await _service.ListDirectivesAsync(null, null, sourceFilter, enabledFilter);
        var filteredDirectives = string.Join(",", result.Select(x => x.Name));

        // Assert
        Assert.That(filteredDirectives, Is.EqualTo(expectedDirectives));
    }

    [Test]
    public void SaveDirectiveAsync_WhenModelIsNull_ThenAnExceptionIsThrown()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.SaveDirectiveAsync(null, "Test", null, null));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveDirectiveAsync_WhenModifiedByIsNullOrWhitespace_ThenAnExceptionIsThrown(string modifiedBy)
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.SaveDirectiveAsync(new SavePermissionPolicyModel(), modifiedBy, null, null));
    }

    [Test]
    public async Task SaveDirectiveAsync_WhenModelAndModifiedByIsValid_ThenDataIsSavedToTheRepository()
    {
        // Arrange
        var model = new SavePermissionPolicyModel
        {
            Name = "Test",
            EnabledState = PermissionPolicyEnabledState.All,
            Sources = []
        };

        // Act
        await _service.SaveDirectiveAsync(model, "Test", null, null);

        // Assert
        _mockRepository.Verify(x => x.SaveDirectiveAsync(model, "Test", null, null), Times.Once);
    }

    [Test]
    public void SaveSettingsAsync_WhenModelIsNull_ThenAnExceptionIsThrown()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.SaveSettingsAsync(null, "Test", null, null));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveSettingsAsync_WhenModifiedByIsNullOrWhitespace_ThenAnExceptionIsThrown(string modifiedBy)
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.SaveSettingsAsync(new PermissionPolicySettingsModel(), modifiedBy, null, null));
    }

    [Test]
    public async Task SaveSettingsAsync_WhenModelAndModifiedByIsValid_ThenDataIsSavedToTheRepository()
    {
        // Arrange
        var model = new PermissionPolicySettingsModel
        {
            IsEnabled = true
        };

        // Act
        await _service.SaveSettingsAsync(model, "Test", null, null);

        // Assert
        _mockRepository.Verify(x => x.SaveSettingsAsync(model, "Test", null, null), Times.Once);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCacheHasCompiledHeaders_ThenHeadersAreReturnedFromCacheAndNotTheRepository()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<CompiledPermissionPolicy>(It.IsAny<string>())).Returns(new CompiledPermissionPolicy { IsEnabled = true, Directives = ["Test"] });

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        _mockCache.Verify(x => x.Get<CompiledPermissionPolicy>(It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCacheIsEmptyAndSettingsAreDisabled_ThenOnlyTheSettingsAreRetrievedAndCached()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = false });

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        _mockCache.Verify(x => x.Get<CompiledPermissionPolicy>(It.IsAny<string>()), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<CompiledPermissionPolicy>()), Times.Once);
        _mockRepository.Verify(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(x => x.ListDirectiveFragments(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCacheIsEmptyAndSettingsAreEnabled_ThenSettingsAndDirectivesAreRetrievedAndCached()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = true });
        _mockRepository.Setup(x => x.ListDirectiveFragments(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(["Test"]);

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        _mockCache.Verify(x => x.Get<CompiledPermissionPolicy>(It.IsAny<string>()), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<CompiledPermissionPolicy>()), Times.Once);
        _mockRepository.Verify(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(x => x.ListDirectiveFragments(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCachedDataIsDisabled_ThenNoHeadersAreReturned()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<CompiledPermissionPolicy>(It.IsAny<string>())).Returns(new CompiledPermissionPolicy { IsEnabled = false });

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCachedDataIsEnabledButThereAreNoDirectives_ThenNoHeadersAreReturned()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<CompiledPermissionPolicy>(It.IsAny<string>())).Returns(new CompiledPermissionPolicy { IsEnabled = true, Directives = []});

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCachedDataIsEnabled_ThenHeadersAreReturned()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<CompiledPermissionPolicy>(It.IsAny<string>())).Returns(new CompiledPermissionPolicy { IsEnabled = true, Directives = ["Test"] });

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result, Has.Some.Matches<HeaderDto>(x => x.Key == PermissionPolicyConstants.PermissionPolicyHeader && x.Value == "Test"));
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCachedDataIsEnabledAndTwoDirectivesExist_ThenHeadersAreReturned()
    {
        // Arrange
        var cachedData = new CompiledPermissionPolicy { IsEnabled = true, Directives = ["Test", "Example"] };
        _mockCache.Setup(x => x.Get<CompiledPermissionPolicy>(It.IsAny<string>())).Returns(cachedData);

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result, Has.Some.Matches<HeaderDto>(x => x.Key == PermissionPolicyConstants.PermissionPolicyHeader && x.Value == "Test, Example"));
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCacheIsEmptyAndSettingsAreDisabled_ThenNoHeadersAreReturned()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = false });

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCacheIsEmptyAndSettingsAreEnabledButThereAreNoDirectives_ThenNoHeadersAreReturned()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = true });
        _mockRepository.Setup(x => x.ListDirectiveFragments(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync([]);

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCacheIsEmptyAndSettingsAreEnabledAndThereIsOneDirective_ThenHeadersAreReturned()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = true });
        _mockRepository.Setup(x => x.ListDirectiveFragments(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(["Test"]);

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result, Has.Some.Matches<HeaderDto>(x => x.Key == PermissionPolicyConstants.PermissionPolicyHeader && x.Value == "Test"));
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCacheIsEmptyAndSettingsAreEnabledAndThereAreTwoDirectives_ThenHeadersAreReturned()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetSettingsAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = true });
        _mockRepository.Setup(x => x.ListDirectiveFragments(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(["Test", "Example"]);

        // Act
        var result = await _service.GetCompiledHeaders(null, null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result, Has.Some.Matches<HeaderDto>(x => x.Key == PermissionPolicyConstants.PermissionPolicyHeader && x.Value == "Test, Example"));
    }

    #region Multi-Site Context-Aware Cache Key Tests

    [Test]
    public async Task GetPermissionPolicySettingsAsync_GivenDifferentContexts_ThenDifferentCacheKeysAreUsed()
    {
        // Arrange
        var globalSettings = new PermissionPolicySettingsModel { IsEnabled = false };
        var appSettings = new PermissionPolicySettingsModel { IsEnabled = true };

        _mockRepository.Setup(x => x.GetSettingsAsync(null, null)).ReturnsAsync(globalSettings);
        _mockRepository.Setup(x => x.GetSettingsAsync("app1", null)).ReturnsAsync(appSettings);

        // Act
        var globalResult = await _service.GetPermissionPolicySettingsAsync(null, null);
        var appResult = await _service.GetPermissionPolicySettingsAsync("app1", null);

        // Assert
        Assert.That(globalResult.IsEnabled, Is.False);
        Assert.That(appResult.IsEnabled, Is.True);
        _mockRepository.Verify(x => x.GetSettingsAsync(null, null), Times.Once);
        _mockRepository.Verify(x => x.GetSettingsAsync("app1", null), Times.Once);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenDifferentContexts_ThenDifferentCacheKeysAreUsed()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetSettingsAsync(null, null)).ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = false });
        _mockRepository.Setup(x => x.GetSettingsAsync("app1", null)).ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = true });
        _mockRepository.Setup(x => x.ListDirectiveFragments("app1", null)).ReturnsAsync(["Test"]);

        // Act
        var globalResult = await _service.GetCompiledHeaders(null, null);
        var appResult = await _service.GetCompiledHeaders("app1", null);

        // Assert
        Assert.That(globalResult, Is.Empty);
        Assert.That(appResult, Has.Count.EqualTo(1));
    }

    #endregion

    #region Multi-Site HasDirectiveOverride Tests

    [Test]
    public async Task HasOverrideAsync_WhenSettingsByContextReturnsNonNull_ThenReturnsTrue()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetSettingsByContextAsync("app1", null))
            .ReturnsAsync(new PermissionPolicySettingsModel());

        // Act
        var result = await _service.ExistsForContextAsync("app1", null);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasOverrideAsync_WhenSettingsByContextReturnsNull_ThenReturnsFalse()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetSettingsByContextAsync("app1", null))
            .ReturnsAsync((PermissionPolicySettingsModel?)null);

        // Act
        var result = await _service.ExistsForContextAsync("app1", null);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region Multi-Site CreateDirectiveOverride Tests

    [Test]
    public void CreateDirectiveOverrideAsync_WhenModifiedByIsNull_ThenExceptionIsThrown()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateOverrideAsync("app1", null, null));
    }

    [Test]
    public async Task CreateDirectiveOverrideAsync_WhenCreatingAppLevelOverride_ThenSourceIsGlobal()
    {
        // Act
        await _service.CreateOverrideAsync("app1", null, "test.user");

        // Assert
        _mockRepository.Verify(x => x.CreateOverrideAsync(null, null, "app1", null, "test.user"), Times.Once);
    }

    [Test]
    public async Task CreateDirectiveOverrideAsync_WhenCreatingHostLevelOverride_ThenSourceIsAppLevel()
    {
        // Act
        await _service.CreateOverrideAsync("app1", "host1", "test.user");

        // Assert
        _mockRepository.Verify(x => x.CreateOverrideAsync("app1", null, "app1", "host1", "test.user"), Times.Once);
    }

    [Test]
    public async Task CreateDirectiveOverrideAsync_ThenCacheIsCleared()
    {
        // Act
        await _service.CreateOverrideAsync("app1", null, "test.user");

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    #endregion

    #region Multi-Site DeleteByContextAsync Tests

    [Test]
    public void DeleteByContextAsync_WhenDeletedByIsNull_ThenExceptionIsThrown()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteByContextAsync("app1", null, null));
    }

    [Test]
    public async Task DeleteByContextAsync_WhenValid_ThenRepositoryIsCalledAndCacheIsCleared()
    {
        // Act
        await _service.DeleteByContextAsync("app1", null, "test.user");

        // Assert
        _mockRepository.Verify(x => x.DeleteByContextAsync("app1", null, "test.user"), Times.Once);
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    #endregion

    #region Multi-Site SaveDirective Context Pass-Through Tests

    [Test]
    public async Task SaveDirectiveAsync_WhenAppIdProvided_ThenContextIsPassedToRepository()
    {
        // Arrange
        var model = new SavePermissionPolicyModel
        {
            Name = "Test",
            EnabledState = PermissionPolicyEnabledState.All,
            Sources = []
        };

        // Act
        await _service.SaveDirectiveAsync(model, "test.user", "app1", "host1");

        // Assert
        _mockRepository.Verify(x => x.SaveDirectiveAsync(model, "test.user", "app1", "host1"), Times.Once);
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    #endregion
}