namespace Stott.Security.Optimizely.Test.Features.Reporting.Repository;

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Reporting.Repository;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public class CspViolationReportRepositoryTests
{
    private TestDataContext _inMemoryDatabase;

    private CspViolationReportRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _repository = new CspViolationReportRepository(_inMemoryDatabase);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _inMemoryDatabase.Reset();
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task SaveAsync_GivenANullOrEmptyBlockedUri_ThenNoAttemptIsMadeToSaveARecord(string blockedUri)
    {
        // Arrange
        var mockDatabase = new Mock<ICspDataContext>();
        _repository = new CspViolationReportRepository(mockDatabase.Object);

        // Act
        await _repository.SaveAsync(blockedUri, CspConstants.Directives.DefaultSource);

        // Assert
        mockDatabase.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task SaveAsync_GivenANullOrEmptyViolatedDirective_ThenNoAttemptIsMadeToSaveARecord(string violatedDirective)
    {
        // Arrange
        var mockDatabase = new Mock<ICspDataContext>();
        _repository = new CspViolationReportRepository(mockDatabase.Object);

        // Act
        await _repository.SaveAsync(CspConstants.Sources.SchemeData, violatedDirective);

        // Assert
        mockDatabase.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task SaveAsync_GivenAPopulatedViolationReport_ThenANewReportSummaryWithMatchingValueShouldBeSaved()
    {
        // Arrange
        _inMemoryDatabase.SetExecuteSqlAsyncResult(0);

        // Act
        var originalCount = await _inMemoryDatabase.CspViolations.AsQueryable().CountAsync();

        await _repository.SaveAsync("https://www.example.com/some-part/", CspConstants.Directives.ScriptSourceElement);

        var updatedCount = await _inMemoryDatabase.CspViolations.AsQueryable().CountAsync();
        var createdRecord = await _inMemoryDatabase.CspViolations.AsQueryable().FirstOrDefaultAsync();

        // Assert
        Assert.That(updatedCount, Is.GreaterThan(originalCount));
        Assert.That(createdRecord, Is.Not.Null);
        Assert.That(createdRecord.LastReported, Is.EqualTo(DateTime.UtcNow).Within(5).Seconds);
        Assert.That(createdRecord.BlockedUri, Is.EqualTo("https://www.example.com/some-part/"));
        Assert.That(createdRecord.ViolatedDirective, Is.EqualTo(CspConstants.Directives.ScriptSourceElement));
        Assert.That(createdRecord.Instances, Is.EqualTo(1));
    }

    [Test]
    public async Task SaveAsync_GivenARecordExists_ThenANewRecordIsNotCreated()
    {
        // Arrange
        _inMemoryDatabase.SetExecuteSqlAsyncResult(1);

        // Act
        var originalCount = await _inMemoryDatabase.CspViolations.AsQueryable().CountAsync();

        await _repository.SaveAsync("https://www.example.com/some-part/", CspConstants.Directives.ScriptSourceElement);

        var updatedCount = await _inMemoryDatabase.CspViolations.AsQueryable().CountAsync();

        // Assert
        Assert.That(updatedCount, Is.EqualTo(originalCount));
    }
}
