namespace Stott.Security.Optimizely.Test.Features.Reporting.Repository;

using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.Data.SqlClient;
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

    private Lazy<ICspDataContext> _lazyInMemoryDatabase;

    private CspViolationReportRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _lazyInMemoryDatabase = new Lazy<ICspDataContext>(() => _inMemoryDatabase);

        _repository = new CspViolationReportRepository(_lazyInMemoryDatabase);
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
        var lazyDatabase = new Lazy<ICspDataContext>(() => mockDatabase.Object);

        _repository = new CspViolationReportRepository(lazyDatabase);

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
        var lazyDatabase = new Lazy<ICspDataContext>(() => mockDatabase.Object);

        _repository = new CspViolationReportRepository(lazyDatabase);

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

    [Test]
    public async Task GetReportAsync_CorrectlyGroupsReportsByMatchingBlockedUriAndViolatedDirective()
    {
        // Arrange
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = "https://www.example.com", ViolatedDirective = CspConstants.Directives.ScriptSource, Instances = 4, LastReported = new DateTime(2022, 2, 1) });
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = "https://www.example.com", ViolatedDirective = CspConstants.Directives.ScriptSource, Instances = 5, LastReported = new DateTime(2022, 1, 1) });
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = CspConstants.Sources.Self, ViolatedDirective = CspConstants.Directives.StyleSource, Instances = 6, LastReported = new DateTime(2022, 3, 10) });
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = CspConstants.Sources.Self, ViolatedDirective = CspConstants.Directives.StyleSource, Instances = 7, LastReported = new DateTime(2022, 3, 11) });
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = CspConstants.Sources.Self, ViolatedDirective = CspConstants.Directives.ImageSource, Instances = 8, LastReported = new DateTime(2022, 4, 20) });
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = CspConstants.Sources.Self, ViolatedDirective = CspConstants.Directives.ImageSource, Instances = 9, LastReported = new DateTime(2022, 4, 21) });
        _inMemoryDatabase.SaveChanges();

        // Act
        var report = await _repository.GetReportAsync(string.Empty, string.Empty, DateTime.MinValue);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(report, Is.Not.Null);
            Assert.That(report, Has.Count.EqualTo(3));
            Assert.That(report[0].Source, Is.EqualTo(CspConstants.Sources.Self));
            Assert.That(report[0].Directive, Is.EqualTo(CspConstants.Directives.ImageSource));
            Assert.That(report[0].Violations, Is.EqualTo(17));
            Assert.That(report[0].LastViolated, Is.EqualTo(new DateTime(2022, 4, 21)));
            Assert.That(report[1].Source, Is.EqualTo(CspConstants.Sources.Self));
            Assert.That(report[1].Directive, Is.EqualTo(CspConstants.Directives.StyleSource));
            Assert.That(report[1].Violations, Is.EqualTo(13));
            Assert.That(report[1].LastViolated, Is.EqualTo(new DateTime(2022, 3, 11)));
            Assert.That(report[2].Source, Is.EqualTo("https://www.example.com"));
            Assert.That(report[2].Directive, Is.EqualTo(CspConstants.Directives.ScriptSource));
            Assert.That(report[2].Violations, Is.EqualTo(9));
            Assert.That(report[2].LastViolated, Is.EqualTo(new DateTime(2022, 2, 1)));
        });
    }

    [Test]
    [TestCase(null, null, 4)]
    [TestCase("", null, 4)]
    [TestCase(" ", null, 4)]
    [TestCase("google", null, 1)]
    [TestCase("www", null, 2)]
    [TestCase(CspConstants.Sources.Self, null, 1)]
    [TestCase(null, CspConstants.Directives.ScriptSource, 2)]
    [TestCase(null, CspConstants.Directives.StyleSource, 1)]
    [TestCase("www", CspConstants.Directives.ScriptSource, 2)]
    [TestCase("www", CspConstants.Directives.StyleSource, 0)]
    public async Task GetReportAsync_CorrectlyFiltersBySourceAndDirective(
        [CanBeNull] string source,
        [CanBeNull] string directive,
        int expectedRecords)
    {
        // Arrange
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = "https://www.google.com", ViolatedDirective = CspConstants.Directives.ScriptSource, Instances = 4, LastReported = new DateTime(2022, 2, 1) });
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = "https://www.example.com", ViolatedDirective = CspConstants.Directives.ScriptSource, Instances = 4, LastReported = new DateTime(2022, 2, 1) });
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = CspConstants.Sources.Self, ViolatedDirective = CspConstants.Directives.StyleSource, Instances = 6, LastReported = new DateTime(2022, 3, 10) });
        _inMemoryDatabase.CspViolations.Add(new CspViolationSummary { BlockedUri = CspConstants.Sources.None, ViolatedDirective = CspConstants.Directives.ImageSource, Instances = 8, LastReported = new DateTime(2022, 4, 20) });
        _inMemoryDatabase.SaveChanges();

        // Act
        var report = await _repository.GetReportAsync(source, directive, DateTime.MinValue);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(report, Is.Not.Null);
            Assert.That(report, Has.Count.EqualTo(expectedRecords));
        });
    }

    [Test]
    public async Task DeleteAsync_CorrectlyPassesThresholdIntoTheReportCleanupJob()
    {
        // Arrange
        var mockDatabase = new Mock<ICspDataContext>();
        var lazyDatabase = new Lazy<ICspDataContext>(() => mockDatabase.Object);

        _repository = new CspViolationReportRepository(lazyDatabase);

        SqlParameter[] parametersUsed = null;
        mockDatabase.Setup(x => x.ExecuteSqlAsync(It.IsAny<string>(), It.IsAny<SqlParameter[]>()))
                    .Callback<string, SqlParameter[]>((_, x) => parametersUsed = x);

        var datePassed = DateTime.UtcNow;

        // Act
        await _repository.DeleteAsync(datePassed);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(parametersUsed, Is.Not.Null);
            Assert.That(parametersUsed.Length, Is.EqualTo(1));
            Assert.That(parametersUsed[0].ParameterName, Is.EqualTo("@threshold"));
            Assert.That(parametersUsed[0].Value, Is.EqualTo(datePassed));
        });
    }
}
