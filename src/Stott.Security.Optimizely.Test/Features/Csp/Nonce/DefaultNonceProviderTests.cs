using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Moq;

using NUnit.Framework;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Nonce;

namespace Stott.Security.Optimizely.Test.Features.Csp.Nonce;

[TestFixture]
public sealed class DefaultNonceProviderTests
{
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    private Mock<INonceService> _mockNonceService;

    private Mock<IPageRouteHelper> _mockPageRouteHelper;

    private Mock<HttpContext> _mockContext;

    private Mock<HttpRequest> _mockHttpRequest;

    [SetUp]
    public void SetUp()
    {
        _mockHttpRequest = new Mock<HttpRequest>();

        _mockContext = new Mock<HttpContext>();
        _mockContext.Setup(x => x.Request).Returns(_mockHttpRequest.Object);

        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockContext.Object);

        _mockPageRouteHelper = new Mock<IPageRouteHelper>();

        _mockNonceService = new Mock<INonceService>();
    }

    [Test]
    [TestCaseSource(typeof(DefaultNonceProviderTestCases), nameof(DefaultNonceProviderTestCases.InvalidSettingsTestCases))]
    public void GetNonce_ReturnsNullWhenCspOrNonceIsDisabled(NonceSettings nonceSettings)
    {
        // Assert
        _mockNonceService.Setup(x => x.GetNonceSettingsAsync()).ReturnsAsync(nonceSettings);
        _mockPageRouteHelper.Setup(x => x.PageLink).Returns(new PageReference(1));

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockNonceService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);
        var nonce = nonceProvider.GetNonce();

        // Assert
        Assert.That(nonce, Is.Null);
    }

    [Test]
    [TestCase("/", true)]
    [TestCase("/some-other-path", true)]
    [TestCase("/stott.security.optimizely/api/compiled-headers", false)]
    [TestCase("/stott.security.optimizely/api/compiled-headers/list", false)]
    [TestCase("/stott.security.optimizely/api/compiled-headers/reporting-endpoints", false)]
    public void GetNonce_ReturnsNullOnNonContentPathExceptForTheCompiledHeadersPath(string pathValue, bool shouldBeNull)
    {
        // Assert
        var nonceSettings = new NonceSettings
        {
            IsEnabled = true,
            Directives = new List<string> { CspConstants.Directives.ScriptSource }
        };

        _mockNonceService.Setup(x => x.GetNonceSettingsAsync()).ReturnsAsync(nonceSettings);
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString(pathValue));

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockNonceService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);
        var nonce = nonceProvider.GetNonce();
        var nonceIsNull = nonce is null;

        // Assert
        Assert.That(nonceIsNull, Is.EqualTo(shouldBeNull));
    }

    [Test]
    public void GetNonce_ReturnsNullIfNotRenderingAPage()
    {
        // Assert
        var nonceSettings = new NonceSettings
        {
            IsEnabled = true,
            Directives = new List<string> { CspConstants.Directives.ScriptSource }
        };
        
        _mockNonceService.Setup(x => x.GetNonceSettingsAsync()).ReturnsAsync(nonceSettings);
        _mockPageRouteHelper.Setup(x => x.PageLink).Returns((PageReference)null);

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockNonceService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);
        var nonce = nonceProvider.GetNonce();

        // Assert
        Assert.That(nonce, Is.Null);
    }

    [Test]
    public void GetNonce_ReturnsNonceIfRederingContextDoesContainContentData()
    {
        // Assert
        var nonceSettings = new NonceSettings
        {
            IsEnabled = true,
            Directives = new List<string> { CspConstants.Directives.ScriptSource }
        };
        
        _mockNonceService.Setup(x => x.GetNonceSettingsAsync()).ReturnsAsync(nonceSettings);
        _mockPageRouteHelper.Setup(x => x.PageLink).Returns(new PageReference(1));

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockNonceService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);
        var nonce = nonceProvider.GetNonce();

        // Assert
        Assert.That(nonce, Is.Not.Null);
    }
}

public static class DefaultNonceProviderTestCases
{
    public static IEnumerable<TestCaseData> InvalidSettingsTestCases()
    {
        yield return new TestCaseData(new NonceSettings()).SetName("Disabled Nonce Settings - Returns Null");
        yield return new TestCaseData(new NonceSettings { IsEnabled = true }).SetName("Nonce Enabled With Null Directives - Returns Null");
        yield return new TestCaseData(new NonceSettings { IsEnabled = true, Directives = [] }).SetName("Nonce Enabled With Empty Directives - Returns Null");
    }
}