using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Features.Reporting;
using Stott.Optimizely.Csp.Features.Reporting.Repository;
using Stott.Optimizely.Csp.Features.Whitelist;

namespace Stott.Optimizely.Csp.Test.Features.Reporting
{
    [TestFixture]
    public class CspReportingControllerTests
    {
        private Mock<ICspViolationReportRepository> _mockRepository;

        private Mock<IWhitelistService> _mockWhitelistService;

        private CspReportingController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ICspViolationReportRepository>();
            _mockWhitelistService = new Mock<IWhitelistService>();

            _controller = new CspReportingController(_mockRepository.Object, _mockWhitelistService.Object);
        }

        [Test]
        public void Report_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
        {
            // Arrange
            var saveModel = new ReportModel();

            _mockRepository.Setup(x => x.SaveAsync(It.IsAny<ReportModel>()))
                           .ThrowsAsync(new Exception(string.Empty));

            // Assert
            Assert.ThrowsAsync<Exception>(() => _controller.Report(saveModel));
        }

        [Test]
        public async Task Report_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Arrange
            var saveModel = new ReportModel();

            // Act
            var response = await _controller.Report(saveModel);

            // Assert
            Assert.That(response, Is.AssignableFrom<OkResult>());
        }

        [Test]
        public void ReportSummary_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
        {
            // Arrange
            _mockRepository.Setup(x => x.GetReportAsync(It.IsAny<DateTime>()))
                           .ThrowsAsync(new Exception(string.Empty));

            // Assert
            Assert.ThrowsAsync<Exception>(() => _controller.ReportSummary());
        }

        [Test]
        public async Task ReportSummary_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Act
            var response = await _controller.ReportSummary();

            // Assert
            Assert.That(response, Is.AssignableFrom<ContentResult>());
            Assert.That((response as ContentResult).StatusCode, Is.EqualTo(200));
        }
    }
}
