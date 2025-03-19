using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Caching;
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
        var result = await _service.GetPermissionPolicySettingsAsync();

        // Assert
        Assert.That(result, Is.EqualTo(settings));
        _mockRepository.Verify(x => x.GetSettingsAsync(), Times.Never);
    }

    [Test]
    public async Task GetPermissionPolicySettingsAsync_WhenSettingsAreNotInTheCache_ThenDataIsRetrievedFromTheRepository()
    {
        // Arrange
        var settings = new PermissionPolicySettingsModel();
        _mockRepository.Setup(x => x.GetSettingsAsync()).ReturnsAsync(settings);

        // Act
        var result = await _service.GetPermissionPolicySettingsAsync();

        // Assert
        Assert.That(result, Is.EqualTo(settings));
        _mockRepository.Verify(x => x.GetSettingsAsync(), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), settings), Times.Once);
    }

    [Test]
    public async Task ListDirectivesAsync_WhenDirectivesAreInTheCache_ThenDataIsNotRetrievedFromTheRepository()
    {
        // Arrange
        var directives = new List<PermissionPolicyDirectiveModel>();
        _mockCache.Setup(x => x.Get<List<PermissionPolicyDirectiveModel>>(It.IsAny<string>())).Returns(directives);

        // Act
        var result = await _service.ListDirectivesAsync(null, PermissionPolicyEnabledFilter.All);

        // Assert
        Assert.That(result, Is.EqualTo(directives));
        _mockRepository.Verify(x => x.ListDirectivesAsync(), Times.Never);
    }

    [Test]
    public async Task ListDirectivesAsync_WhenDirectivesAreNotInTheCache_ThenDataIsRetrievedFromTheRepository()
    {
        // Arrange
        var directives = new List<PermissionPolicyDirectiveModel>();
        _mockRepository.Setup(x => x.ListDirectivesAsync()).ReturnsAsync(directives);

        // Act
        var result = await _service.ListDirectivesAsync(null, PermissionPolicyEnabledFilter.All);

        // Assert
        Assert.That(result, Is.EqualTo(directives));
        _mockRepository.Verify(x => x.ListDirectivesAsync(), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), directives), Times.Once);
    }

    [Test]
    public async Task ListDirectivesAsync_WhenTheRepositoryReturnsNoDirectives_ThenDefaultDirectivesAreAdded()
    {
        // Arrange
        var directives = new List<PermissionPolicyDirectiveModel>();
        _mockRepository.Setup(x => x.ListDirectivesAsync()).ReturnsAsync(directives);

        // Act
        var result = await _service.ListDirectivesAsync(null, PermissionPolicyEnabledFilter.All);

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
        _mockRepository.Setup(x => x.ListDirectivesAsync()).ReturnsAsync(directives);

        // Act
        var result = await _service.ListDirectivesAsync(null, PermissionPolicyEnabledFilter.All);

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
        _mockRepository.Setup(x => x.ListDirectivesAsync()).ReturnsAsync(directives);

        // Act
        var result = await _service.ListDirectivesAsync(sourceFilter, enabledFilter);
        var filteredDirectives = string.Join(",", result.Select(x => x.Name));

        // Assert
        Assert.That(filteredDirectives, Is.EqualTo(expectedDirectives));
    }

    [Test]
    public void SaveDirectiveAsync_WhenModelIsNull_ThenAnExceptionIsThrown()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.SaveDirectiveAsync(null, "Test"));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveDirectiveAsync_WhenModifiedByIsNullOrWhitespace_ThenAnExceptionIsThrown(string modifiedBy)
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.SaveDirectiveAsync(new SavePermissionPolicyModel(), modifiedBy));
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
        await _service.SaveDirectiveAsync(model, "Test");

        // Assert
        _mockRepository.Verify(x => x.SaveDirectiveAsync(model, "Test"), Times.Once);
    }

    [Test]
    public void SaveSettingsAsync_WhenModelIsNull_ThenAnExceptionIsThrown()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.SaveSettingsAsync(null, "Test"));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveSettingsAsync_WhenModifiedByIsNullOrWhitespace_ThenAnExceptionIsThrown(string modifiedBy)
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.SaveSettingsAsync(new PermissionPolicySettingsModel(), modifiedBy));
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
        await _service.SaveSettingsAsync(model, "Test");

        // Assert
        _mockRepository.Verify(x => x.SaveSettingsAsync(model, "Test"), Times.Once);
    }
}