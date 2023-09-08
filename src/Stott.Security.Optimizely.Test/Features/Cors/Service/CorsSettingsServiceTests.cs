namespace Stott.Security.Optimizely.Test.Features.Cors.Service;

using System;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Cors.Repository;
using Stott.Security.Optimizely.Features.Cors.Service;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public sealed class CorsSettingsServiceTests
{
    private Mock<ICacheWrapper> _mockCache;

    private Mock<ICorsSettingsRepository> _mockRepository;

    private CorsSettingsService _service;

    [SetUp]
    public void SetUp()
    {
        _mockCache = new Mock<ICacheWrapper>();

        _mockRepository = new Mock<ICorsSettingsRepository>();

        _service = new CorsSettingsService(_mockCache.Object, _mockRepository.Object);
    }

    [Test]
    public async Task GetAsync_GivenCacheIsEmpty_ThenCorsConfigurationWillBeRetrievedFromTheRepositoryAndCached()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<CorsConfiguration>(It.IsAny<string>()))
                  .Returns((CorsConfiguration)null);

        // Act
        _ = await _service.GetAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<CorsConfiguration>()), Times.Once);
    }

    [Test]
    public async Task GetAsync_GivenCacheIsNotEmpty_ThenCorsConfigurationWillNotBeRetrievedFromTheRepository()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<CorsConfiguration>(It.IsAny<string>()))
                  .Returns(new CorsConfiguration());

        // Act
        _ = await _service.GetAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAsync(), Times.Never);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<CorsConfiguration>()), Times.Never);
    }

    [Test]
    public void SaveAsync_GivenANullCorsConfiguration_ThenAnArgumentNullExceptionWillBeThrown()
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _service.SaveAsync(null, "a.name"));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveAsync_GivenANullModifiedBy_ThenAnArgumentNullExceptionWillBeThrown(string modifiedBy)
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _service.SaveAsync(new CorsConfiguration(), modifiedBy));
    }

    [Test]
    public async Task SaveAsync_GivenAValidParameters_ThenCacheWillBeCleared()
    {
        // Act
        await _service.SaveAsync(new CorsConfiguration(), "a.name");

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task SaveAsync_GivenAValidParameters_ThenConfigurationWillBeSaved()
    {
        // Act
        await _service.SaveAsync(new CorsConfiguration(), "a.name");

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<CorsConfiguration>(), It.IsAny<string>()), Times.Once);
    }
}
