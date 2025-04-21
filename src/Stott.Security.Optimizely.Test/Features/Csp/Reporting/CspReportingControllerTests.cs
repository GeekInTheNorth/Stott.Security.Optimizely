namespace Stott.Security.Optimizely.Test.Features.Csp.Reporting;

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
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.AllowList;
using Stott.Security.Optimizely.Features.Csp.Reporting;
using Stott.Security.Optimizely.Features.Csp.Reporting.Models;
using Stott.Security.Optimizely.Features.Csp.Reporting.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;

[TestFixture]
public class CspReportingControllerTests
{
    private Mock<ICspViolationReportService> _mockReportService;

    private Mock<IAllowListService> _mockAllowListService;

    private Mock<ICspSettingsService> _mockSettingsService;

    private Mock<ILogger<CspReportingController>> _mockLogger;

    private Mock<HttpContext> _mockContext;

    private Mock<HttpRequest> _mockRequest;

    private HeaderDictionary _headers;

    private CspReportingController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockReportService = new Mock<ICspViolationReportService>();

        _mockAllowListService = new Mock<IAllowListService>();

        _mockSettingsService = new Mock<ICspSettingsService>();
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsEnabled = true, UseInternalReporting = true });

        _mockLogger = new Mock<ILogger<CspReportingController>>();

        _headers = new HeaderDictionary();

        _mockRequest = new Mock<HttpRequest>();
        _mockRequest.Setup(x => x.Headers).Returns(_headers);

        _mockContext = new Mock<HttpContext>();
        _mockContext.Setup(x => x.Request).Returns(_mockRequest.Object);

        _controller = new CspReportingController(
            _mockReportService.Object,
            _mockAllowListService.Object,
            _mockSettingsService.Object,
            _mockLogger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _mockContext.Object
            }
        };
    }

    [Test]
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    public async Task ReportToViolation_WhenReportingIsNotEnabled_ReturnsAnOkObjectResultWithoutSaving(bool isEnabled, bool useInternalReporting)
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync())
                            .ReturnsAsync(new CspSettings { IsEnabled = isEnabled, UseInternalReporting = useInternalReporting });

        // Act
        var result = await _controller.ReportToViolation();

        // Assert
        Assert.That(result, Is.AssignableTo<OkObjectResult>());

        _mockReportService.Verify(x => x.SaveAsync(It.IsAny<ICspReport>()), Times.Never());
    }

    [Test]
    public void ReportToViolation_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
    {
        // Arrange
        var saveModel = new List<ReportToWrapper>
        {
            new() { CspReport = new ReportToBody() }
        };
        var saveModelJson = JsonConvert.SerializeObject(saveModel);
        var testStream = new MemoryStream(Encoding.UTF8.GetBytes(saveModelJson));

        _mockRequest.Setup(x => x.Body).Returns(testStream);

        _mockReportService.Setup(x => x.SaveAsync(It.IsAny<ICspReport>()))
                          .ThrowsAsync(new Exception(string.Empty));
        
        _headers.Add("Content-Type", "application/reports+json");

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

        _headers.Add("Content-Type", "application/reports+json");

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

        _headers.Add("Content-Type", "application/reports+json");

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

        _headers.Add("Content-Type", "application/reports+json");

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

        _headers.Add("Content-Type", "application/reports+json");

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