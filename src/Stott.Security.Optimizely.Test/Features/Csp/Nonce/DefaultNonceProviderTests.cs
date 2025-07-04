﻿using EPiServer.Core;
using EPiServer.Web.Routing;

using Microsoft.AspNetCore.Http;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Nonce;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

namespace Stott.Security.Optimizely.Test.Features.Csp.Nonce;

[TestFixture]
public sealed class DefaultNonceProviderTests
{
    private Mock<IHttpContextAccessor> _mockHttpContextAccessor;

    private Mock<ICspSettingsService> _mockCspSettingsService;

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

        var nonceProvider = new DefaultNonceProvider(_mockCspSettingsService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);

        // Act
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
        _mockCspSettingsService.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = true, IsNonceEnabled = true });

        _mockHttpRequest.Setup(x => x.Path).Returns(new PathString(pathValue));

        var nonceProvider = new DefaultNonceProvider(_mockCspSettingsService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);

        // Act
        var nonce = nonceProvider.GetNonce();
        var nonceIsNull = nonce is null;

        // Assert
        Assert.That(nonceIsNull, Is.EqualTo(shouldBeNull));
    }

    [Test]
    public void GetNonce_ReturnsNullIfNotRenderingAPage()
    {
        // Assert
        _mockPageRouteHelper.Setup(x => x.PageLink).Returns((PageReference)null);
        _mockCspSettingsService.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = true, IsNonceEnabled = true });

        var nonceProvider = new DefaultNonceProvider(_mockCspSettingsService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);

        // Act
        var nonce = nonceProvider.GetNonce();

        // Assert
        Assert.That(nonce, Is.Null);
    }

    [Test]
    public void GetNonce_ReturnsNonceIfRederingContextDoesContainContentData()
    {
        // Assert
        _mockPageRouteHelper.Setup(x => x.PageLink).Returns(new PageReference(1));
        _mockCspSettingsService.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = true, IsNonceEnabled = true });

        var nonceProvider = new DefaultNonceProvider(_mockCspSettingsService.Object, _mockHttpContextAccessor.Object, _mockPageRouteHelper.Object);

        // Act
        var nonce = nonceProvider.GetNonce();

        // Assert
        Assert.That(nonce, Is.Not.Null);
    }
}
