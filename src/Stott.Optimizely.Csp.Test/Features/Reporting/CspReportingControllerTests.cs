using System;

using Microsoft.AspNetCore.Mvc;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Features.Reporting;
using Stott.Optimizely.Csp.Features.Reporting.Repository;

namespace Stott.Optimizely.Csp.Test.Features.Reporting
{
    [TestFixture]
    public class CspReportingControllerTests
    {
        private Mock<ICspViolationReportRepository> _mockRepository;

        private CspReportingController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ICspViolationReportRepository>();

            _controller = new CspReportingController(_mockRepository.Object);
        }

        [Test]
        public void Report_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
        {
            // Arrange
            var saveModel = new ReportModel();

            _mockRepository.Setup(x => x.Save(It.IsAny<ReportModel>()))
                           .Throws(new Exception(string.Empty));

            // Assert
            Assert.Throws<Exception>(() => _controller.Report(saveModel));
        }

        [Test]
        public void Report_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Arrange
            var saveModel = new ReportModel();

            // Act
            var response = _controller.Report(saveModel);

            // Assert
            Assert.That(response, Is.AssignableFrom<OkResult>());
        }

        [Test]
        public void ReportSummary_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
        {
            // Arrange
            _mockRepository.Setup(x => x.GetReport(It.IsAny<DateTime>()))
                           .Throws(new Exception(string.Empty));

            // Assert
            Assert.Throws<Exception>(() => _controller.ReportSummary());
        }

        [Test]
        public void ReportSummary_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Act
            var response = _controller.ReportSummary();

            // Assert
            Assert.That(response, Is.AssignableFrom<ContentResult>());
            Assert.That((response as ContentResult).StatusCode, Is.EqualTo(200));
        }
    }
}
