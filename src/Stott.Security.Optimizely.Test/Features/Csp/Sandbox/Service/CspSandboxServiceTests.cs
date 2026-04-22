namespace Stott.Security.Optimizely.Test.Features.Csp.Sandbox.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Service;

[TestFixture]
public sealed class CspSandboxServiceTests
{
    private Mock<ICspSandboxRepository> _mockRepository;

    private Mock<ICacheWrapper> _mockCacheWrapper;

    private CspSandboxService _service;

    private static readonly Guid SiteAId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly Guid SiteBId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICspSandboxRepository>();

        _mockCacheWrapper = new Mock<ICacheWrapper>();

        _service = new CspSandboxService(_mockRepository.Object, _mockCacheWrapper.Object);
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
    public async Task SaveAsync_CallsSaveAsyncOnTheRepositoryAndThenClearsCache()
    {
        // Act
        await _service.SaveAsync(new SandboxModel(), "test.user", null, null);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<SandboxModel>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>()), Times.Once);
        _mockCacheWrapper.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task ExistsForContextAsync_GivenGlobalScope_ReturnsExpectedSentinel()
    {
        // The sandbox service returns true for the Global scope (it always exists).
        // Act
        var result = await _service.ExistsForContextAsync(null, null);

        // Assert
        Assert.That(result, Is.True);
        _mockRepository.Verify(x => x.GetByContextAsync(It.IsAny<Guid?>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ExistsForContextAsync_GivenSiteScope_UsesTheRepositoryOnCacheMiss()
    {
        // Arrange
        _mockCacheWrapper.Setup(x => x.Get<ContextStateModel>(It.IsAny<string>())).Returns((ContextStateModel)null);
        _mockRepository.Setup(x => x.GetByContextAsync(SiteAId, "h")).ReturnsAsync(new SandboxModel());

        // Act
        var result = await _service.ExistsForContextAsync(SiteAId, "h");

        // Assert
        Assert.That(result, Is.True);
        _mockRepository.Verify(x => x.GetByContextAsync(SiteAId, "h"), Times.Once);
        _mockCacheWrapper.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<ContextStateModel>()), Times.Once);
    }

    [Test]
    public async Task ExistsForContextAsync_GivenSiteScope_UsesCachedValueOnCacheHit()
    {
        // Arrange
        _mockCacheWrapper.Setup(x => x.Get<ContextStateModel>(It.IsAny<string>())).Returns(new ContextStateModel { Exists = true });

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
        _mockCacheWrapper.Verify(x => x.RemoveAll(), Times.Never);
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
        _mockCacheWrapper.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task GetAsync_DifferentContextsProduceDifferentCacheKeys()
    {
        // Arrange
        var capturedKeys = new HashSet<string>();
        _mockCacheWrapper.Setup(x => x.Get<SandboxModel>(It.IsAny<string>()))
                         .Returns(new SandboxModel())
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