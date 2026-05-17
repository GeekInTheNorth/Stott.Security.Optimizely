namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities.Exceptions;
using Stott.Security.Optimizely.Features.Csp.Permissions;
using Stott.Security.Optimizely.Features.Csp.Permissions.List;
using Stott.Security.Optimizely.Features.Csp.Permissions.Save;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;

[TestFixture]
public sealed class CspPermissionsControllerTests
{
    private Mock<ICspPermissionsListModelBuilder> _mockViewModelBuilder;

    private Mock<ICspPermissionService> _mockService;

    private Mock<ILogger<CspPermissionsController>> _mockLogger;

    private CspPermissionsController _controller;

    private static readonly Guid PropagationSiteId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    [SetUp]
    public void SetUp()
    {
        _mockViewModelBuilder = new Mock<ICspPermissionsListModelBuilder>();

        _mockService = new Mock<ICspPermissionService>();

        _mockLogger = new Mock<ILogger<CspPermissionsController>>();

        _controller = new CspPermissionsController(
            _mockViewModelBuilder.Object,
            _mockService.Object,
            _mockLogger.Object);

        var claims = new List<Claim>()
        {
            new(ClaimTypes.Name, "test.user"),
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new("name", "test.user"),
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
    public async Task Save_GivenAnInvalidModelState_ThenAnInvalidRequestResponseIsReturned()
    {
        // Arrange
        var saveModel = new SavePermissionModel
        {
            Id = Guid.NewGuid(),
            Source = CspConstants.Sources.Self,
            Directives = CspConstants.AllDirectives
        };

        _controller.ModelState.AddModelError(nameof(SavePermissionModel.Source), "An Error.");

        // Act
        var response = await _controller.Save(saveModel) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task Save_WhenTheCommandThrowsAEntityExistsException_ThenAnInvalidRequestResponseIsReturned()
    {
        // Arrange
        var saveModel = new SavePermissionModel
        {
            Id = Guid.NewGuid(),
            Source = CspConstants.Sources.Self,
            Directives = CspConstants.AllDirectives
        };

        _mockService.Setup(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>()))
                    .Throws(new EntityExistsException(string.Empty));

        // Act
        var response = await _controller.Save(saveModel) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public void Save_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        var saveModel = new SavePermissionModel
        {
            Id = Guid.NewGuid(),
            Source = CspConstants.Sources.Self,
            Directives = CspConstants.AllDirectives
        };

        _mockService.Setup(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>()))
                    .ThrowsAsync(new Exception(string.Empty));

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.Save(saveModel));
    }

    [Test]
    public async Task Save_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
    {
        // Arrange
        var saveModel = new SavePermissionModel
        {
            Id = Guid.NewGuid(),
            Source = CspConstants.Sources.Self,
            Directives = CspConstants.AllDirectives
        };

        // Act
        var response = await _controller.Save(saveModel);

        // Assert
        Assert.That(response, Is.AssignableFrom<OkResult>());
    }

    [Test]
    public async Task Append_GivenAnInvalidModelState_ThenAnInvalidRequestResponseIsReturned()
    {
        // Arrange
        var saveModel = new AppendPermissionModel();
        _controller.ModelState.AddModelError(nameof(SavePermissionModel.Source), "An Error.");

        // Act
        var response = await _controller.Append(saveModel) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public void Append_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        var saveModel = new AppendPermissionModel
        {
            Source = CspConstants.Sources.Self,
            Directive = CspConstants.Directives.DefaultSource
        };

        _mockService.Setup(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>()))
                    .ThrowsAsync(new Exception(string.Empty));

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.Append(saveModel));
    }

    [Test]
    public async Task Append_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
    {
        // Arrange
        var saveModel = new AppendPermissionModel
        {
            Source = CspConstants.Sources.Self,
            Directive = CspConstants.Directives.DefaultSource
        };

        // Act
        var response = await _controller.Append(saveModel);

        // Assert
        Assert.That(response, Is.AssignableFrom<OkResult>());
    }

    [Test]
    public async Task Delete_GivenAnEmptyGuid_ThenABadRequestIsReturned()
    {
        // Act
        var response = await _controller.Delete(Guid.Empty) as ContentResult;

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public void Delete_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        _mockService.Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                    .ThrowsAsync(new Exception(string.Empty));

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.Delete(Guid.NewGuid()));
    }

    [Test]
    public async Task Delete_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
    {
        // Act
        var response = await _controller.Delete(Guid.NewGuid());

        // Assert
        Assert.That(response, Is.AssignableFrom<OkResult>());
    }

    [Test]
    public async Task List_GivenConcreteSiteIdAndDirtyHost_ThenBuilderReceivesTheSanitizedHost()
    {
        // Arrange
        _mockViewModelBuilder.Setup(x => x.WithSourceFilter(It.IsAny<string>())).Returns(_mockViewModelBuilder.Object);
        _mockViewModelBuilder.Setup(x => x.WithDirectiveFilter(It.IsAny<string>())).Returns(_mockViewModelBuilder.Object);
        _mockViewModelBuilder.Setup(x => x.WithSiteId(It.IsAny<Guid?>())).Returns(_mockViewModelBuilder.Object);
        _mockViewModelBuilder.Setup(x => x.WithHostName(It.IsAny<string>())).Returns(_mockViewModelBuilder.Object);
        _mockViewModelBuilder.Setup(x => x.BuildAsync()).ReturnsAsync(new CspPermissionsListModel());

        // Act
        await _controller.List(null, null, PropagationSiteId, "https://example.com/");

        // Assert
        _mockViewModelBuilder.Verify(x => x.WithSiteId(PropagationSiteId), Times.Once);
        _mockViewModelBuilder.Verify(x => x.WithHostName("example.com"), Times.Once);
    }

    [Test]
    public async Task Save_GivenADirtyHostInTheModel_ThenTheServiceReceivesTheSanitizedHost()
    {
        // Arrange
        var saveModel = new SavePermissionModel
        {
            Id = Guid.NewGuid(),
            Source = CspConstants.Sources.Self,
            Directives = CspConstants.AllDirectives,
            SiteId = PropagationSiteId,
            HostName = "https://example.com/"
        };

        // Act
        await _controller.Save(saveModel);

        // Assert
        _mockService.Verify(
            x => x.SaveAsync(saveModel.Id, saveModel.Source, saveModel.Directives, "test.user", PropagationSiteId, "example.com"),
            Times.Once);
    }

    [Test]
    public async Task Append_GivenADirtyHostInTheModel_ThenTheServiceReceivesTheSanitizedHost()
    {
        // Arrange
        var appendModel = new AppendPermissionModel
        {
            Source = CspConstants.Sources.Self,
            Directive = CspConstants.Directives.DefaultSource,
            SiteId = PropagationSiteId,
            HostName = "https://example.com/"
        };

        // Act
        await _controller.Append(appendModel);

        // Assert
        _mockService.Verify(
            x => x.AppendDirectiveAsync(appendModel.Source, appendModel.Directive, "test.user", PropagationSiteId, "example.com"),
            Times.Once);
    }
}
