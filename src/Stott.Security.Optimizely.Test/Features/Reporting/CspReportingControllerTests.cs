//// namespace Stott.Security.Optimizely.Test.Features.Reporting;
//// 
//// using System;
//// using System.Threading.Tasks;
//// 
//// using Microsoft.AspNetCore.Mvc;
//// using Microsoft.Extensions.Logging;
//// 
//// using Moq;
//// 
//// using NUnit.Framework;
//// 
//// using Stott.Security.Optimizely.Features.Reporting;
//// using Stott.Security.Optimizely.Features.Reporting.Service;
//// using Stott.Security.Optimizely.Features.AllowList;
//// 
//// [TestFixture]
//// public class CspReportingControllerTests
//// {
////     private Mock<ICspViolationReportService> _mockReportService;
//// 
////     private Mock<IAllowListService> _mockAllowListService;
//// 
////     private Mock<ILogger<CspReportingController>> _mockLogger;
//// 
////     private CspReportingController _controller;
//// 
////     [SetUp]
////     public void SetUp()
////     {
////         _mockReportService = new Mock<ICspViolationReportService>();
////         _mockAllowListService = new Mock<IAllowListService>();
////         _mockLogger = new Mock<ILogger<CspReportingController>>();
//// 
////         _controller = new CspReportingController(
////             _mockReportService.Object,
////             _mockAllowListService.Object,
////             _mockLogger.Object);
////     }
//// 
////     [Test]
////     public void Report_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
////     {
////         // Arrange
////         var saveModel = new ReportModel();
//// 
////         _mockReportService.Setup(x => x.SaveAsync(It.IsAny<ReportModel>()))
////                           .ThrowsAsync(new Exception(string.Empty));
//// 
////         // Assert
////         Assert.ThrowsAsync<Exception>(() => _controller.Report(saveModel));
////     }
//// 
////     [Test]
////     public async Task Report_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
////     {
////         // Arrange
////         var saveModel = new ReportModel();
//// 
////         // Act
////         var response = await _controller.Report(saveModel);
//// 
////         // Assert
////         Assert.That(response, Is.AssignableFrom<OkResult>());
////     }
//// 
////     [Test]
////     [TestCase(true, 1)]
////     [TestCase(false, 0)]
////     public async Task Report_AddsViolationToTheCspWhenItIsOnTheAllowList(bool isOnAllowList, int expectedUpdatesToCsp)
////     {
////         // Arrange
////         var saveModel = new ReportModel();
//// 
////         _mockAllowListService.Setup(x => x.IsOnAllowListAsync(It.IsAny<string>(), It.IsAny<string>()))
////                              .ReturnsAsync(isOnAllowList);
//// 
////         // Act
////         await _controller.Report(saveModel);
//// 
////         // Assert
////         _mockAllowListService.Verify(x => x.AddFromAllowListToCspAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(expectedUpdatesToCsp));
////     }
//// 
////     [Test]
////     public void ReportSummary_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
////     {
////         // Arrange
////         _mockReportService.Setup(x => x.GetReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
////                           .ThrowsAsync(new Exception(string.Empty));
//// 
////         // Assert
////         Assert.ThrowsAsync<Exception>(() => _controller.ReportSummary(string.Empty, string.Empty));
////     }
//// 
////     [Test]
////     public async Task ReportSummary_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
////     {
////         // Act
////         var response = await _controller.ReportSummary(string.Empty, string.Empty);
//// 
////         // Assert
////         Assert.That(response, Is.AssignableFrom<ContentResult>());
////         Assert.That((response as ContentResult).StatusCode, Is.EqualTo(200));
////     }
//// }
