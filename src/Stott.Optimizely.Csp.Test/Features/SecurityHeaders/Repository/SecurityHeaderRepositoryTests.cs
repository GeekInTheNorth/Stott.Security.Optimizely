using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Repository;

namespace Stott.Optimizely.Csp.Test.Features.SecurityHeaders.Repository
{
    [TestFixture]
    public class SecurityHeaderRepositoryTests
    {
        private Mock<ICspDataContext> _mockContext;

        private Mock<DbSet<SecurityHeaderSettings>> _mockDbSet;

        private SecurityHeaderRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<ICspDataContext>();
            _mockDbSet = DbSetMocker.GetQueryableMockDbSet<SecurityHeaderSettings>();
            _mockContext.Setup(x => x.SecurityHeaderSettings).Returns(_mockDbSet.Object);

            _repository = new SecurityHeaderRepository(_mockContext.Object);
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
                FrameOptions = XFrameOptions.SameOrigin,
                ReferrerPolicy = ReferrerPolicy.SameOrigin,
                IsXContentTypeOptionsEnabled = true,
                IsXXssProtectionEnabled = true
            };

            var settingsTwo = new SecurityHeaderSettings
            {
                FrameOptions = XFrameOptions.Deny,
                ReferrerPolicy = ReferrerPolicy.NoReferrer,
                IsXContentTypeOptionsEnabled = false,
                IsXXssProtectionEnabled = false
            };

            _mockDbSet = DbSetMocker.GetQueryableMockDbSet(settingsOne, settingsTwo);
            _mockContext.Setup(x => x.SecurityHeaderSettings).Returns(_mockDbSet.Object);

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
            SecurityHeaderSettings settingsSaved = null;
            _mockDbSet.Setup(x => x.Add(It.IsAny<SecurityHeaderSettings>()))
                      .Callback<SecurityHeaderSettings>(x => settingsSaved = x);

            // Act
            await _repository.SaveAsync(isXContentTypeOptionsEnabled, isXXssProtectionEnabled, referrerPolicy, xFrameOptions);

            // Assert
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(settingsSaved, Is.Not.Null);
            Assert.That(settingsSaved.IsXContentTypeOptionsEnabled, Is.EqualTo(isXContentTypeOptionsEnabled));
            Assert.That(settingsSaved.IsXXssProtectionEnabled, Is.EqualTo(isXXssProtectionEnabled));
            Assert.That(settingsSaved.ReferrerPolicy, Is.EqualTo(referrerPolicy));
            Assert.That(settingsSaved.FrameOptions, Is.EqualTo(xFrameOptions));
        }

        [Test]
        [TestCase(XFrameOptions.Deny, ReferrerPolicy.NoReferrer, true, true)]
        [TestCase(XFrameOptions.SameOrigin, ReferrerPolicy.NoReferrerWhenDowngrade, true, false)]
        [TestCase(XFrameOptions.Deny, ReferrerPolicy.Origin, false, true)]
        [TestCase(XFrameOptions.SameOrigin, ReferrerPolicy.OriginWhenCrossOrigin, false, false)]
        public async Task SaveAsync_CreateUpdatesTheFirstCspSettingsWhenSettingsExist(
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

            _mockDbSet = DbSetMocker.GetQueryableMockDbSet(existingRecord);
            _mockContext.Setup(x => x.SecurityHeaderSettings).Returns(_mockDbSet.Object);

            // Act
            await _repository.SaveAsync(isXContentTypeOptionsEnabled, isXXssProtectionEnabled, referrerPolicy, xFrameOptions);

            // Assert
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(existingRecord.IsXContentTypeOptionsEnabled, Is.EqualTo(isXContentTypeOptionsEnabled));
            Assert.That(existingRecord.IsXXssProtectionEnabled, Is.EqualTo(isXXssProtectionEnabled));
            Assert.That(existingRecord.ReferrerPolicy, Is.EqualTo(referrerPolicy));
            Assert.That(existingRecord.FrameOptions, Is.EqualTo(xFrameOptions));
        }
    }
}
