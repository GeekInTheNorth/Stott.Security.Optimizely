using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Moq;

using NUnit.Framework;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Reporting;
using Stott.Security.Core.Features.Reporting.Repository;

namespace Stott.Security.Core.Test.Features.Reporting.Repository
{
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
        public async Task SaveAsync_GivenANullViolationReport_ThenNoAttemptIsMadeToSaveARecord()
        {
            // Arrange
            var mockDatabase = new Mock<ICspDataContext>();
            _repository = new CspViolationReportRepository(mockDatabase.Object);

            // Act
            await _repository.SaveAsync(null);

            // Assert
            mockDatabase.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task SaveAsync_GivenAPopulatedViolationReport_ThenANewReportSummaryWithMatchingValueShouldBeSaved()
        {
            // Arrange
            var reportModel = new ReportModel
            {
                BlockedUri = "https://www.example.com/some-part/?someQuery=true",
                Disposition = "a-disposition",
                DocumentUri = "https://www.google.com",
                EffectiveDirective = CspConstants.Directives.ScriptSource,
                OriginalPolicy = "original policy",
                Referrer = CspConstants.HeaderNames.ReferrerPolicy,
                ScriptSample = "script sample",
                SourceFile = "source file",
                ViolatedDirective = CspConstants.Directives.ScriptSourceElement
            };

            _inMemoryDatabase.SetExecuteSqlAsyncResult(0);

            // Act
            var originalCount = await _inMemoryDatabase.CspViolations.AsQueryable().CountAsync();

            await _repository.SaveAsync(reportModel);

            var updatedCount = await _inMemoryDatabase.CspViolations.AsQueryable().CountAsync();
            var createdRecord = await _inMemoryDatabase.CspViolations.AsQueryable().FirstOrDefaultAsync();

            // Assert
            Assert.That(updatedCount, Is.GreaterThan(originalCount));
            Assert.That(createdRecord, Is.Not.Null);
            Assert.That(createdRecord.LastReported, Is.EqualTo(DateTime.UtcNow).Within(5).Seconds);
            Assert.That(createdRecord.BlockedUri, Is.EqualTo("https://www.example.com/some-part/"));
            Assert.That(createdRecord.ViolatedDirective, Is.EqualTo(reportModel.ViolatedDirective));
            Assert.That(createdRecord.Instances, Is.EqualTo(1));
        }

        [Test]
        public async Task SaveAsync_GivenARecordExists_ThenANewRecordIsNotCreated()
        {
            // Arrange
            var reportModel = new ReportModel
            {
                BlockedUri = "https://www.example.com/some-part/?someQuery=true",
                ViolatedDirective = CspConstants.Directives.ScriptSourceElement
            };

            _inMemoryDatabase.SetExecuteSqlAsyncResult(1);

            // Act
            var originalCount = await _inMemoryDatabase.CspViolations.AsQueryable().CountAsync();

            await _repository.SaveAsync(reportModel);

            var updatedCount = await _inMemoryDatabase.CspViolations.AsQueryable().CountAsync();

            // Assert
            Assert.That(updatedCount, Is.EqualTo(originalCount));
        }

        [Test]
        public async Task SaveAsync_CorrectlySeparatesUrlAndQuery()
        {
            // Arrange
            var reportModel = new ReportModel
            {
                BlockedUri = "https://www.example.com/segment-one/?query=one",
                Disposition = "a-disposition",
                DocumentUri = "https://www.google.com",
                EffectiveDirective = CspConstants.Directives.ScriptSource,
                OriginalPolicy = "original policy",
                Referrer = CspConstants.HeaderNames.ReferrerPolicy,
                ScriptSample = "script sample",
                SourceFile = "source file",
                ViolatedDirective = CspConstants.Directives.ScriptSourceElement
            };

            _inMemoryDatabase.SetExecuteSqlAsyncResult(0);

            // Act
            await _repository.SaveAsync(reportModel);

            // Assert
            var createdRecord = await _inMemoryDatabase.CspViolations.AsQueryable().FirstOrDefaultAsync();
            Assert.That(createdRecord, Is.Not.Null);
            Assert.That(createdRecord.BlockedUri, Is.EqualTo("https://www.example.com/segment-one/"));
        }
    }
}
