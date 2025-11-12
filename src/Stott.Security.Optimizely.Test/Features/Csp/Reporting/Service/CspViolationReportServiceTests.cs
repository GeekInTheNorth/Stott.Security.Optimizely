namespace Stott.Security.Optimizely.Test.Features.Csp.Reporting.Service;

using System;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Reporting.Models;
using Stott.Security.Optimizely.Features.Csp.Reporting.Repository;
using Stott.Security.Optimizely.Features.Csp.Reporting.Service;

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
        string source,
        string directive)
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
        var mockModel = new Mock<ICspReport>();
        mockModel.Setup(x => x.BlockedUri).Returns(originalBlockedUri);
        mockModel.Setup(x => x.ViolatedDirective).Returns(CspConstants.Directives.DefaultSource);

        // Act
        await _service.SaveAsync(mockModel.Object);

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
        var mockModel = new Mock<ICspReport>();
        mockModel.Setup(x => x.BlockedUri).Returns(blockedUri);
        mockModel.Setup(x => x.ViolatedDirective).Returns(violatedDirective);

        // Act
        await _service.SaveAsync(mockModel.Object);

        // Assert
        _mockRepository.Verify(x => x.SaveAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(saveAttempts));
    }
}