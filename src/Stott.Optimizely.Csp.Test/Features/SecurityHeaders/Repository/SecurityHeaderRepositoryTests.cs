using System;
using System.Collections.Generic;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

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
        //private Mock<DynamicDataStoreFactory> _mockDynamicDataStoreFactory;

        //private Mock<DynamicDataStore> _mockDynamicDataStore;

        //private Mock<StoreDefinition> _mockStoreDefinition;

        //private SecurityHeaderRepository _repository;

        //[SetUp]
        //public void SetUp()
        //{
        //    _mockStoreDefinition = new Mock<StoreDefinition>(
        //        MockBehavior.Loose,
        //        string.Empty,
        //        new List<PropertyMap>(0),
        //        null);

        //    _mockDynamicDataStore = new Mock<DynamicDataStore>(
        //        MockBehavior.Loose,
        //        _mockStoreDefinition.Object);

        //    _mockDynamicDataStoreFactory = new Mock<DynamicDataStoreFactory>();
        //    _mockDynamicDataStoreFactory.Setup(x => x.CreateStore(typeof(SecurityHeaderSettings))).Returns(_mockDynamicDataStore.Object);

        //    _repository = new SecurityHeaderRepository(_mockDynamicDataStoreFactory.Object);
        //}

        //[Test]
        //public void Get_GivenThereAreNoSavedSettings_ThenDefaultSettingsShouldBeReturned()
        //{
        //    // Arrange
        //    _mockDynamicDataStore.Setup(x => x.LoadAll<SecurityHeaderSettings>())
        //                         .Returns(new List<SecurityHeaderSettings>(0));

        //    // Act
        //    var settings = _repository.GetAsync();

        //    // Assert
        //    Assert.That(settings, Is.Not.Null);
        //    Assert.That(settings.IsXContentTypeOptionsEnabled, Is.False);
        //    Assert.That(settings.IsXXssProtectionEnabled, Is.False);
        //    Assert.That(settings.ReferrerPolicy, Is.EqualTo(ReferrerPolicy.None));
        //    Assert.That(settings.FrameOptions, Is.EqualTo(XFrameOptions.None));
        //}

        //[Test]
        //public void Get_GivenThereAreMultipleSavedSettings_ThenThefirstSettingsShouldBeReturned()
        //{
        //    // Arrange
        //    var settingsOne = new SecurityHeaderSettings
        //    {
        //        FrameOptions = XFrameOptions.SameOrigin,
        //        ReferrerPolicy = ReferrerPolicy.SameOrigin,
        //        IsXContentTypeOptionsEnabled = true,
        //        IsXXssProtectionEnabled = true
        //    };

        //    var settingsTwo = new SecurityHeaderSettings
        //    {
        //        FrameOptions = XFrameOptions.Deny,
        //        ReferrerPolicy = ReferrerPolicy.NoReferrer,
        //        IsXContentTypeOptionsEnabled = false,
        //        IsXXssProtectionEnabled = false
        //    };

        //    _mockDynamicDataStore.Setup(x => x.LoadAll<SecurityHeaderSettings>())
        //                         .Returns(new List<SecurityHeaderSettings> { settingsOne, settingsTwo });

        //    // Act
        //    var settings = _repository.GetAsync();

        //    // Assert
        //    Assert.That(settings, Is.Not.Null);
        //    Assert.That(settings.IsXContentTypeOptionsEnabled, Is.EqualTo(settingsOne.IsXContentTypeOptionsEnabled));
        //    Assert.That(settings.IsXXssProtectionEnabled, Is.EqualTo(settingsOne.IsXXssProtectionEnabled));
        //    Assert.That(settings.ReferrerPolicy, Is.EqualTo(settingsOne.ReferrerPolicy));
        //    Assert.That(settings.FrameOptions, Is.EqualTo(settingsOne.FrameOptions));
        //}

        //[Test]
        //[TestCase(XFrameOptions.None, ReferrerPolicy.None, true, true)]
        //[TestCase(XFrameOptions.SameOrigin, ReferrerPolicy.NoReferrer, true, false)]
        //[TestCase(XFrameOptions.Deny, ReferrerPolicy.NoReferrerWhenDowngrade,false, true)]
        //[TestCase(XFrameOptions.None, ReferrerPolicy.Origin, false, false)]
        //public void Save_CreatesANewRecordWhenSecurityHeaderSettingsDoNotExist(
        //    XFrameOptions xFrameOptions,
        //    ReferrerPolicy referrerPolicy,
        //    bool isXContentTypeOptionsEnabled, 
        //    bool isXXssProtectionEnabled)
        //{
        //    // Arrange
        //    _mockDynamicDataStore.Setup(x => x.LoadAll<SecurityHeaderSettings>())
        //                         .Returns(new List<SecurityHeaderSettings>(0));

        //    SecurityHeaderSettings settingsSaved = null;
        //    _mockDynamicDataStore.Setup(x => x.Save(It.IsAny<object>()))
        //                         .Callback<object>(x => settingsSaved = x as SecurityHeaderSettings);

        //    // Act
        //    _repository.SaveAsync(isXContentTypeOptionsEnabled, isXXssProtectionEnabled, referrerPolicy, xFrameOptions);

        //    // Assert
        //    Assert.That(settingsSaved, Is.Not.Null);
        //    Assert.That(settingsSaved.IsXContentTypeOptionsEnabled, Is.EqualTo(isXContentTypeOptionsEnabled));
        //    Assert.That(settingsSaved.IsXXssProtectionEnabled, Is.EqualTo(isXXssProtectionEnabled));
        //    Assert.That(settingsSaved.ReferrerPolicy, Is.EqualTo(referrerPolicy));
        //    Assert.That(settingsSaved.FrameOptions, Is.EqualTo(xFrameOptions));
        //}

        //[Test]
        //[TestCase(XFrameOptions.Deny, ReferrerPolicy.NoReferrer, true, true)]
        //[TestCase(XFrameOptions.SameOrigin, ReferrerPolicy.NoReferrerWhenDowngrade, true, false)]
        //[TestCase(XFrameOptions.Deny, ReferrerPolicy.Origin, false, true)]
        //[TestCase(XFrameOptions.SameOrigin, ReferrerPolicy.OriginWhenCrossOrigin, false, false)]
        //public void Save_CreateUpdatesTheFirstCspSettingsWhenSettingsExist(
        //    XFrameOptions xFrameOptions,
        //    ReferrerPolicy referrerPolicy,
        //    bool isXContentTypeOptionsEnabled,
        //    bool isXXssProtectionEnabled)
        //{
        //    // Arrange
        //    var existingRecord = new SecurityHeaderSettings
        //    {
        //        Id = Identity.NewIdentity(Guid.NewGuid()),
        //        IsXContentTypeOptionsEnabled = false,
        //        IsXXssProtectionEnabled = false,
        //        FrameOptions = XFrameOptions.None,
        //        ReferrerPolicy = ReferrerPolicy.None
        //    };

        //    _mockDynamicDataStore.Setup(x => x.LoadAll<SecurityHeaderSettings>())
        //                         .Returns(new List<SecurityHeaderSettings> { existingRecord });

        //    SecurityHeaderSettings settingsSaved = null;
        //    _mockDynamicDataStore.Setup(x => x.Save(It.IsAny<object>()))
        //                         .Callback<object>(x => settingsSaved = x as SecurityHeaderSettings);

        //    // Act
        //    _repository.SaveAsync(isXContentTypeOptionsEnabled, isXXssProtectionEnabled, referrerPolicy, xFrameOptions);

        //    // Assert
        //    Assert.That(settingsSaved, Is.Not.Null);
        //    Assert.That(settingsSaved.IsXContentTypeOptionsEnabled, Is.EqualTo(isXContentTypeOptionsEnabled));
        //    Assert.That(settingsSaved.IsXXssProtectionEnabled, Is.EqualTo(isXXssProtectionEnabled));
        //    Assert.That(settingsSaved.ReferrerPolicy, Is.EqualTo(referrerPolicy));
        //    Assert.That(settingsSaved.FrameOptions, Is.EqualTo(xFrameOptions));
        //}
    }
}
