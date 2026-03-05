namespace Stott.Security.Optimizely.Test.Features.Audit;

using System;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Audit;
using Stott.Security.Optimizely.Features.Configuration;

[TestFixture]
public class AuditCleanupScheduledJobTests
{
    private Mock<IAuditRepository> _mockRepository;

    private SecurityConfiguration _configuration;

    private AuditCleanupScheduledJob _scheduledJob;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IAuditRepository>();
        _configuration = new SecurityConfiguration
        {
            AuditRetentionPeriod = TimeSpan.FromDays(730) // 2 years
        };
        _scheduledJob = new AuditCleanupScheduledJob(_mockRepository.Object, _configuration);
    }

    [Test]
    public void Execute_GivenRecordsWereDeleted_ThenReturnsSuccessMessage()
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                       .Returns(Task.FromResult(42));

        // Act
        var result = _scheduledJob.Execute();

        // Assert
        Assert.That(result, Does.Contain("42 Audit Record(s)"));
        Assert.That(result, Does.Contain("730 days"));
    }

    [Test]
    public void Execute_GivenNoRecordsWereDeleted_ThenReturnsSuccessMessageWithZero()
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                       .Returns(Task.FromResult(0));

        // Act
        var result = _scheduledJob.Execute();

        // Assert
        Assert.That(result, Does.Contain("0 Audit Record(s)"));
    }

    [Test]
    public void Execute_GivenCustomRetentionPeriod_ThenUsesCustomPeriod()
    {
        // Arrange
        _configuration.AuditRetentionPeriod = TimeSpan.FromDays(90); // 90 days
        _scheduledJob = new AuditCleanupScheduledJob(_mockRepository.Object, _configuration);

        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                       .Returns(Task.FromResult(10));

        // Act
        var result = _scheduledJob.Execute();

        // Assert
        Assert.That(result, Does.Contain("10 Audit Record(s)"));
        Assert.That(result, Does.Contain("90 days"));
    }

    [Test]
    public void Execute_GivenRepositoryThrowsException_ThenReturnsErrorMessage()
    {
        // Arrange
        var exceptionMessage = "Database connection failed";
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                       .Throws(new Exception(exceptionMessage));

        // Act
        var result = _scheduledJob.Execute();

        // Assert
        Assert.That(result, Does.Contain("An error was encountered"));
        Assert.That(result, Does.Contain(exceptionMessage));
    }

    [Test]
    public void Execute_GivenDefaultOptions_ThenPassesCorrectThresholdToRepository()
    {
        // Arrange
        DateTime thresholdPassed = DateTime.MinValue;
        int batchSizePassed = 0;
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                       .Callback<DateTime, int>((threshold, batchSize) =>
                       {
                           thresholdPassed = threshold;
                           batchSizePassed = batchSize;
                       })
                       .Returns(Task.FromResult(5));

        var expectedThreshold = DateTime.Today.AddDays(-730); // 2 years

        // Act
        _scheduledJob.Execute();

        // Assert
        Assert.That(thresholdPassed.Date, Is.EqualTo(expectedThreshold.Date));
    }

    [Test]
    [TestCase(30)]
    [TestCase(90)]
    [TestCase(365)]
    [TestCase(730)]
    [TestCase(1095)]
    public void Execute_GivenVariousRetentionPeriods_ThenPassesCorrectThresholdToRepository(int days)
    {
        // Arrange
        _configuration.AuditRetentionPeriod = TimeSpan.FromDays(days);
        _scheduledJob = new AuditCleanupScheduledJob(_mockRepository.Object, _configuration);

        DateTime thresholdPassed = DateTime.MinValue;
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                       .Callback<DateTime, int>((threshold, batchSize) => thresholdPassed = threshold)
                       .Returns(Task.FromResult(0));

        var expectedThreshold = DateTime.Today.AddDays(-days);

        // Act
        _scheduledJob.Execute();

        // Assert
        Assert.That(thresholdPassed.Date, Is.EqualTo(expectedThreshold.Date));
    }

    [Test]
    public void Execute_GivenBatchSizeLimit_ThenPassesBatchSizeToRepository()
    {
        // Arrange
        int batchSizePassed = 0;
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                       .Callback<DateTime, int>((threshold, batchSize) => batchSizePassed = batchSize)
                       .Returns(Task.FromResult(100));

        // Act
        _scheduledJob.Execute();

        // Assert
        Assert.That(batchSizePassed, Is.EqualTo(CspConstants.AuditDeletionBatchSize));
    }

    [Test]
    public void Execute_GivenDeletedRecordsEqualsBatchSize_ThenIncludesAdditionalRecordsMessage()
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                       .Returns(Task.FromResult(1000));

        // Act
        var result = _scheduledJob.Execute();

        // Assert
        Assert.That(result, Does.Contain($"1000 Audit Record(s)"));
        Assert.That(result, Does.Contain("Additional old records may remain"));
    }

    [Test]
    public void Execute_GivenDeletedRecordsLessThanBatchSize_ThenDoesNotIncludeAdditionalRecordsMessage()
    {
        // Arrange
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<DateTime>(), It.IsAny<int>()))
                       .Returns(Task.FromResult(500)); // Less than batch size

        // Act
        var result = _scheduledJob.Execute();

        // Assert
        Assert.That(result, Does.Contain("500 Audit Record(s)"));
        Assert.That(result, Does.Not.Contain("Additional old records may remain"));
    }
}
