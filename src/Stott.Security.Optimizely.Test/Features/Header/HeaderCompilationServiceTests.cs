namespace Stott.Security.Optimizely.Test.Features.Header;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.ServiceLocation;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp;
using Stott.Security.Optimizely.Features.Csp.Nonce;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Pages;
using Stott.Security.Optimizely.Features.SecurityHeaders.Service;

[TestFixture]
public sealed class HeaderCompilationServiceTests
{
    private Mock<ISecurityHeaderService> _securityHeaderService;

    private Mock<ICspReportUrlResolver> _mockReportUrlResolver;

    private Mock<INonceProvider> _mockNonceProvider;

    private Mock<ICacheWrapper> _cacheWrapper;

    private Mock<IServiceProvider> _mockServiceProvider;

    private HeaderCompilationService _service;

    [SetUp]
    public void SetUp()
    {
        _securityHeaderService = new Mock<ISecurityHeaderService>();
        _securityHeaderService.Setup(x => x.GetCompiledHeaders()).ReturnsAsync([]);

        _mockReportUrlResolver = new Mock<ICspReportUrlResolver>();

        _mockNonceProvider = new Mock<INonceProvider>();

        _cacheWrapper = new Mock<ICacheWrapper>();

        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceProvider.Setup(x => x.GetService(typeof(ISecurityHeaderService))).Returns(_securityHeaderService.Object);

        ServiceLocator.SetServiceProvider(_mockServiceProvider.Object);

        _service = new HeaderCompilationService(
            _mockReportUrlResolver.Object,
            _mockNonceProvider.Object,
            _cacheWrapper.Object);
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenThereIsNoPageData()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(null);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenPageDataIsNotACspPage()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };
        var mockPageData = new Mock<PageData>(MockBehavior.Loose);

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(mockPageData.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenPageDataIsACspPageWithANullSourceCollection()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };
        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns((IList<PageCspSourceMapping>)null);

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(mockPageData.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenPageDataIsACspPageWithAnEmptySourceCollection()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };
        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns(new List<PageCspSourceMapping>(0));

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(mockPageData.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesPageSpecificCacheKeyWhenPageDataIsACspPageWithSources()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new Dictionary<string, string> { { "HeaderOne", "HeaderOneValues" } };

        var pageSources = new List<PageCspSourceMapping>
        {
            new() { Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.DefaultSource }
        };

        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns(pageSources);

        _cacheWrapper.Setup(x => x.Get<Dictionary<string, string>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(mockPageData.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.Not.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
        Assert.That(cacheKeyUsed.Contains(CspConstants.CacheKeys.CompiledHeaders), Is.True);
    }
}