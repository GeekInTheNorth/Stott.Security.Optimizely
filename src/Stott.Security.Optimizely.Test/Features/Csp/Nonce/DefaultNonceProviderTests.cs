using System.Collections.Generic;
using EPiServer.Core;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Moq;

using NUnit.Framework;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Nonce;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

namespace Stott.Security.Optimizely.Test.Features.Csp.Nonce;

[TestFixture]
public sealed class DefaultNonceProviderTests
{
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    private Mock<ICspSettingsService> _mockCspSettingsService;

    private Mock<ICspPermissionService> _mockCspPermissionService;

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

        _mockCspSettingsService = new Mock<ICspSettingsService>();

        _mockCspPermissionService = new Mock<ICspPermissionService>();
    }

    [Test]
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    public void GetNonce_ReturnsNullWhenCspOrNonceIsDisabled(bool isEnabled, bool isNonceEnabled)
    {
        // Assert
        _mockCspSettingsService.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = isEnabled, IsNonceEnabled = isNonceEnabled });
        _mockPageRouteHelper.Setup(x => x.PageLink).Returns(new PageReference(1));

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockCspSettingsService.Object, _mockCspPermissionService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);
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
        var sources = new List<CspSource>
        {
            new() { Source = CspConstants.Sources.Nonce, Directives = CspConstants.Directives.ScriptSource }
        };
        _mockCspPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);
        _mockCspSettingsService.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = true, IsNonceEnabled = true });
        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString(pathValue));

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockCspSettingsService.Object, _mockCspPermissionService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);
        var nonce = nonceProvider.GetNonce();
        var nonceIsNull = nonce is null;

        // Assert
        Assert.That(nonceIsNull, Is.EqualTo(shouldBeNull));
    }

    [Test]
    public void GetNonce_ReturnsNullIfNotRenderingAPage()
    {
        // Assert
        var sources = new List<CspSource>
        {
            new() { Source = CspConstants.Sources.Nonce, Directives = CspConstants.Directives.ScriptSource }
        };
        _mockCspPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);
        _mockPageRouteHelper.Setup(x => x.PageLink).Returns((PageReference)null);
        _mockCspSettingsService.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = true, IsNonceEnabled = true });

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockCspSettingsService.Object, _mockCspPermissionService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);
        var nonce = nonceProvider.GetNonce();

        // Assert
        Assert.That(nonce, Is.Null);
    }

    [Test]
    public void GetNonce_ReturnsNonceIfRederingContextDoesContainContentData()
    {
        // Assert
        var sources = new List<CspSource>
        {
            new() { Source = CspConstants.Sources.Nonce, Directives = CspConstants.Directives.ScriptSource }
        };
        _mockCspPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);
        _mockCspSettingsService.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = true, IsNonceEnabled = true });
        _mockPageRouteHelper.Setup(x => x.PageLink).Returns(new PageReference(1));

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockCspSettingsService.Object, _mockCspPermissionService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);
        var nonce = nonceProvider.GetNonce();

        // Assert
        Assert.That(nonce, Is.Not.Null);
    }
}
