using System;
using System.Collections.Generic;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Settings.Repository;

namespace Stott.Optimizely.Csp.Test.Features.Settings.Repository
{
    [TestFixture]
    public class CspSettingsRepositoryTests
    {
        //private Mock<DynamicDataStoreFactory> _mockDynamicDataStoreFactory;

        //private Mock<DynamicDataStore> _mockDynamicDataStore;

        //private Mock<StoreDefinition> _mockStoreDefinition;

        //private CspSettingsRepository _repository;

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
        //    _mockDynamicDataStoreFactory.Setup(x => x.CreateStore(typeof(CspSettings))).Returns(_mockDynamicDataStore.Object);

        //    _repository = new CspSettingsRepository(_mockDynamicDataStoreFactory.Object);
        //}

        //[Test]
        //public void Get_GivenThereAreNoSavedSettings_ThenDefaultSettingsShouldBeReturned()
        //{
        //    // Arrange
        //    _mockDynamicDataStore.Setup(x => x.LoadAll<CspSettings>())
        //                         .Returns(new List<CspSettings>(0));

        //    // Act
        //    var settings = _repository.GetAsync();

        //    // Assert
        //    Assert.That(settings, Is.Not.Null);
        //    Assert.That(settings.IsEnabled, Is.False);
        //    Assert.That(settings.IsReportOnly, Is.False);
        //}

        //[Test]
        //public void Get_GivenThereAreMultipleSavedSettings_ThenThefirstSettingsShouldBeReturned()
        //{
        //    // Arrange
        //    var settingsOne = new CspSettings { IsEnabled = true, IsReportOnly = false };
        //    var settingsTwo = new CspSettings { IsEnabled = false, IsReportOnly = true };

        //    _mockDynamicDataStore.Setup(x => x.LoadAll<CspSettings>())
        //                         .Returns(new List<CspSettings> { settingsOne, settingsTwo });

        //    // Act
        //    var settings = _repository.GetAsync();

        //    // Assert
        //    Assert.That(settings, Is.Not.Null);
        //    Assert.That(settings.IsEnabled, Is.EqualTo(settingsOne.IsEnabled));
        //    Assert.That(settings.IsReportOnly, Is.EqualTo(settingsOne.IsReportOnly));
        //}

        //[Test]
        //[TestCase(true, true)]
        //[TestCase(true, false)]
        //[TestCase(false, true)]
        //[TestCase(false, false)]
        //public void Save_CreatesANewRecordWhenCspSettingsDoNotExist(bool isEnabled, bool isReportOnly)
        //{
        //    // Arrange
        //    _mockDynamicDataStore.Setup(x => x.LoadAll<CspSettings>())
        //                         .Returns(new List<CspSettings>(0));

        //    CspSettings settingsSaved = null;
        //    _mockDynamicDataStore.Setup(x => x.Save(It.IsAny<object>()))
        //                         .Callback<object>(x => settingsSaved = x as CspSettings);

        //    // Act
        //    _repository.SaveAsync(isEnabled, isReportOnly);

        //    // Assert
        //    Assert.That(settingsSaved, Is.Not.Null);
        //    Assert.That(settingsSaved.IsEnabled, Is.EqualTo(isEnabled));
        //    Assert.That(settingsSaved.IsReportOnly, Is.EqualTo(isReportOnly));
        //}

        //[Test]
        //[TestCase(true, true)]
        //[TestCase(true, false)]
        //[TestCase(false, true)]
        //[TestCase(false, false)]
        //public void Save_CreateUpdatesTheFirstCspSettingsWhenSettingsExist(bool isEnabled, bool isReportOnly)
        //{
        //    // Arrange
        //    var existingRecord = new CspSettings
        //    {
        //        Id = Identity.NewIdentity(Guid.NewGuid()),
        //        IsEnabled = false,
        //        IsReportOnly = false
        //    };

        //    _mockDynamicDataStore.Setup(x => x.LoadAll<CspSettings>())
        //                         .Returns(new List<CspSettings> { existingRecord });

        //    CspSettings settingsSaved = null;
        //    _mockDynamicDataStore.Setup(x => x.Save(It.IsAny<object>()))
        //                         .Callback<object>(x => settingsSaved = x as CspSettings);

        //    // Act
        //    _repository.SaveAsync(isEnabled, isReportOnly);

        //    // Assert
        //    Assert.That(settingsSaved, Is.Not.Null);
        //    Assert.That(settingsSaved.Id.ExternalId, Is.EqualTo(existingRecord.Id.ExternalId));
        //    Assert.That(settingsSaved.IsEnabled, Is.EqualTo(isEnabled));
        //    Assert.That(settingsSaved.IsReportOnly, Is.EqualTo(isReportOnly));
        //}
    }
}
