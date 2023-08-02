namespace Stott.Security.Optimizely.Test.Features.Cors;

using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Cors.Repository;

public sealed class CorsConfigurationControllerTests
{
    private Mock<ICorsSettingsRepository> _mockRepository;

    private Mock<ILogger<CorsConfigurationController>> _mockLogger;

    private CorsConfigurationController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICorsSettingsRepository>();

        _mockLogger = new Mock<ILogger<CorsConfigurationController>>();

        _controller = new CorsConfigurationController(_mockRepository.Object, _mockLogger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            }
        };
    }

    [Test]
    public async Task Save_GivenAnInvalidModel_ThenAnErrorResponseWillBeReturned()
    {
        // Arrange
        _controller.ModelState.TryAddModelError("AnError", "Has happened");

        // Act
        var response = await _controller.Save(new CorsConfiguration()) as ContentResult;

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Save_GivenAnInvalidModel_ThenAnSaveWillNoBeCalledOnTheRepository()
    {
        // Arrange
        _controller.ModelState.TryAddModelError("AnError", "Has happened");

        // Act
        _ = await _controller.Save(new CorsConfiguration());

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<CorsConfiguration>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Save_GivenAnValidModel_ThenAnSaveWillBeCalledOnTheRepository()
    {
        // Act
        _ = await _controller.Save(new CorsConfiguration());

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<CorsConfiguration>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task Save_GivenAnValidModel_ThenAnOkResultWillBeReturned()
    {
        // Act
        var response = await _controller.Save(new CorsConfiguration());

        // Assert
        Assert.That(response, Is.AssignableTo<OkResult>());
    }
}