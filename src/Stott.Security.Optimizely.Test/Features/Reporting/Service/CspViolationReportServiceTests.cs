namespace Stott.Security.Optimizely.Test.Features.Reporting.Service;

using System;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Reporting;
using Stott.Security.Optimizely.Features.Reporting.Repository;
using Stott.Security.Optimizely.Features.Reporting.Service;

[TestFixture]
internal class CspViolationReportServiceTests
{
    private Mock<ICspViolationReportRepository> _mockRepository;

    private CspViolationReportService _service;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICspViolationReportRepository>();

        _service = new CspViolationReportService(_mockRepository.Object);
    }

    [Test]
    public async Task DeleteAsync_CallsDeleteAsyncOnTheRepository()
    {
        // Act
        await _service.DeleteAsync(DateTime.UtcNow);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<DateTime>()), Times.Once);
    }

    [Test]
    public async Task GetReportAsync_CallsGetReportAsyncOnTheRepository()
    {
        // Act
        await _service.GetReportAsync(DateTime.UtcNow);

        // Assert
        _mockRepository.Verify(x => x.GetReportAsync(It.IsAny<DateTime>()), Times.Once);
    }

    [Test]
    [TestCaseSource(typeof(CspViolationReportServiceTestCases), nameof(CspViolationReportServiceTestCases.BlockedUriTransformTestCases))]
    public async Task SaveAsync_CorrectlyConvertsBlockedUriIntoAProtocolOrUrlPathOnly(
        string originalBlockedUri,
        string expectedBlockedUri)
    {
        // Arrange
        var report = new ReportModel
        {
            BlockedUri = originalBlockedUri,
            ViolatedDirective = CspConstants.Directives.DefaultSource
        };

        // Act
        await _service.SaveAsync(report);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockRepository.Verify(x => x.SaveAsync(expectedBlockedUri, It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCaseSource(typeof(CspViolationReportServiceTestCases), nameof(CspViolationReportServiceTestCases.RepositorySaveAttemptsTestCases))]
    public async Task SaveAsync_OnlyAttemptsToSaveWithValidBlockedUriAndDirective(
        string blockedUri,
        string violatedDirective,
        int saveAttempts)
    {
        // Arrange
        var report = new ReportModel
        {
            BlockedUri = blockedUri,
            ViolatedDirective = violatedDirective
        };

        // Act
        await _service.SaveAsync(report);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(saveAttempts));
    }
}