namespace Stott.Security.Optimizely.Test.Features.Sandbox.Service;

using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Features.Sandbox.Repository;
using Stott.Security.Optimizely.Features.Sandbox.Service;

[TestFixture]
public class CspSandboxServiceTests
{
    private Mock<ICspSandboxRepository> _mockRepository;

    private Mock<ICacheWrapper> _mockCacheWrapper;

    private CspSandboxService _service;

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
        _ = await _service.GetAsync();

        // Assert
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Test]
    public async Task SaveAsync_CallsSaveAsyncOnTheRepositoryAndThenClearsCache()
    {
        // Act
        await _service.SaveAsync(new SandboxModel(), "test.user");

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<SandboxModel>(), It.IsAny<string>()), Times.Once);
        _mockCacheWrapper.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
    }
}