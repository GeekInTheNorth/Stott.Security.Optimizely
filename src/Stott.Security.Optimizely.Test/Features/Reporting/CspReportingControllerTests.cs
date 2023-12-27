namespace Stott.Security.Optimizely.Test.Features.Reporting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.AllowList;
using Stott.Security.Optimizely.Features.Reporting;
using Stott.Security.Optimizely.Features.Reporting.Models;
using Stott.Security.Optimizely.Features.Reporting.Service;

[TestFixture]
public class CspReportingControllerTests
{
    private Mock<ICspViolationReportService> _mockReportService;

    private Mock<IAllowListService> _mockAllowListService;

    private Mock<ILogger<CspReportingController>> _mockLogger;

    private Mock<HttpContext> _mockContext;

    private Mock<HttpRequest> _mockRequest;

    private CspReportingController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockReportService = new Mock<ICspViolationReportService>();

        _mockAllowListService = new Mock<IAllowListService>();

        _mockLogger = new Mock<ILogger<CspReportingController>>();

        _mockRequest = new Mock<HttpRequest>();

        _mockContext = new Mock<HttpContext>();
        _mockContext.Setup(x => x.Request).Returns(_mockRequest.Object);

        _controller = new CspReportingController(
            _mockReportService.Object,
            _mockAllowListService.Object,
            _mockLogger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _mockContext.Object
            }
        };
    }

    [Test]
    public void ReportUriViolation_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        var saveModel = JsonConvert.SerializeObject(new ReportUriWrapper { CspReport = new ReportUriBody() });
        var testStream = new MemoryStream(Encoding.UTF8.GetBytes(saveModel));

        _mockRequest.Setup(x => x.Body).Returns(testStream);
            
        _mockReportService.Setup(x => x.SaveAsync(It.IsAny<ICspReport>()))
                          .ThrowsAsync(new Exception(string.Empty));

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.ReportUriViolation());
    }

    [Test]
    public async Task ReportUriViolation_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
    {
        // Arrange
        var saveModel = JsonConvert.SerializeObject(new ReportUriWrapper { CspReport = new ReportUriBody() });
        var testStream = new MemoryStream(Encoding.UTF8.GetBytes(saveModel));

        _mockRequest.Setup(x => x.Body).Returns(testStream);

        // Act
        var response = await _controller.ReportUriViolation();

        // Assert
        Assert.That(response, Is.AssignableFrom<OkResult>());
    }

    [Test]
    [TestCase(true, 1)]
    [TestCase(false, 0)]
    public async Task ReportUriViolation_AddsViolationToTheCspWhenItIsOnTheAllowList(bool isOnAllowList, int expectedUpdatesToCsp)
    {
        // Arrange
        var saveModel = JsonConvert.SerializeObject(new ReportUriWrapper { CspReport = new ReportUriBody() });
        var testStream = new MemoryStream(Encoding.UTF8.GetBytes(saveModel));

        _mockRequest.Setup(x => x.Body).Returns(testStream);

        _mockAllowListService.Setup(x => x.IsOnAllowListAsync(It.IsAny<string>(), It.IsAny<string>()))
                             .ReturnsAsync(isOnAllowList);

        // Act
        await _controller.ReportUriViolation();

        // Assert
        _mockAllowListService.Verify(x => x.AddFromAllowListToCspAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(expectedUpdatesToCsp));
    }

    [Test]
    public void ReportToViolation_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        var saveModel = new List<ReportToWrapper>
        {
            new ReportToWrapper()
            {
                CspReport = new ReportToBody()
            }
        };
        var saveModelJson = JsonConvert.SerializeObject(saveModel);
        var testStream = new MemoryStream(Encoding.UTF8.GetBytes(saveModelJson));

        _mockRequest.Setup(x => x.Body).Returns(testStream);

        _mockReportService.Setup(x => x.SaveAsync(It.IsAny<ICspReport>()))
                          .ThrowsAsync(new Exception(string.Empty));

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.ReportToViolation());
    }

    [Test]
    public async Task ReportToViolation_WhenTheReportIsAnEmptyCollection_ThenNoAttemptToSaveOrCheckTheViolationIsMade()
    {
        // Arrange
        var saveModel = new List<ReportToWrapper>(0);
        var saveModelJson = JsonConvert.SerializeObject(saveModel);
        var testStream = new MemoryStream(Encoding.UTF8.GetBytes(saveModelJson));

        _mockRequest.Setup(x => x.Body).Returns(testStream);

        // Act
        _ = await _controller.ReportToViolation();

        // Assert
        _mockReportService.Verify(x => x.SaveAsync(It.IsAny<ICspReport>()), Times.Never);
        _mockAllowListService.Verify(x => x.IsOnAllowListAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public async Task ReportToViolation_WhenTheReportIsPopulatedCollection_ThenAnAttemptToSaveOrCheckTheViolationIsMadeForEveryReport(
        int numberOfReports)
    {
        // Arrange
        var saveModel = Enumerable.Range(1, numberOfReports)
                                  .Select(x => new ReportToWrapper
                                  {
                                      CspReport = new ReportToBody
                                      {
                                          BlockedUri = $"https://example.com/{x}/",
                                          EffectiveDirective = CspConstants.Directives.ScriptSource
                                      }
                                  })
                                  .ToList();
        var saveModelJson = JsonConvert.SerializeObject(saveModel);
        var testStream = new MemoryStream(Encoding.UTF8.GetBytes(saveModelJson));

        _mockRequest.Setup(x => x.Body).Returns(testStream);

        // Act
        _ = await _controller.ReportToViolation();

        // Assert
        _mockReportService.Verify(x => x.SaveAsync(It.IsAny<ICspReport>()), Times.Exactly(numberOfReports));
        _mockAllowListService.Verify(x => x.IsOnAllowListAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(numberOfReports));
    }

    [Test]
    public async Task ReportToViolation_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
    {
        // Arrange
        var saveModel = new List<ReportToWrapper>
        {
            new ReportToWrapper()
            {
                CspReport = new ReportToBody()
            }
        };
        var saveModelJson = JsonConvert.SerializeObject(saveModel);
        var testStream = new MemoryStream(Encoding.UTF8.GetBytes(saveModelJson));

        _mockRequest.Setup(x => x.Body).Returns(testStream);

        // Act
        var response = await _controller.ReportToViolation();

        // Assert
        Assert.That(response, Is.AssignableFrom<OkResult>());
    }

    [Test]
    [TestCase(true, 1)]
    [TestCase(false, 0)]
    public async Task ReportToViolation_AddsViolationToTheCspWhenItIsOnTheAllowList(bool isOnAllowList, int expectedUpdatesToCsp)
    {
        // Arrange
        var saveModel = new List<ReportToWrapper>
        {
            new ReportToWrapper()
            {
                CspReport = new ReportToBody()
            }
        };
        var saveModelJson = JsonConvert.SerializeObject(saveModel);
        var testStream = new MemoryStream(Encoding.UTF8.GetBytes(saveModelJson));

        _mockRequest.Setup(x => x.Body).Returns(testStream);

        _mockAllowListService.Setup(x => x.IsOnAllowListAsync(It.IsAny<string>(), It.IsAny<string>()))
                             .ReturnsAsync(isOnAllowList);

        // Act
        await _controller.ReportToViolation();

        // Assert
        _mockAllowListService.Verify(x => x.AddFromAllowListToCspAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(expectedUpdatesToCsp));
    }

    [Test]
    public void ReportSummary_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        _mockReportService.Setup(x => x.GetReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                          .ThrowsAsync(new Exception(string.Empty));

        // Assert
        Assert.ThrowsAsync<Exception>(() => _controller.ReportSummary(string.Empty, string.Empty));
    }

    [Test]
    public async Task ReportSummary_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
    {
        // Act
        var response = await _controller.ReportSummary(string.Empty, string.Empty);

        // Assert
        Assert.That(response, Is.AssignableFrom<ContentResult>());
        Assert.That((response as ContentResult).StatusCode, Is.EqualTo(200));
    }
}