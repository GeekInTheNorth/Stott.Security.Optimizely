namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.Service;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;
using Stott.Security.Optimizely.Test.TestCases;

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
    [TestCase("default-src", "self", null)]
    [TestCase("default-src", "self", "")]
    [TestCase("default-src", "self", " ")]
    [TestCase("default-src", null, "test.user")]
    [TestCase("default-src", "", "test.user")]
    [TestCase("default-src", " ", "test.user")]
    [TestCase(null, "self", "test.user")]
    [TestCase("", "self", "test.user")]
    [TestCase(" ", "self", "test.user")]
    public async Task AppendDirectiveAsync_DoesNotPerformAnyActionWhenGivenANullorEmptyParameters(string source, string directive, string modifiedBy)
    {
        // Act
        await _service.AppendDirectiveAsync(source, directive, modifiedBy);

        // Assert
        _mockRepository.Verify(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockCache.Verify(x => x.RemoveAll(), Times.Never);
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
    public async Task DeleteAsync_DoesNotPerformAnyActionWhenGivenAnEmptyId()
    {
        // Act
        await _service.DeleteAsync(Guid.Empty, "test.user");

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        _mockCache.Verify(x => x.RemoveAll(), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task DeleteAsync_DoesNotPerformAnyActionWhenGivenANullOrEmptyDeletedBy(string deletedBy)
    {
        // Act
        await _service.DeleteAsync(Guid.NewGuid(), deletedBy);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        _mockCache.Verify(x => x.RemoveAll(), Times.Never);
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
        // Act
        await _service.SaveAsync(Guid.NewGuid(), CspConstants.Sources.Self, CspConstants.AllDirectives, "test.user");

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task SaveAsync_ClearsTheCompiledCspCacheAfterSaving()
    {
        // Act
        await _service.SaveAsync(Guid.NewGuid(), CspConstants.Sources.Self, CspConstants.AllDirectives, "test.user");

        // Assert
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task GetAsync_GivenCacheIsNotYetPopulated_CallsGetAsyncOnTheRepositoryAndAddsTheResultToTheCache()
    {
        // Act
        await _service.GetAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<IList<CspSource>>()), Times.Once);
    }

    [Test]
    public async Task GetAsync_GivenCacheIsPopulatedWithEmptySources_CallsGetAsyncOnTheRepositoryAndAddsTheResultToTheCache()
    {
        // Arrange
        _mockCache.Setup(x => x.Get<IList<CspSource>>(It.IsAny<string>())).Returns([]);

        // Act
        await _service.GetAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<IList<CspSource>>()), Times.Once);
    }

    [Test]
    public async Task GetAsync_GivenCacheIsPopulatedWithSources_ReturnsTheSourcesFromTheCache()
    {
        // Arrange
        var sources = new List<CspSource> { new() { Id = Guid.NewGuid(), Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.DefaultSource } };
        _mockCache.Setup(x => x.Get<IList<CspSource>>(It.IsAny<string>())).Returns(sources);

        // Act
        var result = await _service.GetAsync();

        // Assert
        Assert.That(result, Is.EquivalentTo(sources));
    }

    [Test]
    public async Task GetAsync_GivenCacheIsPopulatedWithSources_ThenTheRepositoryIsNotCalled()
    {
        // Arrange
        var sources = new List<CspSource> { new() { Id = Guid.NewGuid(), Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.DefaultSource } };
        _mockCache.Setup(x => x.Get<IList<CspSource>>(It.IsAny<string>())).Returns(sources);

        // Act
        await _service.GetAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAsync(), Times.Never);
    }

    [Test]
    public async Task GetAsync_GivenSourcesExist_WhenGetAsyncIsCalledAgain_ThenTheRepositoryAndCacheAreNotCalledAgain()
    {
        // Arrange
        var sources = new List<CspSource> { new() { Id = Guid.NewGuid(), Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.DefaultSource } };
        _mockRepository.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        await _service.GetAsync();

        // Act A Second Time
        await _service.GetAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockCache.Verify(x => x.Add(It.IsAny<string>(), It.IsAny<IList<CspSource>>()), Times.Once);
    }
}
