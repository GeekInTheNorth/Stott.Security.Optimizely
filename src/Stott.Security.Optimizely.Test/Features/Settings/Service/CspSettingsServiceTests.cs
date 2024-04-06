namespace Stott.Security.Optimizely.Test.Features.Settings.Service;

using System;
using System.Threading.Tasks;

using EPiServer.ServiceLocation;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Settings;
using Stott.Security.Optimizely.Features.Settings.Repository;
using Stott.Security.Optimizely.Features.Settings.Service;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public class CspSettingsServiceTests
{
    private Mock<ICspSettingsRepository> _mockRepository;

    private Mock<ICacheWrapper> _mockCache;

    private Mock<IServiceProvider> _mockServiceProvider;

    private CspSettingsService _service;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICspSettingsRepository>();

        _mockCache = new Mock<ICacheWrapper>();

        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceProvider.Setup(x => x.GetService(typeof(ICspSettingsRepository))).Returns(_mockRepository.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(ICacheWrapper))).Returns(_mockCache.Object);

        ServiceLocator.SetServiceProvider(_mockServiceProvider.Object);

        _service = new CspSettingsService();
    }

    [Test]
    public async Task GetAsync_CallsGetAsyncOnTheRepository()
    {
        // Act
        _ = await _service.GetAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Test]
    public async Task SaveAsync_PerformsNoActionsWhenModelIsNull()
    {
        // Act
        await _service.SaveAsync(null, "test.user");

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<ICspSettings>(), It.IsAny<string>()), Times.Never);
        _mockCache.Verify(x => x.RemoveAll(), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task SaveAsync_PerformsNoActionsWhenWhenPassedANullOrEmptyModifiedBy(string modifiedBy)
    {
        // Act
        await _service.SaveAsync(new CspSettingsModel(), modifiedBy);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<ICspSettings>(), It.IsAny<string>()), Times.Never);
        _mockCache.Verify(x => x.RemoveAll(), Times.Never);
    }

    [Test]
    public async Task SaveAsync_CallsSaveAsyncOnTheRepository()
    {
        // Arrange
        var model = new CspSettingsModel { AllowListUrl = "https://www.example.com" };

        // Act
        await _service.SaveAsync(model, "test.user");

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<ICspSettings>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task SaveAsync_ClearsTheCompiledCspCacheAfterSaving()
    {
        // Arrange
        var model = new CspSettingsModel { AllowListUrl = "https://www.example.com" };

        // Act
        await _service.SaveAsync(model, "test.user");

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }
}