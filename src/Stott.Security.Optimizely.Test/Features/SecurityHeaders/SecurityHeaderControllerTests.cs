namespace Stott.Security.Optimizely.Test.Features.SecurityHeaders;

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.SecurityHeaders;
using Stott.Security.Optimizely.Features.SecurityHeaders.Service;

[TestFixture]
public class SecurityHeaderControllerTests
{
    private Mock<ISecurityHeaderService> _mockService;

    private Mock<ILogger<SecurityHeaderController>> _mockLogger;

    private SecurityHeaderController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ISecurityHeaderService>();

        _mockLogger = new Mock<ILogger<SecurityHeaderController>>();

        _controller = new SecurityHeaderController(_mockService.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Get_CallsGetFromTheService()
    {
        // Arrange
        _mockService.Setup(x => x.GetAsync())
                    .ReturnsAsync(new SecurityHeaderSettings());

        // Act
        await _controller.Get();

        // Assert
        _mockService.Verify(x => x.GetAsync(), Times.Once());
    }

    [Test]
    public void Get_ReturnsErrorWhenServiceThrowsAnException()
    {
        // Arrange
        _mockService.Setup(x => x.GetAsync())
                    .ThrowsAsync(new Exception(string.Empty));

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.Get());
    }

    [Test]
    public async Task Get_ReturnsSuccessResponseWhenServiceReturnsData()
    {
        // Arrange
        _mockService.Setup(x => x.GetAsync())
                    .ReturnsAsync(new SecurityHeaderSettings());

        // Act
        var response = await _controller.Get();

        // Assert
        Assert.That(response, Is.AssignableFrom<ContentResult>());
        Assert.That((response as ContentResult).StatusCode, Is.EqualTo(200));
    }
}
