using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Reporting;
using Stott.Optimizely.Csp.Features.Reporting.Repository;

namespace Stott.Optimizely.Csp.Test.Features.Reporting.Repository
{
    [TestFixture]
    public class CspViolationReportRepositoryTests
    {
        private Mock<ICspDataContext> _mockContext;

        private Mock<DbSet<CspViolationSummary>> _mockDbSet;

        private CspViolationReportRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<ICspDataContext>();
            _mockDbSet = DbSetMocker.GetQueryableMockDbSet<CspViolationSummary>();
            _mockContext.Setup(x => x.CspViolations).Returns(_mockDbSet.Object);

            _repository = new CspViolationReportRepository(_mockContext.Object);
        }

        [Test]
        public async Task SaveAsync_GivenANullViolationReport_ThenNoAttemptIsMadeToSaveARecord()
        {
            // Act
            await _repository.SaveAsync(null);

            // Assert
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
        /*
        [Test]
        public async Task SaveAsync_GivenAPopulatedViolationReport_ThenANewReportSummaryWithMatchingValueShouldBeSaved()
        {
            // Arrange
            var reportModel = new ReportModel
            {
                BlockedUri = "https://www.example.com",
                Disposition = "a-disposition",
                DocumentUri = "https://www.google.com",
                EffectiveDirective = CspConstants.Directives.ScriptSource,
                OriginalPolicy = "original policy",
                Referrer = CspConstants.HeaderNames.ReferrerPolicy,
                ScriptSample = "script sample",
                SourceFile = "source file",
                ViolatedDirective = CspConstants.Directives.ScriptSourceElement
            };

            CspViolationSummary savedRecord = null;
            _mockDbSet.Setup(x => x.Add(It.IsAny<CspViolationSummary>()))
                      .Callback<CspViolationSummary>(x => savedRecord = x);

            // Act
            await _repository.SaveAsync(reportModel);

            // Assert
            Assert.That(savedRecord, Is.Not.Null);
            Assert.That(savedRecord.LastReported, Is.EqualTo(DateTime.Now).Within(5).Seconds);
            Assert.That(savedRecord.BlockedUri, Is.EqualTo(reportModel.BlockedUri));
            Assert.That(savedRecord.Disposition, Is.EqualTo(reportModel.Disposition));
            Assert.That(savedRecord.DocumentUri, Is.EqualTo(reportModel.DocumentUri));
            Assert.That(savedRecord.EffectiveDirective, Is.EqualTo(reportModel.EffectiveDirective));
            Assert.That(savedRecord.OriginalPolicy, Is.EqualTo(reportModel.OriginalPolicy));
            Assert.That(savedRecord.Referrer, Is.EqualTo(reportModel.Referrer));
            Assert.That(savedRecord.ScriptSample, Is.EqualTo(reportModel.ScriptSample));
            Assert.That(savedRecord.SourceFile, Is.EqualTo(reportModel.SourceFile));
            Assert.That(savedRecord.ViolatedDirective, Is.EqualTo(reportModel.ViolatedDirective));
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

            CspViolationSummary savedRecord = null;
            _mockDbSet.Setup(x => x.Add(It.IsAny<CspViolationSummary>()))
                      .Callback<CspViolationSummary>(x => savedRecord = x);

            // Act
            await _repository.SaveAsync(reportModel);

            // Assert
            Assert.That(savedRecord, Is.Not.Null);
            Assert.That(savedRecord.BlockedUri, Is.EqualTo("https://www.example.com/segment-one/"));
            Assert.That(savedRecord.BlockedQueryString, Is.EqualTo("?query=one"));
        }
        */
    }
}
