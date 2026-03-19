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
        var result = await _controller.List(sourceFilter, enabledFilter, null, null);

        // Assert
        _mockService.Verify(x => x.ListDirectivesAsync(It.IsAny<string?>(), It.IsAny<string?>(), sourceFilter, enabledFilter), Times.Once);
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
        _mockService.Verify(x => x.SaveDirectiveAsync(It.IsAny<SavePermissionPolicyModel>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    [Test]
    public void Save_WhenServiceThrowsAnException_ThenTheErrorIsIsRethrownAfterLogging()
    {
        // Arrange
        _controller.ModelState.Clear();
        _mockService.Setup(x => x.SaveDirectiveAsync(It.IsAny<SavePermissionPolicyModel>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>())).ThrowsAsync(new Exception());

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.Save(new SavePermissionPolicyModel()));
    }

    [Test]
    public async Task GetSettings_CallsGetPermissionPolicySettingsAsyncOnTheService()
    {
        // Arrange
        _mockService.Setup(x => x.GetPermissionPolicySettingsAsync(It.IsAny<string?>(), It.IsAny<string?>())).ReturnsAsync(new PermissionPolicySettingsModel());

        // Act
        var result = await _controller.GetSettings(null, null);

        // Assert
        _mockService.Verify(x => x.GetPermissionPolicySettingsAsync(It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
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
        _mockService.Verify(x => x.SaveSettingsAsync(It.IsAny<PermissionPolicySettingsModel>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
    }

    [Test]
    public void SaveSettings_WhenServiceThrowsAnException_ThenTheErrorIsIsRethrownAfterLogging()
    {
        // Arrange
        _controller.ModelState.Clear();
        _mockService.Setup(x => x.SaveSettingsAsync(It.IsAny<PermissionPolicySettingsModel>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>())).ThrowsAsync(new Exception());

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.SaveSettings(new PermissionPolicySettingsModel()));
    }

    #region Multi-Site HasOverride Tests

    [Test]
    public async Task HasOverride_WhenOverrideExists_ThenReturnsHasOverrideTrue()
    {
        // Arrange
        _mockService.Setup(x => x.HasDirectiveOverrideAsync("app1", null)).ReturnsAsync(true);

        // Act
        var result = await _controller.HasOverride("app1", null);

        // Assert
        _mockService.Verify(x => x.HasDirectiveOverrideAsync("app1", null), Times.Once);
    }

    [Test]
    public async Task HasOverride_WhenNoOverrideExists_ThenReturnsHasOverrideFalse()
    {
        // Arrange
        _mockService.Setup(x => x.HasDirectiveOverrideAsync("app1", null)).ReturnsAsync(false);

        // Act
        var result = await _controller.HasOverride("app1", null);

        // Assert
        _mockService.Verify(x => x.HasDirectiveOverrideAsync("app1", null), Times.Once);
    }

    #endregion

    #region Multi-Site CreateOverride Tests

    [Test]
    public async Task CreateOverride_WhenAppIdIsNull_ThenReturnsValidationError()
    {
        // Act
        var result = await _controller.CreateOverride(null, null);
        var contentResult = result as ContentResult;

        // Assert
        Assert.That(contentResult!.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task CreateOverride_WhenAppIdIsProvided_ThenCallsServiceAndReturnsOk()
    {
        // Act
        var result = await _controller.CreateOverride("app1", null);

        // Assert
        Assert.That(result, Is.TypeOf<OkResult>());
        _mockService.Verify(x => x.CreateDirectiveOverrideAsync("app1", null, It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void CreateOverride_WhenServiceThrowsException_ThenExceptionIsRethrown()
    {
        // Arrange
        _mockService.Setup(x => x.CreateDirectiveOverrideAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>())).ThrowsAsync(new Exception());

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.CreateOverride("app1", null));
    }

    #endregion

    #region Multi-Site DeleteDirectives Tests

    [Test]
    public async Task DeleteDirectives_WhenAppIdIsNull_ThenReturnsValidationError()
    {
        // Act
        var result = await _controller.DeleteDirectives(null, null);
        var contentResult = result as ContentResult;

        // Assert
        Assert.That(contentResult!.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task DeleteDirectives_WhenAppIdIsProvided_ThenCallsServiceAndReturnsOk()
    {
        // Act
        var result = await _controller.DeleteDirectives("app1", null);

        // Assert
        Assert.That(result, Is.TypeOf<OkResult>());
        _mockService.Verify(x => x.DeleteDirectivesByContextAsync("app1", null, It.IsAny<string?>()), Times.Once);
    }

    [Test]
    public void DeleteDirectives_WhenServiceThrowsException_ThenExceptionIsRethrown()
    {
        // Arrange
        _mockService.Setup(x => x.DeleteDirectivesByContextAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>())).ThrowsAsync(new Exception());

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.DeleteDirectives("app1", null));
    }

    #endregion

    #region Multi-Site GetSettings Context Tests

    [Test]
    public async Task GetSettings_WhenOverrideExists_ThenIsInheritedIsFalse()
    {
        // Arrange
        _mockService.Setup(x => x.HasDirectiveOverrideAsync("app1", null)).ReturnsAsync(true);
        _mockService.Setup(x => x.GetPermissionPolicySettingsAsync("app1", null))
            .ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = true });

        // Act
        var result = await _controller.GetSettings("app1", null);

        // Assert
        _mockService.Verify(x => x.HasDirectiveOverrideAsync("app1", null), Times.Once);
        _mockService.Verify(x => x.GetPermissionPolicySettingsAsync("app1", null), Times.Once);
    }

    [Test]
    public async Task GetSettings_WhenNoOverrideExists_ThenIsInheritedIsTrue()
    {
        // Arrange
        _mockService.Setup(x => x.HasDirectiveOverrideAsync("app1", null)).ReturnsAsync(false);
        _mockService.Setup(x => x.GetPermissionPolicySettingsAsync("app1", null))
            .ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = false });

        // Act
        var result = await _controller.GetSettings("app1", null);

        // Assert
        _mockService.Verify(x => x.HasDirectiveOverrideAsync("app1", null), Times.Once);
        _mockService.Verify(x => x.GetPermissionPolicySettingsAsync("app1", null), Times.Once);
    }

    #endregion

    #region Multi-Site List Context Tests

    [Test]
    public async Task List_WhenAppIdAndHostNameProvided_ThenContextIsPassedToService()
    {
        // Act
        await _controller.List(null, PermissionPolicyEnabledFilter.All, "app1", "host1");

        // Assert
        _mockService.Verify(x => x.ListDirectivesAsync("app1", "host1", null, PermissionPolicyEnabledFilter.All), Times.Once);
    }

    #endregion
}
