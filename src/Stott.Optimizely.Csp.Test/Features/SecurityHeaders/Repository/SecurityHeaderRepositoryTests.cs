using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.SecurityHeaders.Enums;
using Stott.Security.Core.Features.SecurityHeaders.Repository;

namespace Stott.Optimizely.Csp.Test.Features.SecurityHeaders.Repository
{
    [TestFixture]
    public class SecurityHeaderRepositoryTests
    {
        private TestDataContext _inMemoryDatabase;

        private SecurityHeaderRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<TestDataContext>()
            .UseInMemoryDatabase(databaseName: "CspDatabase")
            .Options;

            _inMemoryDatabase = new TestDataContext(options);

            _repository = new SecurityHeaderRepository(_inMemoryDatabase);
        }

        [TearDown]
        public async Task TearDown()
        {
            _inMemoryDatabase.SetExecuteSqlAsyncResult(0);

            var allData = await _inMemoryDatabase.SecurityHeaderSettings.AsQueryable().ToListAsync();
            if (allData.Any())
            {
                _inMemoryDatabase.SecurityHeaderSettings.RemoveRange(allData);
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
            Assert.That(settings.IsXContentTypeOptionsEnabled, Is.False);
            Assert.That(settings.IsXXssProtectionEnabled, Is.False);
            Assert.That(settings.ReferrerPolicy, Is.EqualTo(ReferrerPolicy.None));
            Assert.That(settings.FrameOptions, Is.EqualTo(XFrameOptions.None));
        }

        [Test]
        public async Task GetAsync_GivenThereAreMultipleSavedSettings_ThenThefirstSettingsShouldBeReturned()
        {
            // Arrange
            var settingsOne = new SecurityHeaderSettings
            {
                Id = Guid.NewGuid(),
                FrameOptions = XFrameOptions.SameOrigin,
                ReferrerPolicy = ReferrerPolicy.SameOrigin,
                IsXContentTypeOptionsEnabled = true,
                IsXXssProtectionEnabled = true
            };

            var settingsTwo = new SecurityHeaderSettings
            {
                Id = Guid.NewGuid(),
                FrameOptions = XFrameOptions.Deny,
                ReferrerPolicy = ReferrerPolicy.NoReferrer,
                IsXContentTypeOptionsEnabled = false,
                IsXXssProtectionEnabled = false
            };

            _inMemoryDatabase.SecurityHeaderSettings.AddRange(settingsOne, settingsTwo);
            _inMemoryDatabase.SaveChanges();

            // Act
            var settings = await _repository.GetAsync();

            // Assert
            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.IsXContentTypeOptionsEnabled, Is.EqualTo(settingsOne.IsXContentTypeOptionsEnabled));
            Assert.That(settings.IsXXssProtectionEnabled, Is.EqualTo(settingsOne.IsXXssProtectionEnabled));
            Assert.That(settings.ReferrerPolicy, Is.EqualTo(settingsOne.ReferrerPolicy));
            Assert.That(settings.FrameOptions, Is.EqualTo(settingsOne.FrameOptions));
        }

        [Test]
        [TestCase(XFrameOptions.None, ReferrerPolicy.None, true, true)]
        [TestCase(XFrameOptions.SameOrigin, ReferrerPolicy.NoReferrer, true, false)]
        [TestCase(XFrameOptions.Deny, ReferrerPolicy.NoReferrerWhenDowngrade,false, true)]
        [TestCase(XFrameOptions.None, ReferrerPolicy.Origin, false, false)]
        public async Task SaveAsync_CreatesANewRecordWhenSecurityHeaderSettingsDoNotExist(
            XFrameOptions xFrameOptions,
            ReferrerPolicy referrerPolicy,
            bool isXContentTypeOptionsEnabled, 
            bool isXXssProtectionEnabled)
        {
            // Act
            var originalCount = await _inMemoryDatabase.SecurityHeaderSettings.AsQueryable().CountAsync();

            await _repository.SaveAsync(isXContentTypeOptionsEnabled, isXXssProtectionEnabled, referrerPolicy, xFrameOptions);

            var updatedCount = await _inMemoryDatabase.SecurityHeaderSettings.AsQueryable().CountAsync();
            var createdRecord = await _inMemoryDatabase.SecurityHeaderSettings.AsQueryable().LastOrDefaultAsync();

            // Assert
            Assert.That(updatedCount, Is.GreaterThan(originalCount));
            Assert.That(createdRecord, Is.Not.Null);
            Assert.That(createdRecord.IsXContentTypeOptionsEnabled, Is.EqualTo(isXContentTypeOptionsEnabled));
            Assert.That(createdRecord.IsXXssProtectionEnabled, Is.EqualTo(isXXssProtectionEnabled));
            Assert.That(createdRecord.ReferrerPolicy, Is.EqualTo(referrerPolicy));
            Assert.That(createdRecord.FrameOptions, Is.EqualTo(xFrameOptions));
        }

        [Test]
        [TestCase(XFrameOptions.Deny, ReferrerPolicy.NoReferrer, true, true)]
        [TestCase(XFrameOptions.SameOrigin, ReferrerPolicy.NoReferrerWhenDowngrade, true, false)]
        [TestCase(XFrameOptions.Deny, ReferrerPolicy.Origin, false, true)]
        [TestCase(XFrameOptions.SameOrigin, ReferrerPolicy.OriginWhenCrossOrigin, false, false)]
        public async Task SaveAsync_UpdatesTheFirstCspSettingsWhenSettingsExist(
            XFrameOptions xFrameOptions,
            ReferrerPolicy referrerPolicy,
            bool isXContentTypeOptionsEnabled,
            bool isXXssProtectionEnabled)
        {
            // Arrange
            var existingRecord = new SecurityHeaderSettings
            {
                Id = Guid.NewGuid(),
                IsXContentTypeOptionsEnabled = false,
                IsXXssProtectionEnabled = false,
                FrameOptions = XFrameOptions.None,
                ReferrerPolicy = ReferrerPolicy.None
            };

            _inMemoryDatabase.SecurityHeaderSettings.Add(existingRecord);
            _inMemoryDatabase.SaveChanges();

            // Act
            var originalCount = await _inMemoryDatabase.SecurityHeaderSettings.AsQueryable().CountAsync();

            await _repository.SaveAsync(isXContentTypeOptionsEnabled, isXXssProtectionEnabled, referrerPolicy, xFrameOptions);

            var updatedCount = await _inMemoryDatabase.SecurityHeaderSettings.AsQueryable().CountAsync();
            var updatedRecord = await _inMemoryDatabase.SecurityHeaderSettings.AsQueryable().FirstOrDefaultAsync();

            // Assert
            Assert.That(updatedCount, Is.EqualTo(originalCount));
            Assert.That(updatedRecord.IsXContentTypeOptionsEnabled, Is.EqualTo(isXContentTypeOptionsEnabled));
            Assert.That(updatedRecord.IsXXssProtectionEnabled, Is.EqualTo(isXXssProtectionEnabled));
            Assert.That(updatedRecord.ReferrerPolicy, Is.EqualTo(referrerPolicy));
            Assert.That(updatedRecord.FrameOptions, Is.EqualTo(xFrameOptions));
        }
    }
}
