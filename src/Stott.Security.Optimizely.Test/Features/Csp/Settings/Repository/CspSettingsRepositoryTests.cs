namespace Stott.Security.Optimizely.Test.Features.Csp.Settings.Repository;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Settings;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;

[TestFixture]
public sealed class CspSettingsRepositoryTests
{
    private static readonly Guid SiteA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly Guid SiteB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private TestDataContext _inMemoryDatabase;

    private Lazy<ICspDataContext> _lazyInMemoryDatabase;

    private CspSettingsRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _lazyInMemoryDatabase = new Lazy<ICspDataContext>(() => _inMemoryDatabase);

        _repository = new CspSettingsRepository(_lazyInMemoryDatabase);
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
        var settings = await _repository.GetAsync(null, null);

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
    public async Task GetAsync_GivenThereAreMultipleSavedSettingsAtTheSameScope_ThenOneOfThoseRecordsIsReturned()
    {
        // As of v6.0.0 the fallback chain ranks candidates by (SiteId, HostName) specificity only;
        // when multiple records exist at the same scope (e.g. two Global rows) the winning row is
        // implementation-defined. The repository's upsert path ensures duplicate per-context rows
        // shouldn't be created in practice, but this test pins the resilience behaviour.
        // Arrange
        var settingsOne = new CspSettings { Id = Guid.NewGuid(), IsEnabled = true, IsReportOnly = false };
        var settingsTwo = new CspSettings { Id = Guid.NewGuid(), IsEnabled = false, IsReportOnly = true };

        _inMemoryDatabase.CspSettings.AddRange(settingsOne, settingsTwo);
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetAsync(null, null);

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.Id, Is.AnyOf(settingsOne.Id, settingsTwo.Id));
    }

    [Test]
    [TestCase(true, true, true, "https://www.example.com/one.json", false, "test.user.one")]
    [TestCase(true, false, false, null, true, "test.user.two")]
    [TestCase(false, true, true, "https://www.example.com/two.json", false, "test.user.three")]
    [TestCase(false, false, false, null, false, "test.user.four")]
    public async Task SaveAsync_CreatesANewRecordWhenCspSettingsDoNotExist(
        bool isEnabled,
        bool isReportOnly,
        bool isAllowListEnabled,
        string allowListUrl,
        bool isUpgradeInsecureRequestsEnabled,
        string modifiedBy)
    {
        // Act
        var modelToSave = new Mock<ICspSettings>();
        modelToSave.Setup(x => x.IsEnabled).Returns(isEnabled);
        modelToSave.Setup(x => x.IsReportOnly).Returns(isReportOnly);
        modelToSave.Setup(x => x.IsAllowListEnabled).Returns(isAllowListEnabled);
        modelToSave.Setup(x => x.AllowListUrl).Returns(allowListUrl);
        modelToSave.Setup(x => x.IsUpgradeInsecureRequestsEnabled).Returns(isUpgradeInsecureRequestsEnabled);

        var originalCount = await _inMemoryDatabase.CspSettings.AsQueryable().CountAsync();

        await _repository.SaveAsync(modelToSave.Object, null, null, modifiedBy);

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
            Assert.That(createdRecord.IsAllowListEnabled, Is.EqualTo(isAllowListEnabled));
            Assert.That(createdRecord.AllowListUrl, Is.EqualTo(allowListUrl));
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
        bool isAllowListEnabled,
        string allowListUrl,
        bool isUpgradeInsecureRequestsEnabled,
        string modifiedBy)
    {
        // Arrange
        var modelToSave = new Mock<ICspSettings>();
        modelToSave.Setup(x => x.IsEnabled).Returns(isEnabled);
        modelToSave.Setup(x => x.IsReportOnly).Returns(isReportOnly);
        modelToSave.Setup(x => x.IsAllowListEnabled).Returns(isAllowListEnabled);
        modelToSave.Setup(x => x.AllowListUrl).Returns(allowListUrl);
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

        await _repository.SaveAsync(modelToSave.Object, null, null, modifiedBy);

        var updatedCount = await _inMemoryDatabase.CspSettings.AsQueryable().CountAsync();
        var updatedRecord = await _inMemoryDatabase.CspSettings.AsQueryable().FirstOrDefaultAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(originalCount, Is.EqualTo(updatedCount));
            Assert.That(updatedRecord.IsEnabled, Is.EqualTo(isEnabled));
            Assert.That(updatedRecord.IsReportOnly, Is.EqualTo(isReportOnly));
            Assert.That(updatedRecord.IsAllowListEnabled, Is.EqualTo(isAllowListEnabled));
            Assert.That(updatedRecord.AllowListUrl, Is.EqualTo(allowListUrl));
            Assert.That(updatedRecord.IsUpgradeInsecureRequestsEnabled, Is.EqualTo(isUpgradeInsecureRequestsEnabled));
            Assert.That(updatedRecord.Modified, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(3)));
            Assert.That(updatedRecord.ModifiedBy, Is.EqualTo(modifiedBy));
        });
    }

    [Test]
    public async Task GetAsync_OnlyGlobalExists_ReturnsGlobal()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = null,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetAsync(SiteA, "host.com");

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.IsEnabled, Is.True);
    }

    [Test]
    public async Task GetAsync_SiteAndGlobalExist_ReturnsSite()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = null,
            HostName = null,
            IsReportOnly = false
        });
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = SiteA,
            HostName = null,
            IsReportOnly = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetAsync(SiteA, null);

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.IsReportOnly, Is.True);
    }

    [Test]
    public async Task GetAsync_HostSiteAndGlobalExist_ReturnsHost()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = null,
            HostName = null,
            IsReportOnly = false
        });
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = SiteA,
            HostName = null,
            IsReportOnly = false
        });
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = SiteA,
            HostName = "www.example.com",
            IsReportOnly = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetAsync(SiteA, "www.example.com");

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.IsReportOnly, Is.True);
    }

    [Test]
    public async Task GetAsync_SiteIdSuppliedButNoSiteRecord_FallsBackToGlobal()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = null,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetAsync(SiteA, null);

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.IsEnabled, Is.True);
    }

    [Test]
    public async Task GetAsync_HostNameSuppliedButNoHostRecord_FallsBackToSite()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = null,
            HostName = null,
            IsEnabled = false
        });
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = SiteA,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetAsync(SiteA, "www.example.com");

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.IsEnabled, Is.True);
    }

    [Test]
    public async Task GetAsync_NullSiteIdWithOtherSiteRecordsPresent_StillReturnsGlobal()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = null,
            HostName = null,
            IsEnabled = false
        });
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = SiteA,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetAsync(null, null);

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.IsEnabled, Is.False);
    }

    [Test]
    public async Task GetByContextAsync_NoExactMatch_ReturnsNull()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = null,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetByContextAsync(SiteA, null);

        // Assert
        Assert.That(settings, Is.Null);
    }

    [Test]
    public async Task GetByContextAsync_ExactMatch_ReturnsRecord()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = SiteA,
            HostName = null,
            IsEnabled = true,
            IsReportOnly = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetByContextAsync(SiteA, null);

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.IsReportOnly, Is.True);
    }

    [Test]
    public async Task SaveAsync_CalledTwiceForSameContext_UpdatesNotInserts()
    {
        // Arrange
        var firstModel = new Mock<ICspSettings>();
        firstModel.Setup(x => x.IsEnabled).Returns(false);
        firstModel.Setup(x => x.IsReportOnly).Returns(false);

        var secondModel = new Mock<ICspSettings>();
        secondModel.Setup(x => x.IsEnabled).Returns(true);
        secondModel.Setup(x => x.IsReportOnly).Returns(true);

        // Act
        await _repository.SaveAsync(firstModel.Object, SiteA, null, "user.one");
        await _repository.SaveAsync(secondModel.Object, SiteA, null, "user.two");

        // Assert
        Assert.That(_inMemoryDatabase.CspSettings.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task DeleteByContextAsync_GivenNullSiteId_DoesNothing()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = null,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = SiteA,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.DeleteByContextAsync(null, null, "user");

        // Assert
        Assert.That(_inMemoryDatabase.CspSettings.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteByContextAsync_RemovesOnlyMatchingContext()
    {
        // Arrange
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = null,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = SiteA,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.CspSettings.Add(new CspSettings
        {
            Id = Guid.NewGuid(),
            SiteId = SiteB,
            HostName = null,
            IsEnabled = true
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.DeleteByContextAsync(SiteA, null, "user");

        // Assert
        var remaining = _inMemoryDatabase.CspSettings.AsNoTracking().ToList();
        Assert.That(remaining.Count, Is.EqualTo(2));
        Assert.That(remaining.Any(x => x.SiteId == null), Is.True);
        Assert.That(remaining.Any(x => x.SiteId == SiteB), Is.True);
        Assert.That(remaining.Any(x => x.SiteId == SiteA), Is.False);
    }
}