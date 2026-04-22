namespace Stott.Security.Optimizely.Test.Features.Csp.Settings.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EPiServer.ServiceLocation;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Settings;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public sealed class CspSettingsServiceTests
{
    private Mock<ICspSettingsRepository> _mockRepository;

    private Mock<ICacheWrapper> _mockCache;

    private Mock<IServiceProvider> _mockServiceProvider;

    private CspSettingsService _service;

    private static readonly Guid SiteAId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly Guid SiteBId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICspSettingsRepository>();

        _mockCache = new Mock<ICacheWrapper>();

        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceProvider.Setup(x => x.GetService(typeof(ICspSettingsRepository))).Returns(_mockRepository.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(ICacheWrapper))).Returns(_mockCache.Object);

        ServiceLocator.SetServiceProvider(_mockServiceProvider.Object);

        _service = new CspSettingsService(_mockRepository.Object, _mockCache.Object);
    }

    [Test]
    public async Task GetAsync_CallsGetAsyncOnTheRepository()
    {
        // Act
        _ = await _service.GetAsync(null, null);

        // Assert
        _mockRepository.Verify(x => x.GetAsync(It.IsAny<Guid?>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task SaveAsync_PerformsNoActionsWhenModelIsNull()
    {
        // Act
        await _service.SaveAsync(null, "test.user", null, null);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<ICspSettings>(), It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockCache.Verify(x => x.RemoveAll(), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task SaveAsync_PerformsNoActionsWhenWhenPassedANullOrEmptyModifiedBy(string modifiedBy)
    {
        // Act
        await _service.SaveAsync(new CspSettingsModel(), modifiedBy, null, null);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<ICspSettings>(), It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockCache.Verify(x => x.RemoveAll(), Times.Never);
    }

    [Test]
    public async Task SaveAsync_CallsSaveAsyncOnTheRepository()
    {
        // Arrange
        var model = new CspSettingsModel { AllowListUrl = "https://www.example.com" };

        // Act
        await _service.SaveAsync(model, "test.user", null, null);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<ICspSettings>(), It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task SaveAsync_ClearsTheCompiledCspCacheAfterSaving()
    {
        // Arrange
        var model = new CspSettingsModel { AllowListUrl = "https://www.example.com" };

        // Act
        await _service.SaveAsync(model, "test.user", null, null);

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task ExistsForContextAsync_GivenGlobalScope_ReturnsFalseWithoutHittingRepository()
    {
        // Act
        var result = await _service.ExistsForContextAsync(null, null);

        // Assert
        Assert.That(result, Is.False);
        _mockRepository.Verify(x => x.GetByContextAsync(It.IsAny<Guid?>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ExistsForContextAsync_GivenSiteScope_UsesTheRepositoryOnCacheMiss()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<ContextStateModel>(It.IsAny<string>())).Returns((ContextStateModel)null);
        _mockRepository.Setup(x => x.GetByContextAsync(SiteAId, "h")).ReturnsAsync(new CspSettings());

        // Act
        var result = await _service.ExistsForContextAsync(SiteAId, "h");

        // Assert
        Assert.That(result, Is.True);
        _mockRepository.Verify(x => x.GetByContextAsync(SiteAId, "h"), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<ContextStateModel>()), Times.Once);
    }

    [Test]
    public async Task ExistsForContextAsync_GivenSiteScope_UsesCachedValueOnCacheHit()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<ContextStateModel>(It.IsAny<string>())).Returns(new ContextStateModel { Exists = true });

        // Act
        var result = await _service.ExistsForContextAsync(SiteAId, null);

        // Assert
        Assert.That(result, Is.True);
        _mockRepository.Verify(x => x.GetByContextAsync(It.IsAny<Guid?>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task DeleteByContextAsync_GivenNullSiteId_DoesNothing()
    {
        // Act
        await _service.DeleteByContextAsync(null, null, "user");

        // Assert
        _mockRepository.Verify(x => x.DeleteByContextAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockCache.Verify(x => x.RemoveAll(), Times.Never);
    }

    [Test]
    public async Task DeleteByContextAsync_GivenEmptyGuidSiteId_DoesNothing()
    {
        // Act
        await _service.DeleteByContextAsync(Guid.Empty, null, "user");

        // Assert
        _mockRepository.Verify(x => x.DeleteByContextAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task DeleteByContextAsync_GivenWhitespaceDeletedBy_DoesNothing()
    {
        // Act
        await _service.DeleteByContextAsync(SiteAId, null, "  ");

        // Assert
        _mockRepository.Verify(x => x.DeleteByContextAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task DeleteByContextAsync_GivenValidSiteAndUser_CallsRepositoryAndClearsCache()
    {
        // Act
        await _service.DeleteByContextAsync(SiteAId, "host.com", "user");

        // Assert
        _mockRepository.Verify(x => x.DeleteByContextAsync(SiteAId, "host.com", "user"), Times.Once);
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task GetAsync_DifferentContextsProduceDifferentCacheKeys()
    {
        // Arrange
        var capturedKeys = new HashSet<string>();
        _mockCache.Setup(x => x.Get<CspSettings>(It.IsAny<string>()))
                  .Returns(new CspSettings())
                  .Callback<string>(key => capturedKeys.Add(key));

        // Act
        _ = await _service.GetAsync(null, null);
        _ = await _service.GetAsync(SiteAId, null);
        _ = await _service.GetAsync(SiteAId, "host.com");
        _ = await _service.GetAsync(SiteBId, null);

        // Assert
        Assert.That(capturedKeys.Count, Is.EqualTo(4));
    }
}