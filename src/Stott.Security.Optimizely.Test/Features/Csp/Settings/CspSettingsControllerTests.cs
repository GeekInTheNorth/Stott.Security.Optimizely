namespace Stott.Security.Optimizely.Test.Features.Csp.Settings;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

[TestFixture]
public sealed class CspSettingsControllerTests
{
    private static readonly Guid TestSiteId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private Mock<ICspSettingsService> _mockService;

    private Mock<ILogger<CspSettingsController>> _mockLogger;

    private CspSettingsController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ICspSettingsService>();
        _mockLogger = new Mock<ILogger<CspSettingsController>>();

        _controller = new CspSettingsController(_mockService.Object, _mockLogger.Object);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "test.user"),
            new("name", "test.user"),
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        _mockService
            .Setup(x => x.GetAsync(It.IsAny<Guid?>(), It.IsAny<string>()))
            .ReturnsAsync(new CspSettings());
    }

    [Test]
    public async Task Get_GivenSiteIdAndDirtyHostName_ThenBothAreForwardedAsReceivedAndHostIsSanitized()
    {
        // Act
        await _controller.Get(TestSiteId, "https://example.com/");

        // Assert
        _mockService.Verify(x => x.GetAsync(TestSiteId, "example.com"), Times.Once);
        _mockService.Verify(x => x.ExistsForContextAsync(TestSiteId, "example.com"), Times.Once);
    }

    [Test]
    [TestCase("https://example.com/", "example.com")]
    [TestCase("http://example.com/", "example.com")]
    [TestCase("example.com/", "example.com")]
    [TestCase("example.com", "example.com")]
    public async Task Get_GivenADirtyHostName_ThenTheHostIsSanitizedBeforeBeingPassedToTheService(string dirtyHost, string expected)
    {
        // Act
        await _controller.Get(TestSiteId, dirtyHost);

        // Assert
        _mockService.Verify(x => x.GetAsync(TestSiteId, expected), Times.Once);
    }

    [Test]
    public async Task Get_WhenExistsForContextReturnsFalse_ThenIsInheritedIsTrue()
    {
        // Arrange
        _mockService.Setup(x => x.ExistsForContextAsync(It.IsAny<Guid?>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var response = await _controller.Get(TestSiteId, null) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(200));
        Assert.That(response.Content, Does.Contain("\"isInherited\":true"));
    }

    [Test]
    public async Task Get_WhenExistsForContextReturnsTrue_ThenIsInheritedIsFalse()
    {
        // Arrange
        _mockService.Setup(x => x.ExistsForContextAsync(It.IsAny<Guid?>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        var response = await _controller.Get(TestSiteId, null) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Content, Does.Contain("\"isInherited\":false"));
    }

    [Test]
    public void Get_WhenTheServiceThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        _mockService.Setup(x => x.GetAsync(It.IsAny<Guid?>(), It.IsAny<string>())).ThrowsAsync(new Exception("boom"));

        // Act + Assert
        Assert.ThrowsAsync<Exception>(() => _controller.Get(TestSiteId, null));
    }

    [Test]
    public async Task Save_GivenAnInvalidModelState_ThenAValidationResponseIsReturned()
    {
        // Arrange
        _controller.ModelState.AddModelError("field", "error");

        // Act
        var response = await _controller.Save(new CspSettingsModel()) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(400));
        _mockService.Verify(x => x.SaveAsync(It.IsAny<CspSettingsModel>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Save_GivenADirtyHostInTheModel_ThenTheServiceReceivesTheSanitizedHost()
    {
        // Arrange
        var model = new CspSettingsModel { SiteId = TestSiteId, HostName = "https://example.com/" };

        // Act
        await _controller.Save(model);

        // Assert
        _mockService.Verify(x => x.SaveAsync(model, "test.user", TestSiteId, "example.com"), Times.Once);
    }

    [Test]
    public async Task Delete_GivenANullSiteId_ThenAValidationErrorIsReturnedAndTheServiceIsNotCalled()
    {
        // Act
        var response = await _controller.Delete(null, null) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(400));
        _mockService.Verify(x => x.DeleteByContextAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Delete_GivenAnEmptyGuidSiteId_ThenAValidationErrorIsReturnedAndTheServiceIsNotCalled()
    {
        // Act
        var response = await _controller.Delete(Guid.Empty, null) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(400));
        _mockService.Verify(x => x.DeleteByContextAsync(It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Delete_GivenAValidSiteId_ThenTheServiceIsCalledWithTheSanitizedHost()
    {
        // Act
        var response = await _controller.Delete(TestSiteId, "https://example.com/");

        // Assert
        Assert.That(response, Is.InstanceOf<OkResult>());
        _mockService.Verify(x => x.DeleteByContextAsync(TestSiteId, "example.com", "test.user"), Times.Once);
    }
}
