namespace Stott.Security.Core.Test.Features.SecurityHeaders.Service;

using System;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Core.Common;
using Stott.Security.Core.Features.Caching;
using Stott.Security.Core.Features.SecurityHeaders.Enums;
using Stott.Security.Core.Features.SecurityHeaders.Repository;
using Stott.Security.Core.Features.SecurityHeaders.Service;

[TestFixture]
public class SecurityHeaderServiceTests
{
    private Mock<ISecurityHeaderRepository> _mockRepository;

    private Mock<ICacheWrapper> _mockCache;

    private SecurityHeaderService _service;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ISecurityHeaderRepository>();

        _mockCache = new Mock<ICacheWrapper>();

        _service = new SecurityHeaderService(_mockRepository.Object, _mockCache.Object);
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionWhenGivenANullRepository()
    {
        Assert.Throws<ArgumentNullException>(() => new SecurityHeaderService(null, _mockCache.Object));
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionWhenGivenANullCache()
    {
        Assert.Throws<ArgumentNullException>(() => new SecurityHeaderService(_mockRepository.Object, null));
    }

    [Test]
    public void Constructor_DoesNotThrowsAnExceptionWhenGivenAValidParameters()
    {
        Assert.DoesNotThrow(() => new SecurityHeaderService(_mockRepository.Object, _mockCache.Object));
    }

    [Test]
    public async Task GetAsync_CallsGetAsyncOnTheRepository()
    {
        // Act
        _ = await _service.GetAsync();

        // Verify
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Test]
    public async Task SaveAsync_CallsSaveAsyncOnTheRepository()
    {
        // Act
        await _service.SaveAsync(true, true, ReferrerPolicy.None, XFrameOptions.None);

        // Verify
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<ReferrerPolicy>(), It.IsAny<XFrameOptions>()), Times.Once);
    }

    [Test]
    public async Task SaveAsync_ClearsTheCompiledCspCacheAfterSaving()
    {
        // Act
        await _service.SaveAsync(true, true, ReferrerPolicy.None, XFrameOptions.None);

        // Verify
        _mockCache.Verify(x => x.Remove(CspConstants.CacheKeys.CompiledCsp), Times.Once);
    }
}
