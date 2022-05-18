using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

using Microsoft.EntityFrameworkCore;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Settings.Repository;

namespace Stott.Optimizely.Csp.Test.Features.Settings.Repository
{
    [TestFixture]
    public class CspSettingsRepositoryTests
    {
        private Mock<ICspDataContext> _mockContext;

        private Mock<DbSet<CspSettings>> _mockDbSet;

        private CspSettingsRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<ICspDataContext>();
            _mockDbSet = DbSetMocker.GetQueryableMockDbSet<CspSettings>();
            _mockContext.Setup(x => x.CspSettings).Returns(_mockDbSet.Object);

            _repository = new CspSettingsRepository(_mockContext.Object);
        }

        [Test]
        public async Task GetAsync_GivenThereAreNoSavedSettings_ThenDefaultSettingsShouldBeReturned()
        {
            // Act
            var settings = await _repository.GetAsync();

            // Assert
            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.Id, Is.EqualTo(Guid.Empty));
            Assert.That(settings.IsEnabled, Is.False);
            Assert.That(settings.IsReportOnly, Is.False);
        }

        [Test]
        public async Task GetAsync_GivenThereAreMultipleSavedSettings_ThenThefirstSettingsShouldBeReturned()
        {
            // Arrange
            var settingsOne = new CspSettings { Id = Guid.NewGuid(), IsEnabled = true, IsReportOnly = false };
            var settingsTwo = new CspSettings { Id = Guid.NewGuid(), IsEnabled = false, IsReportOnly = true };

            _mockDbSet = DbSetMocker.GetQueryableMockDbSet(settingsOne, settingsTwo);
            _mockContext.Setup(x => x.CspSettings).Returns(_mockDbSet.Object);

            // Act
            var settings = await _repository.GetAsync();

            // Assert
            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.Id, Is.EqualTo(settingsOne.Id));
            Assert.That(settings.IsEnabled, Is.EqualTo(settingsOne.IsEnabled));
            Assert.That(settings.IsReportOnly, Is.EqualTo(settingsOne.IsReportOnly));
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public async Task SaveAsync_CreatesANewRecordWhenCspSettingsDoNotExist(bool isEnabled, bool isReportOnly)
        {
            // Arrange
            CspSettings settingsSaved = null;
            _mockDbSet.Setup(x => x.Add(It.IsAny<CspSettings>()))
                      .Callback<CspSettings>(x => settingsSaved = x);

            // Act
            await _repository.SaveAsync(isEnabled, isReportOnly);

            // Assert
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(settingsSaved, Is.Not.Null);
            Assert.That(settingsSaved.IsEnabled, Is.EqualTo(isEnabled));
            Assert.That(settingsSaved.IsReportOnly, Is.EqualTo(isReportOnly));
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public async Task SaveAsync_CreateUpdatesTheFirstCspSettingsWhenSettingsExist(bool isEnabled, bool isReportOnly)
        {
            // Arrange
            var existingRecord = new CspSettings
            {
                Id = Guid.NewGuid(),
                IsEnabled = false,
                IsReportOnly = false
            };

            _mockDbSet = DbSetMocker.GetQueryableMockDbSet(existingRecord);
            _mockContext.Setup(x => x.CspSettings).Returns(_mockDbSet.Object);

            // Act
            await _repository.SaveAsync(isEnabled, isReportOnly);

            // Assert
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(existingRecord.IsEnabled, Is.EqualTo(isEnabled));
            Assert.That(existingRecord.IsReportOnly, Is.EqualTo(isReportOnly));
        }
    }
}
