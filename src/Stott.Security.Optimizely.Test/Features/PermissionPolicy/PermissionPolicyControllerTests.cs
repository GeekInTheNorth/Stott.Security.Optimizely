using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Service;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy;

[TestFixture]
public sealed class PermissionPolicyControllerTests
{
    private Mock<IPermissionPolicyService> _mockService;

    private Mock<ILogger<PermissionPolicyController>> _mockLogger;

    private PermissionPolicyController _controller;

    private static readonly Guid ConcretePropagationSiteId = Guid.Parse("77777777-7777-7777-7777-777777777777");

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IPermissionPolicyService>();
        _mockLogger = new Mock<ILogger<PermissionPolicyController>>();

        _controller = new PermissionPolicyController(_mockService.Object, _mockLogger.Object);

        var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "test.user"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim("name", "test.user"),
            };

        var identity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var context = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            }
        };

        // Then set it to controller before executing test
        _controller.ControllerContext = context;
    }

    [Test]
    [TestCase(null, PermissionPolicyEnabledFilter.All)]
    [TestCase("https://www.example.com", PermissionPolicyEnabledFilter.AllEnabled)]
    public async Task List_CallsListDirectivesAsyncOnTheService(string sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        // Act
        var result = await _controller.List(sourceFilter, enabledFilter, null, null);

        // Assert
        _mockService.Verify(x => x.ListDirectivesAsync(It.IsAny<Guid?>(), It.IsAny<string>(), sourceFilter, enabledFilter), Times.Once);
    }

    [Test]
    public async Task Save_WhenModelStateIsNotValid_ReturnsValidationErrorJson()
    {
        // Arrange
        _controller.ModelState.AddModelError("key", "error");

        // Act
        var result = await _controller.Save(new SavePermissionPolicyModel());
        var contentResult = result as ContentResult;

        // Assert
        Assert.That(contentResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task Save_WhenModelStateIsValid_CallsSaveDirectiveAsyncOnTheService()
    {
        // Arrange
        _controller.ModelState.Clear();

        // Act
        var result = await _controller.Save(new SavePermissionPolicyModel());

        // Assert
        _mockService.Verify(x => x.SaveDirectiveAsync(It.IsAny<SavePermissionPolicyModel>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void Save_WhenServiceThrowsAnException_ThenTheErrorIsIsRethrownAfterLogging()
    {
        // Arrange
        _controller.ModelState.Clear();
        _mockService.Setup(x => x.SaveDirectiveAsync(It.IsAny<SavePermissionPolicyModel>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>())).ThrowsAsync(new Exception());

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.Save(new SavePermissionPolicyModel()));
    }

    [Test]
    public async Task GetSettings_CallsGetPermissionPolicySettingsAsyncOnTheService()
    {
        // Act
        var result = await _controller.GetSettings(null, null);

        // Assert
        _mockService.Verify(x => x.GetPermissionPolicySettingsAsync(It.IsAny<Guid?>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task SaveSettings_WhenModelStateIsNotValid_ReturnsValidationErrorJson()
    {
        // Arrange
        _controller.ModelState.AddModelError("key", "error");

        // Act
        var result = await _controller.SaveSettings(new PermissionPolicySettingsModel());
        var contentResult = result as ContentResult;

        // Assert
        Assert.That(contentResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task SaveSettings_WhenModelStateIsValid_CallsSaveSettingsAsyncOnTheService()
    {
        // Arrange
        _controller.ModelState.Clear();

        // Act
        var result = await _controller.SaveSettings(new PermissionPolicySettingsModel());

        // Assert
        _mockService.Verify(x => x.SaveSettingsAsync(It.IsAny<PermissionPolicySettingsModel>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void SaveSettings_WhenServiceThrowsAnException_ThenTheErrorIsIsRethrownAfterLogging()
    {
        // Arrange
        _controller.ModelState.Clear();
        _mockService.Setup(x => x.SaveSettingsAsync(It.IsAny<PermissionPolicySettingsModel>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>())).ThrowsAsync(new Exception());

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.SaveSettings(new PermissionPolicySettingsModel()));
    }

    [Test]
    public async Task List_GivenConcreteSiteIdAndDirtyHost_ThenServiceIsCalledWithSanitizedHost()
    {
        // Act
        await _controller.List(null, PermissionPolicyEnabledFilter.All, ConcretePropagationSiteId, "https://example.com/");

        // Assert
        _mockService.Verify(x => x.ListDirectivesAsync(ConcretePropagationSiteId, "example.com", null, PermissionPolicyEnabledFilter.All), Times.Once);
    }

    [Test]
    public async Task GetSettings_GivenConcreteSiteIdAndDirtyHost_ThenServiceIsCalledWithSanitizedHost()
    {
        // Arrange
        _mockService
            .Setup(x => x.GetPermissionPolicySettingsAsync(It.IsAny<Guid?>(), It.IsAny<string>()))
            .ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = true });

        // Act
        await _controller.GetSettings(ConcretePropagationSiteId, "https://example.com/");

        // Assert
        _mockService.Verify(x => x.ExistsForContextAsync(ConcretePropagationSiteId, "example.com"), Times.Once);
        _mockService.Verify(x => x.GetPermissionPolicySettingsAsync(ConcretePropagationSiteId, "example.com"), Times.Once);
    }

    [Test]
    public async Task Save_GivenADirtyHostInTheModel_ThenTheServiceReceivesTheSanitizedHost()
    {
        // Arrange
        _controller.ModelState.Clear();
        var model = new SavePermissionPolicyModel
        {
            SiteId = ConcretePropagationSiteId,
            HostName = "https://example.com/"
        };

        // Act
        await _controller.Save(model);

        // Assert
        _mockService.Verify(x => x.SaveDirectiveAsync(model, "test.user", ConcretePropagationSiteId, "example.com"), Times.Once);
    }

    [Test]
    public async Task SaveSettings_GivenADirtyHostInTheModel_ThenTheServiceReceivesTheSanitizedHost()
    {
        // Arrange
        _controller.ModelState.Clear();
        var model = new PermissionPolicySettingsModel
        {
            SiteId = ConcretePropagationSiteId,
            HostName = "https://example.com/",
            IsEnabled = true,
        };

        // Act
        await _controller.SaveSettings(model);

        // Assert
        _mockService.Verify(x => x.SaveSettingsAsync(model, "test.user", ConcretePropagationSiteId, "example.com"), Times.Once);
    }
}
