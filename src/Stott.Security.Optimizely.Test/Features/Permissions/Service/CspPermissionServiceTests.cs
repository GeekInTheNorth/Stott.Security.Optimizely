namespace Stott.Security.Optimizely.Test.Features.Permissions.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Permissions.Repository;
using Stott.Security.Optimizely.Features.Permissions.Service;

[TestFixture]
public class CspPermissionServiceTests
{
    private Mock<ICspPermissionRepository> _mockRepository;

    private Mock<ICacheWrapper> _mockCache;

    private CspPermissionService _service;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICspPermissionRepository>();

        _mockCache = new Mock<ICacheWrapper>();

        _service = new CspPermissionService(_mockRepository.Object, _mockCache.Object);
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionWhenGivenANullRepository()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new CspPermissionService(null, _mockCache.Object));
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionWhenGivenANullCache()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new CspPermissionService(_mockRepository.Object, null));
    }

    [Test]
    public void Constructor_DoesNotThrowsAnExceptionWhenGivenAValidParameters()
    {
        Assert.DoesNotThrow(() => _ = new CspPermissionService(_mockRepository.Object, _mockCache.Object));
    }

    [Test]
    public async Task AppendDirectiveAsync_CallsAppendDirectiveAsyncOnTheRepository()
    {
        // Arrange
        var user = "test.user";

        // Act
        await _service.AppendDirectiveAsync(CspConstants.Sources.Self, CspConstants.Directives.DefaultSource, user);

        // Assert
        _mockRepository.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task AppendDirectiveAsync_ClearsTheCompiledCspCacheAfterSaving()
    {
        // Arrange
        var user = "test.user";

        // Act
        await _service.AppendDirectiveAsync(CspConstants.Sources.Self, CspConstants.Directives.DefaultSource, user);

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_CallsDeleteAsyncOnTheRepository()
    {
        // Arrange
        var user = "test.user";

        // Act
        await _service.DeleteAsync(Guid.NewGuid(), user);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ClearsTheCompiledCspCacheAfterSaving()
    {
        // Arrange
        var user = "test.user";

        // Act
        await _service.DeleteAsync(Guid.NewGuid(), user);

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task SaveAsync_CallsSaveAsyncOnTheRepository()
    {
        // Arrange
        var user = "test.user";

        // Act
        await _service.SaveAsync(Guid.NewGuid(), CspConstants.Sources.Self, CspConstants.AllDirectives, user);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task SaveAsync_ClearsTheCompiledCspCacheAfterSaving()
    {
        // Arrange
        var user = "test.user";

        // Act
        await _service.SaveAsync(Guid.NewGuid(), CspConstants.Sources.Self, CspConstants.AllDirectives, user);

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task GetAsync_CallsGetAsyncOnTheRepository()
    {
        // Act
        await _service.GetAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
    }
}
