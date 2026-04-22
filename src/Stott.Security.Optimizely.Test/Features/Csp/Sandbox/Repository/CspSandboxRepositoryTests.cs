namespace Stott.Security.Optimizely.Test.Features.Csp.Sandbox.Repository;

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

[TestFixture]
public sealed class CspSandboxRepositoryTests
{
    private static readonly Guid SiteA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly Guid SiteB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private TestDataContext _inMemoryDatabase;

    private Lazy<ICspDataContext> _lazyInMemoryDatabase;

    private CspSandboxRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _lazyInMemoryDatabase = new Lazy<ICspDataContext>(() => _inMemoryDatabase);

        _repository = new CspSandboxRepository(_lazyInMemoryDatabase);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _inMemoryDatabase.Reset();
    }

    [Test]
    public async Task GetAsync_GivenThereIsNoSavedSandbox_ThenDefaultSandboxShouldBeReturned()
    {
        // Act
        var sandbox = await _repository.GetAsync(null, null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sandbox, Is.Not.Null);
            Assert.That(sandbox.IsSandboxEnabled, Is.False);
            Assert.That(sandbox.IsAllowDownloadsEnabled, Is.False);
            Assert.That(sandbox.IsAllowDownloadsWithoutGestureEnabled, Is.False);
            Assert.That(sandbox.IsAllowFormsEnabled, Is.False);
            Assert.That(sandbox.IsAllowModalsEnabled, Is.False);
            Assert.That(sandbox.IsAllowOrientationLockEnabled, Is.False);
            Assert.That(sandbox.IsAllowPointerLockEnabled, Is.False);
            Assert.That(sandbox.IsAllowPopupsEnabled, Is.False);
            Assert.That(sandbox.IsAllowPopupsToEscapeTheSandboxEnabled, Is.False);
            Assert.That(sandbox.IsAllowPresentationEnabled, Is.False);
            Assert.That(sandbox.IsAllowSameOriginEnabled, Is.False);
            Assert.That(sandbox.IsAllowScriptsEnabled, Is.False);
            Assert.That(sandbox.IsAllowStorageAccessByUserEnabled, Is.False);
            Assert.That(sandbox.IsAllowTopNavigationEnabled, Is.False);
            Assert.That(sandbox.IsAllowTopNavigationByUserEnabled, Is.False);
            Assert.That(sandbox.IsAllowTopNavigationToCustomProtocolEnabled, Is.False);
        });
    }

    [Test]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true)]
    public async Task GetAsync_GivenThereIsASavedSandbox_ThenThenTheSandboxShouldBeMappedToTheModel(
            bool isSandboxEnabled,
            bool isAllowDownloadsEnabled,
            bool isAllowDownloadsWithoutGestureEnabled,
            bool isAllowFormsEnabled,
            bool isAllowModalsEnabled,
            bool isAllowOrientationLockEnabled,
            bool isAllowPointerLockEnabled,
            bool isAllowPopupsEnabled,
            bool isAllowPopupsToEscapeTheSandboxEnabled,
            bool isAllowPresentationEnabled,
            bool isAllowSameOriginEnabled,
            bool isAllowScriptsEnabled,
            bool isAllowStorageAccessByUserEnabled,
            bool isAllowTopNavigationEnabled,
            bool isAllowTopNavigationByUserEnabled,
            bool isAllowTopNavigationToCustomProtocolEnabled)
    {
        // Arrange
        var fakeSandbox = new CspSandbox
        {
            IsSandboxEnabled = isSandboxEnabled,
            IsAllowDownloadsEnabled = isAllowDownloadsEnabled,
            IsAllowDownloadsWithoutGestureEnabled = isAllowDownloadsWithoutGestureEnabled,
            IsAllowFormsEnabled = isAllowFormsEnabled,
            IsAllowModalsEnabled = isAllowModalsEnabled,
            IsAllowOrientationLockEnabled = isAllowOrientationLockEnabled,
            IsAllowPointerLockEnabled = isAllowPointerLockEnabled,
            IsAllowPopupsEnabled = isAllowPopupsEnabled,
            IsAllowPopupsToEscapeTheSandboxEnabled = isAllowPopupsToEscapeTheSandboxEnabled,
            IsAllowPresentationEnabled = isAllowPresentationEnabled,
            IsAllowSameOriginEnabled = isAllowSameOriginEnabled,
            IsAllowScriptsEnabled = isAllowScriptsEnabled,
            IsAllowStorageAccessByUserEnabled = isAllowStorageAccessByUserEnabled,
            IsAllowTopNavigationEnabled = isAllowTopNavigationEnabled,
            IsAllowTopNavigationByUserEnabled = isAllowTopNavigationByUserEnabled,
            IsAllowTopNavigationToCustomProtocolEnabled = isAllowTopNavigationToCustomProtocolEnabled
        };

        _inMemoryDatabase.CspSandboxes.AddRange(fakeSandbox);
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetAsync(null, null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sandbox, Is.Not.Null);
            Assert.That(sandbox.IsSandboxEnabled, Is.EqualTo(isSandboxEnabled));
            Assert.That(sandbox.IsAllowDownloadsEnabled, Is.EqualTo(isAllowDownloadsEnabled));
            Assert.That(sandbox.IsAllowDownloadsWithoutGestureEnabled, Is.EqualTo(isAllowDownloadsWithoutGestureEnabled));
            Assert.That(sandbox.IsAllowFormsEnabled, Is.EqualTo(isAllowFormsEnabled));
            Assert.That(sandbox.IsAllowModalsEnabled, Is.EqualTo(isAllowModalsEnabled));
            Assert.That(sandbox.IsAllowOrientationLockEnabled, Is.EqualTo(isAllowOrientationLockEnabled));
            Assert.That(sandbox.IsAllowPointerLockEnabled, Is.EqualTo(isAllowPointerLockEnabled));
            Assert.That(sandbox.IsAllowPopupsEnabled, Is.EqualTo(isAllowPopupsEnabled));
            Assert.That(sandbox.IsAllowPopupsToEscapeTheSandboxEnabled, Is.EqualTo(isAllowPopupsToEscapeTheSandboxEnabled));
            Assert.That(sandbox.IsAllowPresentationEnabled, Is.EqualTo(isAllowPresentationEnabled));
            Assert.That(sandbox.IsAllowSameOriginEnabled, Is.EqualTo(isAllowSameOriginEnabled));
            Assert.That(sandbox.IsAllowScriptsEnabled, Is.EqualTo(isAllowScriptsEnabled));
            Assert.That(sandbox.IsAllowStorageAccessByUserEnabled, Is.EqualTo(isAllowStorageAccessByUserEnabled));
            Assert.That(sandbox.IsAllowTopNavigationEnabled, Is.EqualTo(isAllowTopNavigationEnabled));
            Assert.That(sandbox.IsAllowTopNavigationByUserEnabled, Is.EqualTo(isAllowTopNavigationByUserEnabled));
            Assert.That(sandbox.IsAllowTopNavigationToCustomProtocolEnabled, Is.EqualTo(isAllowTopNavigationToCustomProtocolEnabled));
        });
    }

    [Test]
    public async Task GetAsync_GivenThereAreMultipleSavedSandboxesAtTheSameScope_ThenOneOfThemIsReturned()
    {
        // As of v6.0.0 the fallback chain ranks candidates by (SiteId, HostName) specificity; when
        // multiple records exist at the same scope (e.g. two Global rows) the winning row is
        // implementation-defined. The upsert path prevents duplicates at a given context in practice,
        // but this test pins the resilience behaviour when duplicates are present.
        // Arrange
        var fakeSandboxTwo = new CspSandbox();
        var fakeSandboxOne = new CspSandbox
        {
            IsSandboxEnabled = true,
            IsAllowDownloadsEnabled = true,
            IsAllowDownloadsWithoutGestureEnabled = true,
            IsAllowFormsEnabled = true
        };

        _inMemoryDatabase.CspSandboxes.AddRange(fakeSandboxOne, fakeSandboxTwo);
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetAsync(null, null);

        // Assert — the repository must return either of the two persisted sandboxes,
        // with every boolean matching that chosen record.
        Assert.That(sandbox, Is.Not.Null);
        var matchesSandboxOne =
            sandbox.IsSandboxEnabled == fakeSandboxOne.IsSandboxEnabled &&
            sandbox.IsAllowDownloadsEnabled == fakeSandboxOne.IsAllowDownloadsEnabled &&
            sandbox.IsAllowDownloadsWithoutGestureEnabled == fakeSandboxOne.IsAllowDownloadsWithoutGestureEnabled &&
            sandbox.IsAllowFormsEnabled == fakeSandboxOne.IsAllowFormsEnabled;
        var matchesSandboxTwo =
            sandbox.IsSandboxEnabled == fakeSandboxTwo.IsSandboxEnabled &&
            sandbox.IsAllowDownloadsEnabled == fakeSandboxTwo.IsAllowDownloadsEnabled &&
            sandbox.IsAllowDownloadsWithoutGestureEnabled == fakeSandboxTwo.IsAllowDownloadsWithoutGestureEnabled &&
            sandbox.IsAllowFormsEnabled == fakeSandboxTwo.IsAllowFormsEnabled;
        Assert.That(matchesSandboxOne || matchesSandboxTwo, Is.True,
            "Expected the repository to return one of the two seeded sandbox records.");
    }

    [Test]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false)]
    [TestCase(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true)]
    public async Task SaveAsync_GivenThereIsASavedSandbox_ThenThenTheSandboxShouldBeMappedToTheModel(
            bool isSandboxEnabled,
            bool isAllowDownloadsEnabled,
            bool isAllowDownloadsWithoutGestureEnabled,
            bool isAllowFormsEnabled,
            bool isAllowModalsEnabled,
            bool isAllowOrientationLockEnabled,
            bool isAllowPointerLockEnabled,
            bool isAllowPopupsEnabled,
            bool isAllowPopupsToEscapeTheSandboxEnabled,
            bool isAllowPresentationEnabled,
            bool isAllowSameOriginEnabled,
            bool isAllowScriptsEnabled,
            bool isAllowStorageAccessByUserEnabled,
            bool isAllowTopNavigationEnabled,
            bool isAllowTopNavigationByUserEnabled,
            bool isAllowTopNavigationToCustomProtocolEnabled)
    {
        // Arrange
        var model = new SandboxModel
        {
            IsSandboxEnabled = isSandboxEnabled,
            IsAllowDownloadsEnabled = isAllowDownloadsEnabled,
            IsAllowDownloadsWithoutGestureEnabled = isAllowDownloadsWithoutGestureEnabled,
            IsAllowFormsEnabled = isAllowFormsEnabled,
            IsAllowModalsEnabled = isAllowModalsEnabled,
            IsAllowOrientationLockEnabled = isAllowOrientationLockEnabled,
            IsAllowPointerLockEnabled = isAllowPointerLockEnabled,
            IsAllowPopupsEnabled = isAllowPopupsEnabled,
            IsAllowPopupsToEscapeTheSandboxEnabled = isAllowPopupsToEscapeTheSandboxEnabled,
            IsAllowPresentationEnabled = isAllowPresentationEnabled,
            IsAllowSameOriginEnabled = isAllowSameOriginEnabled,
            IsAllowScriptsEnabled = isAllowScriptsEnabled,
            IsAllowStorageAccessByUserEnabled = isAllowStorageAccessByUserEnabled,
            IsAllowTopNavigationEnabled = isAllowTopNavigationEnabled,
            IsAllowTopNavigationByUserEnabled = isAllowTopNavigationByUserEnabled,
            IsAllowTopNavigationToCustomProtocolEnabled = isAllowTopNavigationToCustomProtocolEnabled
        };

        // Act
        await _repository.SaveAsync(model, "fake.user", null, null);

        var savedValue = await _inMemoryDatabase.CspSandboxes.FirstOrDefaultAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(savedValue, Is.Not.Null);
            Assert.That(savedValue.IsSandboxEnabled, Is.EqualTo(isSandboxEnabled));
            Assert.That(savedValue.IsAllowDownloadsEnabled, Is.EqualTo(isAllowDownloadsEnabled));
            Assert.That(savedValue.IsAllowDownloadsWithoutGestureEnabled, Is.EqualTo(isAllowDownloadsWithoutGestureEnabled));
            Assert.That(savedValue.IsAllowFormsEnabled, Is.EqualTo(isAllowFormsEnabled));
            Assert.That(savedValue.IsAllowModalsEnabled, Is.EqualTo(isAllowModalsEnabled));
            Assert.That(savedValue.IsAllowOrientationLockEnabled, Is.EqualTo(isAllowOrientationLockEnabled));
            Assert.That(savedValue.IsAllowPointerLockEnabled, Is.EqualTo(isAllowPointerLockEnabled));
            Assert.That(savedValue.IsAllowPopupsEnabled, Is.EqualTo(isAllowPopupsEnabled));
            Assert.That(savedValue.IsAllowPopupsToEscapeTheSandboxEnabled, Is.EqualTo(isAllowPopupsToEscapeTheSandboxEnabled));
            Assert.That(savedValue.IsAllowPresentationEnabled, Is.EqualTo(isAllowPresentationEnabled));
            Assert.That(savedValue.IsAllowSameOriginEnabled, Is.EqualTo(isAllowSameOriginEnabled));
            Assert.That(savedValue.IsAllowScriptsEnabled, Is.EqualTo(isAllowScriptsEnabled));
            Assert.That(savedValue.IsAllowStorageAccessByUserEnabled, Is.EqualTo(isAllowStorageAccessByUserEnabled));
            Assert.That(savedValue.IsAllowTopNavigationEnabled, Is.EqualTo(isAllowTopNavigationEnabled));
            Assert.That(savedValue.IsAllowTopNavigationByUserEnabled, Is.EqualTo(isAllowTopNavigationByUserEnabled));
            Assert.That(savedValue.IsAllowTopNavigationToCustomProtocolEnabled, Is.EqualTo(isAllowTopNavigationToCustomProtocolEnabled));
        });
    }

    [Test]
    public async Task GetAsync_OnlyGlobalExists_ReturnsGlobal()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), IsSandboxEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetAsync(SiteA, "host.com");

        // Assert
        Assert.That(sandbox, Is.Not.Null);
        Assert.That(sandbox.IsSandboxEnabled, Is.True);
    }

    [Test]
    public async Task GetAsync_SiteAndGlobalExist_ReturnsSite()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid() });
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), SiteId = SiteA, IsSandboxEnabled = true, IsAllowScriptsEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetAsync(SiteA, null);

        // Assert
        Assert.That(sandbox, Is.Not.Null);
        Assert.That(sandbox.IsSandboxEnabled, Is.True);
        Assert.That(sandbox.IsAllowScriptsEnabled, Is.True);
    }

    [Test]
    public async Task GetAsync_HostSiteAndGlobalExist_ReturnsHost()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid() });
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), SiteId = SiteA });
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), SiteId = SiteA, HostName = "www.example.com", IsAllowFormsEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetAsync(SiteA, "www.example.com");

        // Assert
        Assert.That(sandbox, Is.Not.Null);
        Assert.That(sandbox.IsAllowFormsEnabled, Is.True);
    }

    [Test]
    public async Task GetAsync_SiteIdSuppliedButNoSiteRecord_FallsBackToGlobal()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), IsSandboxEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetAsync(SiteA, null);

        // Assert
        Assert.That(sandbox, Is.Not.Null);
        Assert.That(sandbox.IsSandboxEnabled, Is.True);
    }

    [Test]
    public async Task GetAsync_HostNameSuppliedButNoHostRecord_FallsBackToSite()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid() });
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), SiteId = SiteA, IsSandboxEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetAsync(SiteA, "www.example.com");

        // Assert
        Assert.That(sandbox, Is.Not.Null);
        Assert.That(sandbox.IsSandboxEnabled, Is.True);
    }

    [Test]
    public async Task GetAsync_NullSiteIdWithOtherSiteRecordsPresent_StillReturnsGlobal()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid() });
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), SiteId = SiteA, IsSandboxEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetAsync(null, null);

        // Assert
        Assert.That(sandbox, Is.Not.Null);
        Assert.That(sandbox.IsSandboxEnabled, Is.False);
    }

    [Test]
    public async Task GetByContextAsync_NoExactMatch_ReturnsNull()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), IsSandboxEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetByContextAsync(SiteA, null);

        // Assert
        Assert.That(sandbox, Is.Null);
    }

    [Test]
    public async Task GetByContextAsync_ExactMatch_ReturnsRecord()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), SiteId = SiteA, IsSandboxEnabled = true, IsAllowScriptsEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetByContextAsync(SiteA, null);

        // Assert
        Assert.That(sandbox, Is.Not.Null);
        Assert.That(sandbox.IsSandboxEnabled, Is.True);
        Assert.That(sandbox.IsAllowScriptsEnabled, Is.True);
    }

    [Test]
    public async Task SaveAsync_CalledTwiceForSameContext_UpdatesNotInserts()
    {
        // Arrange
        var firstModel = new SandboxModel { IsSandboxEnabled = false };
        var secondModel = new SandboxModel { IsSandboxEnabled = true };

        // Act
        await _repository.SaveAsync(firstModel, "user.one", SiteA, null);
        await _repository.SaveAsync(secondModel, "user.two", SiteA, null);

        // Assert
        Assert.That(_inMemoryDatabase.CspSandboxes.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task DeleteByContextAsync_GivenNullSiteId_DoesNothing()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), IsSandboxEnabled = true });
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), SiteId = SiteA, IsSandboxEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.DeleteByContextAsync(null, null, "user");

        // Assert
        Assert.That(_inMemoryDatabase.CspSandboxes.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteByContextAsync_RemovesOnlyMatchingContext()
    {
        // Arrange
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), IsSandboxEnabled = true });
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), SiteId = SiteA, IsSandboxEnabled = true });
        _inMemoryDatabase.CspSandboxes.Add(new CspSandbox { Id = Guid.NewGuid(), SiteId = SiteB, IsSandboxEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.DeleteByContextAsync(SiteA, null, "user");

        // Assert
        var remaining = _inMemoryDatabase.CspSandboxes.AsNoTracking().ToList();
        Assert.That(remaining.Count, Is.EqualTo(2));
        Assert.That(remaining.Any(x => x.SiteId == null), Is.True);
        Assert.That(remaining.Any(x => x.SiteId == SiteB), Is.True);
        Assert.That(remaining.Any(x => x.SiteId == SiteA), Is.False);
    }
}