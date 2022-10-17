namespace Stott.Security.Core.Test.Features.Settings.Service;

using System;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Caching;
using Stott.Security.Core.Features.Settings;
using Stott.Security.Core.Features.Settings.Repository;
using Stott.Security.Core.Features.Settings.Service;

[TestFixture]
public class CspSettingsServiceTests
{
    private Mock<ICspSettingsRepository> _mockRepository;

    private Mock<ICacheWrapper> _mockCache;

    private CspSettingsService _service;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICspSettingsRepository>();

        _mockCache = new Mock<ICacheWrapper>();

        _service = new CspSettingsService(_mockRepository.Object, _mockCache.Object);
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionWhenGivenANullRepository()
    {
        Assert.Throws<ArgumentNullException>(() => new CspSettingsService(null, _mockCache.Object));
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionWhenGivenANullCache()
    {
        Assert.Throws<ArgumentNullException>(() => new CspSettingsService(_mockRepository.Object, null));
    }

    [Test]
    public void Constructor_DoesNotThrowsAnExceptionWhenGivenAValidParameters()
    {
        Assert.DoesNotThrow(() => new CspSettingsService(_mockRepository.Object, _mockCache.Object));
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
    public async Task SaveAsync_CallsSaveAsyncOnTheRepository()
    {
        // Act
        await _service.SaveAsync(new CspSettingsModel());

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task SaveAsync_ClearsTheCompiledCspCacheAfterSaving()
    {
        // Act
        await _service.SaveAsync(new CspSettingsModel());

        // Assert
        _mockCache.Verify(x => x.Remove(CspConstants.CacheKeys.CompiledCsp), Times.Once);
    }
}
