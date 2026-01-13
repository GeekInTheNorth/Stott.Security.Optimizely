namespace Stott.Security.Optimizely.Test.Features.Header;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using EPiServer.Core;
using EPiServer.ServiceLocation;

using Microsoft.AspNetCore.Http;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp;
using Stott.Security.Optimizely.Features.Csp.Nonce;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Pages;
using Stott.Security.Optimizely.Features.PermissionPolicy.Service;
using Stott.Security.Optimizely.Features.Route;
using Stott.Security.Optimizely.Features.SecurityHeaders.Service;

[TestFixture]
public sealed class HeaderCompilationServiceTests
{
    private Mock<ISecurityHeaderService> _securityHeaderService;

    private Mock<ICspReportUrlResolver> _mockReportUrlResolver;

    private Mock<INonceProvider> _mockNonceProvider;

    private Mock<ICacheWrapper> _cacheWrapper;

    private Mock<ICspService> _mockCspService;

    private Mock<IPermissionPolicyService> _mockPermissionPolicyService;

    private Mock<IServiceProvider> _mockServiceProvider;

    private Mock<HttpRequest> _mockHttpRequest;

    private Mock<SecurityRouteData> _mockRouteData;

    private HeaderCompilationService _service;

    [SetUp]
    public void SetUp()
    {
        _securityHeaderService = new Mock<ISecurityHeaderService>();
        _securityHeaderService.Setup(x => x.GetCompiledHeaders()).ReturnsAsync([]);

        _mockReportUrlResolver = new Mock<ICspReportUrlResolver>();

        _mockNonceProvider = new Mock<INonceProvider>();

        _cacheWrapper = new Mock<ICacheWrapper>();

        _mockCspService = new Mock<ICspService>();

        _mockPermissionPolicyService = new Mock<IPermissionPolicyService>();

        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceProvider.Setup(x => x.GetService(typeof(ISecurityHeaderService))).Returns(_securityHeaderService.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(ICspService))).Returns(_mockCspService.Object);
        _mockServiceProvider.Setup(x => x.GetService(typeof(IPermissionPolicyService))).Returns(_mockPermissionPolicyService.Object);

        ServiceLocator.SetServiceProvider(_mockServiceProvider.Object);

        _mockHttpRequest = new Mock<HttpRequest>();

        _mockRouteData = new Mock<SecurityRouteData>();

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
        var headers = new List<HeaderDto> { new HeaderDto { Key = "HeaderOne", Value = "HeaderOneValues" } };

        _cacheWrapper.Setup(x => x.Get<List<HeaderDto>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(null, _mockHttpRequest.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenPageDataIsNotACspPage()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new List<HeaderDto> { new HeaderDto { Key = "HeaderOne", Value = "HeaderOneValues" } };
        var mockPageData = new Mock<PageData>(MockBehavior.Loose);
        _mockRouteData.Setup(x => x.Content).Returns(mockPageData.Object);

        _cacheWrapper.Setup(x => x.Get<List<HeaderDto>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(_mockRouteData.Object, _mockHttpRequest.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenPageDataIsACspPageWithANullSourceCollection()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new List<HeaderDto> { new HeaderDto { Key = "HeaderOne", Value = "HeaderOneValues" } };
        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns((IList<PageCspSourceMapping>)null);
        _mockRouteData.Setup(x => x.Content).Returns(mockPageData.Object);

        _cacheWrapper.Setup(x => x.Get<List<HeaderDto>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(_mockRouteData.Object, _mockHttpRequest.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesDefaultCacheKeyWhenPageDataIsACspPageWithAnEmptySourceCollection()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new List<HeaderDto> { new HeaderDto { Key = "HeaderOne", Value = "HeaderOneValues" } };
        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns(new List<PageCspSourceMapping>(0));
        _mockRouteData.Setup(x => x.Content).Returns(mockPageData.Object);

        _cacheWrapper.Setup(x => x.Get<List<HeaderDto>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(_mockRouteData.Object, _mockHttpRequest.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_UsesPageSpecificCacheKeyWhenPageDataIsACspPageWithSources()
    {
        // Arrange
        string cacheKeyUsed = null;
        var headers = new List<HeaderDto> { new HeaderDto { Key = "HeaderOne", Value = "HeaderOneValues" } };

        var pageSources = new List<PageCspSourceMapping>
        {
            new() { Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.DefaultSource }
        };

        var mockPageData = new Mock<TestPageData>(MockBehavior.Loose);
        mockPageData.Setup(x => x.ContentSecurityPolicySources).Returns(pageSources);
        _mockRouteData.Setup(x => x.Content).Returns(mockPageData.Object);

        _cacheWrapper.Setup(x => x.Get<List<HeaderDto>>(It.IsAny<string>()))
                     .Returns(headers)
                     .Callback<string>(x => cacheKeyUsed = x);

        // Act
        _ = await _service.GetSecurityHeadersAsync(_mockRouteData.Object, _mockHttpRequest.Object);

        // Assert
        Assert.That(cacheKeyUsed, Is.Not.EqualTo(CspConstants.CacheKeys.CompiledHeaders));
        Assert.That(cacheKeyUsed.Contains(CspConstants.CacheKeys.CompiledHeaders), Is.True);
    }

    [Test]
    public async Task GetSecurityHeadersAsync_GivenReportingEndpointHeaderIsPresent_ThenInternalReportingPlaceholderIsUpdated()
    {
        // Arrange
        var headers = new List<HeaderDto>
        {
            new() { Key = CspConstants.HeaderNames.ContentSecurityPolicy, Value = "default-src 'self'; report-to /report" },
            new() { Key = CspConstants.HeaderNames.ReportingEndpoints, Value = $"stott-security-endpoint=\"{CspConstants.InternalReportingPlaceholder}\""}
        };

        _cacheWrapper.Setup(x => x.Get<List<HeaderDto>>(It.IsAny<string>()))
                     .Returns(headers);

        _mockReportUrlResolver.Setup(x => x.GetReportToPath()).Returns("https://example.com/report");

        // Act
        var result = await _service.GetSecurityHeadersAsync(null, _mockHttpRequest.Object);
        var reportingHeader = result.Find(x => x.Key == CspConstants.HeaderNames.ReportingEndpoints);

        // Assert
        Assert.That(reportingHeader?.Value, Is.EqualTo("stott-security-endpoint=\"https://example.com/report\""));
    }

    [Test]
    public async Task GetSecurityHeadersAsync_GivenReportingEndpointHeaderIsPresent_AndInternalAndExternalReportingIsEnabled_ThenInternalReportingPlaceholderIsUpdated()
    {
        // Arrange
        var headers = new List<HeaderDto>
        {
            new() { Key = CspConstants.HeaderNames.ContentSecurityPolicy, Value = "default-src 'self'; report-to /report" },
            new() { Key = CspConstants.HeaderNames.ReportingEndpoints, Value = $"stott-security-endpoint=\"{CspConstants.InternalReportingPlaceholder}\", stott-security-external-endpoint=\"https://www.external.com/report/\""}
        };

        _cacheWrapper.Setup(x => x.Get<List<HeaderDto>>(It.IsAny<string>()))
                     .Returns(headers);

        _mockReportUrlResolver.Setup(x => x.GetReportToPath()).Returns("https://example.com/report");

        // Act
        var result = await _service.GetSecurityHeadersAsync(null, _mockHttpRequest.Object);
        var reportingHeader = result.Find(x => x.Key == CspConstants.HeaderNames.ReportingEndpoints);

        // Assert
        Assert.That(reportingHeader?.Value, Is.EqualTo("stott-security-endpoint=\"https://example.com/report\", stott-security-external-endpoint=\"https://www.external.com/report/\""));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task GetSecurityHeadersAsync_OnlyIncludesStrictTransportSecurityHeaderForHttpsRequests(bool isHttps)
    {
        // Arrange
        var headers = new List<HeaderDto>
        {
            new() { Key = CspConstants.HeaderNames.StrictTransportSecurity, Value = "max-age=31536000; includeSubDomains" },
            new() { Key = "Another-Header", Value = "HeaderValue" }
        };
        _cacheWrapper.Setup(x => x.Get<List<HeaderDto>>(It.IsAny<string>()))
                     .Returns(headers);
        _mockHttpRequest.Setup(x => x.IsHttps).Returns(isHttps);
        
        // Act
        var result = await _service.GetSecurityHeadersAsync(null, _mockHttpRequest.Object);
        
        // Assert
        Assert.That(result.Exists(x => x.Key == CspConstants.HeaderNames.StrictTransportSecurity), Is.EqualTo(isHttps));
        Assert.That(result.Exists(x => x.Key == "Another-Header"), Is.True);
    }
}