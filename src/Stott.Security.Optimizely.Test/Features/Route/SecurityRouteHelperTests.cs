using System.Collections.Generic;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using Stott.Security.Optimizely.Features.Configuration;
using Stott.Security.Optimizely.Features.Pages;
using Stott.Security.Optimizely.Features.Route;

namespace Stott.Security.Optimizely.Test.Features.Route;

[TestFixture]
public sealed class SecurityRouteHelperTests
{
    private Mock<IPageRouteHelper> _mockPageRouteHelper;

    private Mock<IContentLoader> _mockContentLoader;

    private Mock<IUrlResolver> _mockUrlResolver;

    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    private Mock<HttpContext> _mockHttpContext;

    private Mock<HttpRequest> _mockHttpRequest;

    private SecurityConfiguration _configuration;

    private SecurityRouteHelper _securityRouteHelper;

    [SetUp]
    public void SetUp()
    {
        _mockPageRouteHelper = new Mock<IPageRouteHelper>();

        _mockContentLoader = new Mock<IContentLoader>();

        _mockUrlResolver = new Mock<IUrlResolver>();

        _mockHttpRequest = new Mock<HttpRequest>();
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/test/path"));

        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpContext.Setup(c => c.Request).Returns(_mockHttpRequest.Object);

        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(_mockHttpContext.Object);

        _configuration = new SecurityConfiguration { ExclusionPaths = [ "/excluded-path", "/another/excluded-path" ] };

        _securityRouteHelper = new SecurityRouteHelper(
            _mockPageRouteHelper.Object,
            _mockContentLoader.Object,
            _mockUrlResolver.Object,
            _mockHttpContextAccessor.Object,
            _configuration);
    }

    [Test]
    [TestCaseSource(typeof(SecurityRouteHelperTestCases), nameof(SecurityRouteHelperTestCases.StandardRouteTestCases))]
    public void GivenAStandardRoute_WhenPathIsInTheExclusionList_ThenRouteTypeIsExclusion(string requestPath, SecurityRouteType expectedRouteType)
    {
        // Arrange
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString(requestPath));

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(expectedRouteType));
    }

    [Test]
    [TestCaseSource(typeof(SecurityRouteHelperTestCases), nameof(SecurityRouteHelperTestCases.StandardRouteTestCases))]
    public void GivenAStandardRouteWithAStandardPage_WhenPathIsInTheExclusionList_ThenRouteTypeIsExclusion(string requestPath, SecurityRouteType expectedRouteType)
    {
        // Arrange
        var mockPageContent = new Mock<IContent>();
        _mockPageRouteHelper.Setup(x => x.Content).Returns(mockPageContent.Object);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString(requestPath));

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(expectedRouteType));
    }

    [Test]
    [TestCaseSource(typeof(SecurityRouteHelperTestCases), nameof(SecurityRouteHelperTestCases.StandardRouteTestCases))]
    public void GivenAStandardRouteWithACspSourcePageWithNullSources_WhenPathIsInTheExclusionList_ThenRouteTypeIsExclusion(string requestPath, SecurityRouteType expectedRouteType)
    {
        // Arrange
        var mockPageContent = new Mock<TestPageData>();
        _mockPageRouteHelper.Setup(x => x.Content).Returns(mockPageContent.Object);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString(requestPath));

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(expectedRouteType));
    }

    [Test]
    [TestCaseSource(typeof(SecurityRouteHelperTestCases), nameof(SecurityRouteHelperTestCases.StandardRouteTestCases))]
    public void GivenAStandardRouteWithACspSourcePageWithEmptySources_WhenPathIsInTheExclusionList_ThenRouteTypeIsExclusion(string requestPath, SecurityRouteType expectedRouteType)
    {
        // Arrange
        var mockPageContent = new Mock<TestPageData>();
        mockPageContent.Setup(x => x.ContentSecurityPolicySources).Returns(new List<PageCspSourceMapping>());
        _mockPageRouteHelper.Setup(x => x.Content).Returns(mockPageContent.Object);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString(requestPath));

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(expectedRouteType));
    }

    [Test]
    [TestCaseSource(typeof(SecurityRouteHelperTestCases), nameof(SecurityRouteHelperTestCases.CspSourceContentRouteTestCases))]
    public void GivenAStandardRouteWithACspSourcePageWithSources_WhenPathIsInTheExclusionList_ThenRouteTypeIsSpecificContentExclusion(string requestPath, SecurityRouteType expectedRouteType)
    {
        // Arrange
        var sources = new List<PageCspSourceMapping>
        {
            new PageCspSourceMapping { Source = "'self'", Directives = "default-src" }
        };
        var mockPageContent = new Mock<TestPageData>();
        mockPageContent.Setup(x => x.ContentSecurityPolicySources).Returns(sources);
        _mockPageRouteHelper.Setup(x => x.Content).Returns(mockPageContent.Object);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString(requestPath));

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(expectedRouteType));
    }

    [Test]
    public void GivenAStandardRoute_OnlyAssessesAndGeneratesSecurityRouteDataOncePerRequest()
    {
        // Arrange
        var requestPath = "/excluded-path/sub-path";
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString(requestPath));
        
        // Act
        var routeData1 = _securityRouteHelper.GetRouteData();
        var routeData2 = _securityRouteHelper.GetRouteData();
        
        // Assert
        Assert.That(routeData1, Is.SameAs(routeData2));
        _mockHttpRequest.Verify(x => x.Path, Times.Once);
    }

    [Test]
    public void GivenACompiledHeadersRouteWithoutAPageIdQueryString_ThenRouteTypeIsAlwaysDefault()
    {
        // Arrange
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/stott.security.optimizely/api/compiled-headers"));
        _mockHttpRequest.Setup(x => x.Query).Returns(new QueryCollection());

        // Act
        var routeData = _securityRouteHelper.GetRouteData();
        
        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(SecurityRouteType.Default));

        _mockContentLoader.Verify(x => x.TryGet(It.IsAny<ContentReference>(), out It.Ref<IContent>.IsAny), Times.Never);
        _mockUrlResolver.Verify(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>()), Times.Never);
    }

    [Test]
    public void GivenACompiledHeadersRouteWithAnInvalidPageIdQueryString_ThenRouteTypeIsAlwaysDefault()
    {
        // Arrange
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/stott.security.optimizely/api/compiled-headers"));
        _mockHttpRequest.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "pageId", "not-a-number" } }));

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(SecurityRouteType.Default));

        _mockContentLoader.Verify(x => x.TryGet(It.IsAny<ContentReference>(), out It.Ref<IContent>.IsAny), Times.Never);
        _mockUrlResolver.Verify(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>()), Times.Never);
    }

    [Test]
    public void GivenACompiledHeadersRouteWithAPageIdQueryStringThatFailsLoading_ThenRouteTypeIsAlwaysDefault()
    {
        // Arrange
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/stott.security.optimizely/api/compiled-headers"));
        _mockHttpRequest.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "pageId", "1" } }));
        _mockContentLoader.Setup(x => x.TryGet(It.IsAny<ContentReference>(), out It.Ref<IContent>.IsAny)).Returns(false);

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(SecurityRouteType.Default));

        _mockContentLoader.Verify(x => x.TryGet(It.IsAny<ContentReference>(), out It.Ref<IContent>.IsAny), Times.Once);
        _mockUrlResolver.Verify(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>()), Times.Never);
    }

    [Test]
    public void GivenACompiledHeadersRouteWithAPageIdQueryStringForNonRoutingContent_ThenRouteTypeIsAlwaysDefault()
    {
        // Arrange
        var mockContent = new Mock<TestPageData>();
        mockContent.Setup(x => x.ContentLink).Returns(new ContentReference(1));
        var mockedContent = mockContent.Object as IContent;

        _mockContentLoader.Setup(x => x.TryGet(It.IsAny<ContentReference>(), out mockedContent)).Returns(true);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/stott.security.optimizely/api/compiled-headers"));
        _mockHttpRequest.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "pageId", "1" } }));
        _mockUrlResolver.Setup(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>())).Returns((string)null);

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(SecurityRouteType.Default));

        _mockContentLoader.Verify(x => x.TryGet(It.IsAny<ContentReference>(), out It.Ref<IContent>.IsAny), Times.Once);
        _mockUrlResolver.Verify(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>()), Times.Once);
    }

    [Test]
    [TestCaseSource(typeof(SecurityRouteHelperTestCases), nameof(SecurityRouteHelperTestCases.StandardRouteTestCases))]
    public void GivenACompiledHeadersRouteWithValidContent_WhenPathIsInTheExclusionList_ThenRouteTypeIsExclusion(string requestPath, SecurityRouteType expectedRouteType)
    {
        // Arrange
        var mockContent = new Mock<PageData>();
        mockContent.Setup(x => x.ContentLink).Returns(new ContentReference(1));
        var mockedContent = mockContent.Object as IContent;

        _mockContentLoader.Setup(x => x.TryGet(It.IsAny<ContentReference>(), out mockedContent)).Returns(true);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/stott.security.optimizely/api/compiled-headers"));
        _mockHttpRequest.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "pageId", "1" } }));
        _mockUrlResolver.Setup(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>())).Returns(requestPath);

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(expectedRouteType));
    }

    [Test]
    [TestCaseSource(typeof(SecurityRouteHelperTestCases), nameof(SecurityRouteHelperTestCases.StandardRouteTestCases))]
    public void GivenACompiledHeadersRouteWithValidCspSourcesContentWithNullSources_WhenPathIsInTheExclusionList_ThenRouteTypeIsExclusion(string requestPath, SecurityRouteType expectedRouteType)
    {
        // Arrange
        var mockContent = new Mock<TestPageData>();
        mockContent.Setup(x => x.ContentLink).Returns(new ContentReference(1));
        mockContent.Setup(x => x.ContentSecurityPolicySources).Returns((IList<PageCspSourceMapping>)null);
        var mockedContent = mockContent.Object as IContent;

        _mockContentLoader.Setup(x => x.TryGet(It.IsAny<ContentReference>(), out mockedContent)).Returns(true);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/stott.security.optimizely/api/compiled-headers"));
        _mockHttpRequest.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "pageId", "1" } }));
        _mockUrlResolver.Setup(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>())).Returns(requestPath);

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(expectedRouteType));
    }

    [Test]
    [TestCaseSource(typeof(SecurityRouteHelperTestCases), nameof(SecurityRouteHelperTestCases.StandardRouteTestCases))]
    public void GivenACompiledHeadersRouteWithValidCspSourcesContentWithEmptySources_WhenPathIsInTheExclusionList_ThenRouteTypeIsExclusion(string requestPath, SecurityRouteType expectedRouteType)
    {
        // Arrange
        var mockContent = new Mock<TestPageData>();
        mockContent.Setup(x => x.ContentLink).Returns(new ContentReference(1));
        mockContent.Setup(x => x.ContentSecurityPolicySources).Returns(new List<PageCspSourceMapping>());
        var mockedContent = mockContent.Object as IContent;

        _mockContentLoader.Setup(x => x.TryGet(It.IsAny<ContentReference>(), out mockedContent)).Returns(true);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/stott.security.optimizely/api/compiled-headers"));
        _mockHttpRequest.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "pageId", "1" } }));
        _mockUrlResolver.Setup(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>())).Returns(requestPath);

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(expectedRouteType));
    }

    [Test]
    [TestCaseSource(typeof(SecurityRouteHelperTestCases), nameof(SecurityRouteHelperTestCases.CspSourceContentRouteTestCases))]
    public void GivenACompiledHeadersRouteWithValidCspSourcesContentWithSources_WhenPathIsInTheExclusionList_ThenRouteTypeIsSpecificContentExclusion(string requestPath, SecurityRouteType expectedRouteType)
    {
        // Arrange
        var sources = new List<PageCspSourceMapping>
        {
            new PageCspSourceMapping { Source = "'self'", Directives = "default-src" }
        };
        var mockContent = new Mock<TestPageData>();
        mockContent.Setup(x => x.ContentLink).Returns(new ContentReference(1));
        mockContent.Setup(x => x.ContentSecurityPolicySources).Returns(sources);
        var mockedContent = mockContent.Object as IContent;

        _mockContentLoader.Setup(x => x.TryGet(It.IsAny<ContentReference>(), out mockedContent)).Returns(true);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/stott.security.optimizely/api/compiled-headers"));
        _mockHttpRequest.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "pageId", "1" } }));
        _mockUrlResolver.Setup(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>())).Returns(requestPath);

        // Act
        var routeData = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData.RouteType, Is.EqualTo(expectedRouteType));
    }

    [Test]
    public void GivenACompiledHeadersRoute_OnlyAssessesAndGeneratesSecurityRouteDataOncePerRequest()
    {
        // Arrange
        var mockContent = new Mock<TestPageData>();
        mockContent.Setup(x => x.ContentLink).Returns(new ContentReference(1));
        var mockedContent = mockContent.Object as IContent;

        _mockContentLoader.Setup(x => x.TryGet(It.IsAny<ContentReference>(), out mockedContent)).Returns(true);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString("/stott.security.optimizely/api/compiled-headers"));
        _mockHttpRequest.Setup(x => x.Query).Returns(new QueryCollection(new Dictionary<string, StringValues> { { "pageId", "1" } }));
        _mockUrlResolver.Setup(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>())).Returns("/");

        // Act
        var routeData1 = _securityRouteHelper.GetRouteData();
        var routeData2 = _securityRouteHelper.GetRouteData();

        // Assert
        Assert.That(routeData1, Is.SameAs(routeData2));
        _mockContentLoader.Verify(x => x.TryGet(It.IsAny<ContentReference>(), out It.Ref<IContent>.IsAny), Times.Once);
        _mockUrlResolver.Verify(x => x.GetUrl(It.IsAny<ContentReference>(), It.IsAny<string>(), It.IsAny<UrlResolverArguments>()), Times.Once);
    }
}