namespace Stott.Security.Optimizely.Test.Features.Reporting.Service;

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

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
    [TestCase(null, null)]
    [TestCase("", null)]
    [TestCase(" ", null)]
    [TestCase("https://www.example.com", null)]
    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase(" ", "")]
    [TestCase("https://www.example.com", "")]
    [TestCase(null, " ")]
    [TestCase("", " ")]
    [TestCase(" ", " ")]
    [TestCase("https://www.example.com", " ")]
    [TestCase(null, CspConstants.Directives.DefaultSource)]
    [TestCase("", CspConstants.Directives.DefaultSource)]
    [TestCase(" ", CspConstants.Directives.DefaultSource)]
    [TestCase("https://www.example.com", CspConstants.Directives.DefaultSource)]
    public async Task GetReportAsync_CallsGetReportAsyncOnTheRepository(
        [CanBeNull] string source,
        [CanBeNull] string directive)
    {
        // Act
        await _service.GetReportAsync(source, directive, DateTime.UtcNow);

        // Assert
        _mockRepository.Verify(x => x.GetReportAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
        _mockRepository.Verify(x => x.GetReportAsync(source, directive, It.IsAny<DateTime>()), Times.Once);
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