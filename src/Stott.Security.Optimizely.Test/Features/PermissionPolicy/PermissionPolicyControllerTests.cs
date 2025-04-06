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
        var result = await _controller.List(sourceFilter, enabledFilter);

        // Assert
        _mockService.Verify(x => x.ListDirectivesAsync(sourceFilter, enabledFilter), Times.Once);
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
        _mockService.Verify(x => x.SaveDirectiveAsync(It.IsAny<SavePermissionPolicyModel>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void Save_WhenServiceThrowsAnException_ThenTheErrorIsIsRethrownAfterLogging()
    {
        // Arrange
        _controller.ModelState.Clear();
        _mockService.Setup(x => x.SaveDirectiveAsync(It.IsAny<SavePermissionPolicyModel>(), It.IsAny<string>())).ThrowsAsync(new Exception());

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.Save(new SavePermissionPolicyModel()));
    }

    [Test]
    public async Task GetSettings_CallsGetPermissionPolicySettingsAsyncOnTheService()
    {
        // Act
        var result = await _controller.GetSettings();

        // Assert
        _mockService.Verify(x => x.GetPermissionPolicySettingsAsync(), Times.Once);
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
        _mockService.Verify(x => x.SaveSettingsAsync(It.IsAny<PermissionPolicySettingsModel>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void SaveSettings_WhenServiceThrowsAnException_ThenTheErrorIsIsRethrownAfterLogging()
    {
        // Arrange
        _controller.ModelState.Clear();
        _mockService.Setup(x => x.SaveSettingsAsync(It.IsAny<PermissionPolicySettingsModel>(), It.IsAny<string>())).ThrowsAsync(new Exception());

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.SaveSettings(new PermissionPolicySettingsModel()));
    }
}
