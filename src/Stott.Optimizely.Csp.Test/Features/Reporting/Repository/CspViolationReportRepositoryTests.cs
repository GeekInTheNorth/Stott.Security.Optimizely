using System;
using System.Collections.Generic;

using EPiServer.Data.Dynamic;

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
        private Mock<DynamicDataStoreFactory> _mockDynamicDataStoreFactory;

        private Mock<DynamicDataStore> _mockDynamicDataStore;

        private Mock<StoreDefinition> _mockStoreDefinition;

        private CspViolationReportRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockStoreDefinition = new Mock<StoreDefinition>(
                MockBehavior.Loose,
                string.Empty,
                new List<PropertyMap>(0),
                null);

            _mockDynamicDataStore = new Mock<DynamicDataStore>(
                MockBehavior.Loose,
                _mockStoreDefinition.Object);

            _mockDynamicDataStoreFactory = new Mock<DynamicDataStoreFactory>();
            _mockDynamicDataStoreFactory.Setup(x => x.CreateStore(typeof(CspViolationReport))).Returns(_mockDynamicDataStore.Object);

            _repository = new CspViolationReportRepository(_mockDynamicDataStoreFactory.Object);
        }

        [Test]
        public void Save_GivenANullViolationReport_ThenNoAttemptIsMadeToSaveARecord()
        {
            // Act
            _repository.Save(null);

            // Assert
            _mockDynamicDataStore.Verify(x => x.Save(It.IsAny<object>()), Times.Never);
        }

        [Test]
        public void Save_GivenAPopulatedViolationReport_ThenANewReportSummaryWithMatchingValueShouldBeSaved()
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

            CspViolationReport savedRecord = null;
            _mockDynamicDataStore.Setup(x => x.Save(It.IsAny<CspViolationReport>()))
                                 .Callback<object>(x => savedRecord = x as CspViolationReport);

            // Act
            _repository.Save(reportModel);

            // Assert
            Assert.That(savedRecord, Is.Not.Null);
            Assert.That(savedRecord.Reported, Is.EqualTo(DateTime.Now).Within(5).Seconds);
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
    }
}
