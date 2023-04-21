namespace Stott.Security.Optimizely.Test.Features.Settings.Repository;

using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Settings;
using Stott.Security.Optimizely.Features.Settings.Repository;

[TestFixture]
public class CspSettingsRepositoryTests
{
    private TestDataContext _inMemoryDatabase;

    private CspSettingsRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _repository = new CspSettingsRepository(_inMemoryDatabase);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _inMemoryDatabase.Reset();
    }

    [Test]
    public async Task GetAsync_GivenThereAreNoSavedSettings_ThenDefaultSettingsShouldBeReturned()
    {
        // Act
        var settings = await _repository.GetAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.Id, Is.EqualTo(Guid.Empty));
            Assert.That(settings.IsEnabled, Is.False);
            Assert.That(settings.IsReportOnly, Is.False);
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(settings, Is.Not.Null);
            Assert.That(settings.Id, Is.EqualTo(settingsOne.Id));
            Assert.That(settings.IsEnabled, Is.EqualTo(settingsOne.IsEnabled));
            Assert.That(settings.IsReportOnly, Is.EqualTo(settingsOne.IsReportOnly));
        });
    }

    [Test]
    [TestCase(true, true, true, "https://www.example.com/one.json", false, "test.user.one")]
    [TestCase(true, false, false, null, true, "test.user.two")]
    [TestCase(false, true, true, "https://www.example.com/two.json", false, "test.user.three")]
    [TestCase(false, false, false, null, false, "test.user.four")]
    public async Task SaveAsync_CreatesANewRecordWhenCspSettingsDoNotExist(
        bool isEnabled, 
        bool isReportOnly, 
        bool isWhitelistEnabled,
        string whitelistUrl,
        bool isUpgradeInsecureRequestsEnabled,
        string modifiedBy)
    {
        // Act
        var modelToSave = new Mock<ICspSettings>();
        modelToSave.Setup(x => x.IsEnabled).Returns(isEnabled);
        modelToSave.Setup(x => x.IsReportOnly).Returns(isReportOnly);
        modelToSave.Setup(x => x.IsWhitelistEnabled).Returns(isWhitelistEnabled);
        modelToSave.Setup(x => x.WhitelistUrl).Returns(whitelistUrl);
        modelToSave.Setup(x => x.IsUpgradeInsecureRequestsEnabled).Returns(isUpgradeInsecureRequestsEnabled);

        var originalCount = await _inMemoryDatabase.CspSettings.AsQueryable().CountAsync();

        await _repository.SaveAsync(modelToSave.Object, modifiedBy);

        var updatedCount = await _inMemoryDatabase.CspSettings.AsQueryable().CountAsync();
        var createdRecord = await _inMemoryDatabase.CspSettings.AsQueryable().FirstOrDefaultAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(originalCount, Is.EqualTo(0));
            Assert.That(updatedCount, Is.EqualTo(1));
            Assert.That(createdRecord, Is.Not.Null);
            Assert.That(createdRecord.IsEnabled, Is.EqualTo(isEnabled));
            Assert.That(createdRecord.IsReportOnly, Is.EqualTo(isReportOnly));
            Assert.That(createdRecord.IsWhitelistEnabled, Is.EqualTo(isWhitelistEnabled));
            Assert.That(createdRecord.WhitelistUrl, Is.EqualTo(whitelistUrl));
            Assert.That(createdRecord.IsUpgradeInsecureRequestsEnabled, Is.EqualTo(isUpgradeInsecureRequestsEnabled));
            Assert.That(createdRecord.Modified, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(3)));
            Assert.That(createdRecord.ModifiedBy, Is.EqualTo(modifiedBy));
        });
    }

    [Test]
    [TestCase(true, true, true, "https://www.example.com/one.json", false, "test.user.one")]
    [TestCase(true, false, false, null, true, "test.user.two")]
    [TestCase(false, true, true, "https://www.example.com/two.json", false, "test.user.three")]
    [TestCase(false, false, false, null, false, "test.user.four")]
    public async Task SaveAsync_CreateUpdatesTheFirstCspSettingsWhenSettingsExist(
        bool isEnabled, 
        bool isReportOnly,
        bool isWhitelistEnabled,
        string whitelistUrl,
        bool isUpgradeInsecureRequestsEnabled,
        string modifiedBy)
    {
        // Arrange
        var modelToSave = new Mock<ICspSettings>();
        modelToSave.Setup(x => x.IsEnabled).Returns(isEnabled);
        modelToSave.Setup(x => x.IsReportOnly).Returns(isReportOnly);
        modelToSave.Setup(x => x.IsWhitelistEnabled).Returns(isWhitelistEnabled);
        modelToSave.Setup(x => x.WhitelistUrl).Returns(whitelistUrl);
        modelToSave.Setup(x => x.IsUpgradeInsecureRequestsEnabled).Returns(isUpgradeInsecureRequestsEnabled);

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

        await _repository.SaveAsync(modelToSave.Object, modifiedBy);

        var updatedCount = await _inMemoryDatabase.CspSettings.AsQueryable().CountAsync();
        var updatedRecord = await _inMemoryDatabase.CspSettings.AsQueryable().FirstOrDefaultAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(originalCount, Is.EqualTo(updatedCount));
            Assert.That(updatedRecord.IsEnabled, Is.EqualTo(isEnabled));
            Assert.That(updatedRecord.IsReportOnly, Is.EqualTo(isReportOnly));
            Assert.That(updatedRecord.IsWhitelistEnabled, Is.EqualTo(isWhitelistEnabled));
            Assert.That(updatedRecord.WhitelistUrl, Is.EqualTo(whitelistUrl));
            Assert.That(updatedRecord.IsUpgradeInsecureRequestsEnabled, Is.EqualTo(isUpgradeInsecureRequestsEnabled));
            Assert.That(updatedRecord.Modified, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(3)));
            Assert.That(updatedRecord.ModifiedBy, Is.EqualTo(modifiedBy));
        });
    }
}