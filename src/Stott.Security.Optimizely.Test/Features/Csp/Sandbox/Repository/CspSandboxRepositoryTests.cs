namespace Stott.Security.Optimizely.Test.Features.Csp.Sandbox.Repository;

using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;
using Stott.Security.Optimizely.Features.Csp.Sandbox;

[TestFixture]
public sealed class CspSandboxRepositoryTests
{
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
        var sandbox = await _repository.GetAsync();

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
        var sandbox = await _repository.GetAsync();

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
    public async Task GetAsync_GivenThereAreMultipleSavedSandboxes_ThenTheFirstSandboxShouldBeMappedToTheModel()
    {
        // Arrange
        var fakeSandboxOne = new CspSandbox
        {
            IsSandboxEnabled = true,
            IsAllowDownloadsEnabled = true,
            IsAllowDownloadsWithoutGestureEnabled = true,
            IsAllowFormsEnabled = true
        };
        var fakeSandboxTwo = new CspSandbox();

        _inMemoryDatabase.CspSandboxes.AddRange(fakeSandboxOne, fakeSandboxTwo);
        _inMemoryDatabase.SaveChanges();

        // Act
        var sandbox = await _repository.GetAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(sandbox, Is.Not.Null);
            Assert.That(sandbox.IsSandboxEnabled, Is.EqualTo(fakeSandboxOne.IsSandboxEnabled));
            Assert.That(sandbox.IsAllowDownloadsEnabled, Is.EqualTo(fakeSandboxOne.IsAllowDownloadsEnabled));
            Assert.That(sandbox.IsAllowDownloadsWithoutGestureEnabled, Is.EqualTo(fakeSandboxOne.IsAllowDownloadsWithoutGestureEnabled));
            Assert.That(sandbox.IsAllowFormsEnabled, Is.EqualTo(fakeSandboxOne.IsAllowFormsEnabled));
            Assert.That(sandbox.IsAllowModalsEnabled, Is.EqualTo(fakeSandboxOne.IsAllowModalsEnabled));
            Assert.That(sandbox.IsAllowOrientationLockEnabled, Is.EqualTo(fakeSandboxOne.IsAllowOrientationLockEnabled));
            Assert.That(sandbox.IsAllowPointerLockEnabled, Is.EqualTo(fakeSandboxOne.IsAllowPointerLockEnabled));
            Assert.That(sandbox.IsAllowPopupsEnabled, Is.EqualTo(fakeSandboxOne.IsAllowPopupsEnabled));
            Assert.That(sandbox.IsAllowPopupsToEscapeTheSandboxEnabled, Is.EqualTo(fakeSandboxOne.IsAllowPopupsToEscapeTheSandboxEnabled));
            Assert.That(sandbox.IsAllowPresentationEnabled, Is.EqualTo(fakeSandboxOne.IsAllowPresentationEnabled));
            Assert.That(sandbox.IsAllowSameOriginEnabled, Is.EqualTo(fakeSandboxOne.IsAllowSameOriginEnabled));
            Assert.That(sandbox.IsAllowScriptsEnabled, Is.EqualTo(fakeSandboxOne.IsAllowScriptsEnabled));
            Assert.That(sandbox.IsAllowStorageAccessByUserEnabled, Is.EqualTo(fakeSandboxOne.IsAllowStorageAccessByUserEnabled));
            Assert.That(sandbox.IsAllowTopNavigationEnabled, Is.EqualTo(fakeSandboxOne.IsAllowTopNavigationEnabled));
            Assert.That(sandbox.IsAllowTopNavigationByUserEnabled, Is.EqualTo(fakeSandboxOne.IsAllowTopNavigationByUserEnabled));
            Assert.That(sandbox.IsAllowTopNavigationToCustomProtocolEnabled, Is.EqualTo(fakeSandboxOne.IsAllowTopNavigationToCustomProtocolEnabled));
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
        await _repository.SaveAsync(model, "fake.user");

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
}