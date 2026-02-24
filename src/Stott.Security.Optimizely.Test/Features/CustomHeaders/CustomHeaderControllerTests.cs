namespace Stott.Security.Optimizely.Test.Features.CustomHeaders;

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;
using Stott.Security.Optimizely.Features.CustomHeaders.Service;

[TestFixture]
public sealed class CustomHeaderControllerTests
{
    private Mock<ICustomHeaderService> _mockService;

    private Mock<ILogger<CustomHeaderController>> _mockLogger;

    private CustomHeaderController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ICustomHeaderService>();

        _mockLogger = new Mock<ILogger<CustomHeaderController>>();

        _controller = new CustomHeaderController(_mockService.Object, _mockLogger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "test-user") }, "test"))
                }
            }
        };
    }

    [Test]
    public async Task List_ThenCallsServiceGetAllAsync()
    {
        // Arrange
        _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeaderModel>());

        // Act
        await _controller.List(null, null);

        // Assert
        _mockService.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task List_GivenNoFilters_ThenReturnsAllHeaders()
    {
        // Arrange
        _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeaderModel>
        {
            new() { HeaderName = "X-Header-A", Behavior = CustomHeaderBehavior.Add, HeaderValue = "a" },
            new() { HeaderName = "X-Header-B", Behavior = CustomHeaderBehavior.Remove },
            new() { HeaderName = "X-Header-C", Behavior = CustomHeaderBehavior.Disabled }
        });

        // Act
        var response = await _controller.List(null, null) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        Assert.That(response.Content, Does.Contain("X-Header-A"));
        Assert.That(response.Content, Does.Contain("X-Header-B"));
        Assert.That(response.Content, Does.Contain("X-Header-C"));
    }

    [Test]
    public async Task List_GivenHeaderNameFilter_ThenFiltersResults()
    {
        // Arrange
        _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeaderModel>
        {
            new() { HeaderName = "X-Custom-One", Behavior = CustomHeaderBehavior.Add, HeaderValue = "a" },
            new() { HeaderName = "X-Custom-Two", Behavior = CustomHeaderBehavior.Add, HeaderValue = "b" },
            new() { HeaderName = "X-Other", Behavior = CustomHeaderBehavior.Add, HeaderValue = "c" }
        });

        // Act
        var response = await _controller.List("Custom", null) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Content, Does.Contain("X-Custom-One"));
        Assert.That(response.Content, Does.Contain("X-Custom-Two"));
        Assert.That(response.Content, Does.Not.Contain("X-Other"));
    }

    [Test]
    public async Task List_GivenBehaviorFilter_ThenFiltersResults()
    {
        // Arrange
        _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeaderModel>
        {
            new() { HeaderName = "X-Add-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "a" },
            new() { HeaderName = "X-Remove-Header", Behavior = CustomHeaderBehavior.Remove },
            new() { HeaderName = "X-Disabled-Header", Behavior = CustomHeaderBehavior.Disabled }
        });

        // Act
        var response = await _controller.List(null, CustomHeaderBehavior.Add) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Content, Does.Contain("X-Add-Header"));
        Assert.That(response.Content, Does.Not.Contain("X-Remove-Header"));
        Assert.That(response.Content, Does.Not.Contain("X-Disabled-Header"));
    }

    [Test]
    public async Task List_ThenReturnsSortedByHeaderName()
    {
        // Arrange
        _mockService.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeaderModel>
        {
            new() { HeaderName = "Z-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "z" },
            new() { HeaderName = "A-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "a" },
            new() { HeaderName = "M-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "m" }
        });

        // Act
        var response = await _controller.List(null, null) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        var aIndex = response.Content.IndexOf("A-Header");
        var mIndex = response.Content.IndexOf("M-Header");
        var zIndex = response.Content.IndexOf("Z-Header");
        Assert.That(aIndex, Is.LessThan(mIndex));
        Assert.That(mIndex, Is.LessThan(zIndex));
    }

    [Test]
    public void List_GivenServiceThrowsException_ThenRethrows()
    {
        // Arrange
        _mockService.Setup(x => x.GetAllAsync()).ThrowsAsync(new InvalidOperationException("Test error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => await _controller.List(null, null));
    }

    [Test]
    public async Task Save_GivenInvalidModelState_ThenReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.TryAddModelError("HeaderName", "Header Name is required.");

        // Act
        var response = await _controller.Save(new SaveCustomHeaderModel()) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Save_GivenInvalidModelState_ThenDoesNotCallService()
    {
        // Arrange
        _controller.ModelState.TryAddModelError("HeaderName", "Header Name is required.");

        // Act
        await _controller.Save(new SaveCustomHeaderModel());

        // Assert
        _mockService.Verify(x => x.SaveAsync(It.IsAny<ICustomHeader>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Save_GivenValidModel_ThenCallsServiceSaveAsync()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            HeaderName = "X-Test",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "value"
        };

        // Act
        await _controller.Save(model);

        // Assert
        _mockService.Verify(x => x.SaveAsync(model, "test-user"), Times.Once);
    }

    [Test]
    public async Task Save_GivenValidModel_ThenReturnsOk()
    {
        // Act
        var response = await _controller.Save(new SaveCustomHeaderModel { HeaderName = "X-Test", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value" });

        // Assert
        Assert.That(response, Is.AssignableTo<OkResult>());
    }

    [Test]
    public void Save_GivenServiceThrowsException_ThenRethrows()
    {
        // Arrange
        _mockService.Setup(x => x.SaveAsync(It.IsAny<ICustomHeader>(), It.IsAny<string>())).ThrowsAsync(new InvalidOperationException("Test error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => await _controller.Save(new SaveCustomHeaderModel { HeaderName = "X-Test", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value" }));
    }

    [Test]
    public async Task Delete_ThenCallsServiceDeleteAsync()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        await _controller.Delete(id);

        // Assert
        _mockService.Verify(x => x.DeleteAsync(id), Times.Once);
    }

    [Test]
    public async Task Delete_ThenReturnsOk()
    {
        // Act
        var response = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.That(response, Is.AssignableTo<OkResult>());
    }

    [Test]
    public void Delete_GivenServiceThrowsException_ThenRethrows()
    {
        // Arrange
        _mockService.Setup(x => x.DeleteAsync(It.IsAny<Guid>())).ThrowsAsync(new InvalidOperationException("Test error"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => await _controller.Delete(Guid.NewGuid()));
    }
}
