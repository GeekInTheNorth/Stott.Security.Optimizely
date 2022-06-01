using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Settings.Repository;

namespace Stott.Optimizely.Csp.Test.Features.Settings.Repository
{
    [TestFixture]
    public class CspSettingsRepositoryTests
    {
        private TestDataContext _inMemoryDatabase;

        private CspSettingsRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<TestDataContext>()
            .UseInMemoryDatabase(databaseName: "CspDatabase")
            .Options;

            _inMemoryDatabase = new TestDataContext(options);

            _repository = new CspSettingsRepository(_inMemoryDatabase);
        }

        [TearDown]
        public async Task TearDown()
        {
            _inMemoryDatabase.SetExecuteSqlAsyncResult(0);

            var allData = await _inMemoryDatabase.CspSettings.AsQueryable().ToListAsync();
            if (allData.Any())
            {
                _inMemoryDatabase.CspSettings.RemoveRange(allData);
                _inMemoryDatabase.SaveChanges();
            }
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

            _inMemoryDatabase.CspSettings.AddRange(settingsOne, settingsTwo);
            _inMemoryDatabase.SaveChanges();

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
            // Act
            var originalCount = await _inMemoryDatabase.CspSettings.AsQueryable().CountAsync();

            await _repository.SaveAsync(isEnabled, isReportOnly);

            var updatedCount = await _inMemoryDatabase.CspSettings.AsQueryable().CountAsync();
            var createdRecord = await _inMemoryDatabase.CspSettings.AsQueryable().FirstOrDefaultAsync();

            // Assert
            Assert.That(originalCount, Is.EqualTo(0));
            Assert.That(updatedCount, Is.EqualTo(1));
            Assert.That(createdRecord, Is.Not.Null);
            Assert.That(createdRecord.IsEnabled, Is.EqualTo(isEnabled));
            Assert.That(createdRecord.IsReportOnly, Is.EqualTo(isReportOnly));
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

            _inMemoryDatabase.CspSettings.AddRange(existingRecord);
            _inMemoryDatabase.SaveChanges();

            // Act
            var originalCount = await _inMemoryDatabase.CspSettings.AsQueryable().CountAsync();

            await _repository.SaveAsync(isEnabled, isReportOnly);

            var updatedCount = await _inMemoryDatabase.CspSettings.AsQueryable().CountAsync();
            var updatedRecord = await _inMemoryDatabase.CspSettings.AsQueryable().FirstOrDefaultAsync();

            // Assert
            Assert.That(originalCount, Is.EqualTo(updatedCount));
            Assert.That(updatedRecord.IsEnabled, Is.EqualTo(isEnabled));
            Assert.That(updatedRecord.IsReportOnly, Is.EqualTo(isReportOnly));
        }
    }
}
