namespace Stott.Security.Optimizely.Test.Features.CustomHeaders.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;
using Stott.Security.Optimizely.Features.CustomHeaders.Repository;
using Stott.Security.Optimizely.Features.CustomHeaders.Service;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public sealed class CustomHeaderServiceTests
{
    private Mock<ICustomHeaderRepository> _mockRepository;

    private Mock<ICacheWrapper> _mockCache;

    private CustomHeaderService _service;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICustomHeaderRepository>();
        _mockCache = new Mock<ICacheWrapper>();

        _service = new CustomHeaderService(_mockRepository.Object, _mockCache.Object);
    }

    [Test]
    public async Task GetAllAsync_GivenCacheHit_ThenReturnsCachedData()
    {
        // Arrange
        var cachedData = new List<CustomHeaderModel>
        {
            new() { HeaderName = "X-Cached", Behavior = CustomHeaderBehavior.Add, HeaderValue = "cached" }
        };
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns(cachedData);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.That(result, Is.SameAs(cachedData));
    }

    [Test]
    public async Task GetAllAsync_GivenCacheHit_ThenRepositoryIsNotCalled()
    {
        // Arrange
        var cachedData = new List<CustomHeaderModel>();
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns(cachedData);

        // Act
        await _service.GetAllAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Test]
    public async Task GetAllAsync_GivenCacheMiss_ThenCallsRepository()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>());

        // Act
        await _service.GetAllAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_GivenCacheMiss_ThenAddsToCacheAfterFetch()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>());

        // Act
        await _service.GetAllAsync();

        // Assert
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<List<CustomHeaderModel>>()), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_GivenNoSavedHeaders_ThenReturnsEightDefaults()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.That(result.Count, Is.EqualTo(8));
    }

    [Test]
    public async Task GetAllAsync_GivenSomeSavedHeaders_ThenMergesWithMissingDefaults()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>
        {
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.XssProtection, Behavior = CustomHeaderBehavior.Add, HeaderValue = "1", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.FrameOptions, Behavior = CustomHeaderBehavior.Add, HeaderValue = "DENY", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = "X-Custom-One", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" }
        });

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        // 3 saved + 6 missing defaults = 9 total
        Assert.That(result.Count, Is.EqualTo(9));
    }

    [Test]
    public async Task GetAllAsync_GivenAllSavedHeaders_ThenDoesNotDuplicateDefaults()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>
        {
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.XssProtection, Behavior = CustomHeaderBehavior.Add, HeaderValue = "1", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.FrameOptions, Behavior = CustomHeaderBehavior.Add, HeaderValue = "DENY", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.ContentTypeOptions, Behavior = CustomHeaderBehavior.Add, HeaderValue = "nosniff", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.ReferrerPolicy, Behavior = CustomHeaderBehavior.Add, HeaderValue = "no-referrer", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.CrossOriginEmbedderPolicy, Behavior = CustomHeaderBehavior.Add, HeaderValue = "require-corp", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.CrossOriginOpenerPolicy, Behavior = CustomHeaderBehavior.Add, HeaderValue = "same-origin", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.CrossOriginResourcePolicy, Behavior = CustomHeaderBehavior.Add, HeaderValue = "same-origin", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = CspConstants.HeaderNames.StrictTransportSecurity, Behavior = CustomHeaderBehavior.Add, HeaderValue = "max-age=31536000", Modified = DateTime.UtcNow, ModifiedBy = "test" }
        });

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.That(result.Count, Is.EqualTo(8));
    }

    [Test]
    public async Task GetAllAsync_GivenSavedHeaderWithDifferentCase_ThenDefaultMergeIsCaseInsensitive()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>
        {
            new() { Id = Guid.NewGuid(), HeaderName = "x-xss-protection", Behavior = CustomHeaderBehavior.Add, HeaderValue = "1", Modified = DateTime.UtcNow, ModifiedBy = "test" }
        });

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        var xssHeaders = result.Where(x => x.HeaderName.Contains("xss", StringComparison.OrdinalIgnoreCase)).ToList();
        Assert.That(xssHeaders.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetCompiledHeaders_GivenDisabledHeaders_ThenExcludesDisabledHeaders()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>
        {
            new() { Id = Guid.NewGuid(), HeaderName = "X-Enabled", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" },
            new() { Id = Guid.NewGuid(), HeaderName = "X-Disabled", Behavior = CustomHeaderBehavior.Disabled, Modified = DateTime.UtcNow, ModifiedBy = "test" }
        });

        // Act
        var result = await _service.GetCompiledHeaders();

        // Assert
        Assert.That(result.Any(x => x.Key == "X-Disabled"), Is.False);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenAddBehavior_ThenReturnsIsRemovalFalse()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>
        {
            new() { Id = Guid.NewGuid(), HeaderName = "X-Add-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" }
        });

        // Act
        var result = await _service.GetCompiledHeaders();

        // Assert
        var addedHeader = result.FirstOrDefault(x => x.Key == "X-Add-Header");
        Assert.That(addedHeader, Is.Not.Null);
        Assert.That(addedHeader.IsRemoval, Is.False);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenRemoveBehavior_ThenReturnsIsRemovalTrue()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>
        {
            new() { Id = Guid.NewGuid(), HeaderName = "X-Remove-Header", Behavior = CustomHeaderBehavior.Remove, Modified = DateTime.UtcNow, ModifiedBy = "test" }
        });

        // Act
        var result = await _service.GetCompiledHeaders();

        // Assert
        var removedHeader = result.FirstOrDefault(x => x.Key == "X-Remove-Header");
        Assert.That(removedHeader, Is.Not.Null);
        Assert.That(removedHeader.IsRemoval, Is.True);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenNullHeaderValue_ThenReturnsEmptyString()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>
        {
            new() { Id = Guid.NewGuid(), HeaderName = "X-Null-Value", Behavior = CustomHeaderBehavior.Remove, HeaderValue = null, Modified = DateTime.UtcNow, ModifiedBy = "test" }
        });

        // Act
        var result = await _service.GetCompiledHeaders();

        // Assert
        var header = result.FirstOrDefault(x => x.Key == "X-Null-Value");
        Assert.That(header, Is.Not.Null);
        Assert.That(header.Value, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task GetCompiledHeaders_GivenAddBehavior_ThenMapsKeyAndValue()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<List<CustomHeaderModel>>(It.IsAny<string>())).Returns((List<CustomHeaderModel>)null);
        _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>
        {
            new() { Id = Guid.NewGuid(), HeaderName = "X-My-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "my-value", Modified = DateTime.UtcNow, ModifiedBy = "test" }
        });

        // Act
        var result = await _service.GetCompiledHeaders();

        // Assert
        var header = result.FirstOrDefault(x => x.Key == "X-My-Header");
        Assert.That(header, Is.Not.Null);
        Assert.That(header.Value, Is.EqualTo("my-value"));
    }

    [Test]
    public async Task SaveAsync_GivenNullModel_ThenDoesNotCallRepository()
    {
        // Act
        await _service.SaveAsync(null, "test-user");

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<ICustomHeader>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task SaveAsync_GivenNullOrEmptyModifiedBy_ThenDoesNotCallRepository(string modifiedBy)
    {
        // Arrange
        var model = new SaveCustomHeaderModel { HeaderName = "X-Test", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value" };

        // Act
        await _service.SaveAsync(model, modifiedBy);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<ICustomHeader>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task SaveAsync_GivenValidInput_ThenCallsRepositorySaveAsync()
    {
        // Arrange
        var model = new SaveCustomHeaderModel { HeaderName = "X-Test", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value" };

        // Act
        await _service.SaveAsync(model, "test-user");

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(model, "test-user"), Times.Once);
    }

    [Test]
    public async Task SaveAsync_GivenValidInput_ThenClearsCache()
    {
        // Arrange
        var model = new SaveCustomHeaderModel { HeaderName = "X-Test", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value" };

        // Act
        await _service.SaveAsync(model, "test-user");

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ThenCallsRepositoryDeleteAsync()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ThenClearsCache()
    {
        // Act
        await _service.DeleteAsync(Guid.NewGuid());

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }
}
