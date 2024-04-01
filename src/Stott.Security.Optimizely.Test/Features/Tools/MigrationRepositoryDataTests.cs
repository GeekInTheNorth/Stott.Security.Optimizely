using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Features.Tools;

namespace Stott.Security.Optimizely.Test.Features.Tools;

[TestFixture]
public sealed class MigrationRepositoryDataTests
{
    private TestDataContext _inMemoryDatabase;

    private MigrationRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _repository = new MigrationRepository(_inMemoryDatabase);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _inMemoryDatabase.Reset();
    }

    [Test]
    public async Task GivenSettingsDoesNotContainACsp_AndTheDataRecordDoesNotExist_ThenNoRecordIsUpserted()
    {
        // Arrange
        var settings = new SettingsModel();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var settingsRecords = await _inMemoryDatabase.CspSettings.CountAsync();

        // Assert
        Assert.That(settingsRecords, Is.EqualTo(0));
    }

    [Test]
    public async Task GivenSettingsDoesNotContainACsp_AndTheDataRecordDoesExist_ThenNoRecordIsUpserted()
    {
        // Arrange
        var settings = new SettingsModel();
        var existingEntity = new CspSettings { Id = Guid.Empty };

        _inMemoryDatabase.CspSettings.Add(existingEntity);
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        // Assert
        Assert.That(_inMemoryDatabase.RecordsUpdated.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GivenSettingsDoesContainACsp_AndTheDataRecordDoesNotExist_ThenARecordIsCreated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = true,
                IsReportOnly = true,
                IsAllowListEnabled = true,
                AllowListUrl = "https://www.example.com/one/",
                IsUpgradeInsecureRequestsEnabled = true,
                IsNonceEnabled = true,
                IsStrictDynamicEnabled = true,
                UseInternalReporting = true,
                UseExternalReporting = true,
                ExternalReportToUrl = "https://www.example.com/two/",
                ExternalReportUriUrl = "https://www.example.com/three/"
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var updatedRecord = await _inMemoryDatabase.CspSettings.FirstOrDefaultAsync();

        // Assert
        Assert.That(updatedRecord, Is.Not.Null);
        Assert.That(updatedRecord.IsEnabled, Is.EqualTo(settings.Csp.IsEnabled));
        Assert.That(updatedRecord.IsReportOnly, Is.EqualTo(settings.Csp.IsReportOnly));
        Assert.That(updatedRecord.IsAllowListEnabled, Is.EqualTo(settings.Csp.IsAllowListEnabled));
        Assert.That(updatedRecord.AllowListUrl, Is.EqualTo(settings.Csp.AllowListUrl));
        Assert.That(updatedRecord.IsUpgradeInsecureRequestsEnabled, Is.EqualTo(settings.Csp.IsUpgradeInsecureRequestsEnabled));
        Assert.That(updatedRecord.IsNonceEnabled, Is.EqualTo(settings.Csp.IsNonceEnabled));
        Assert.That(updatedRecord.IsStrictDynamicEnabled, Is.EqualTo(settings.Csp.IsStrictDynamicEnabled));
        Assert.That(updatedRecord.UseInternalReporting, Is.EqualTo(settings.Csp.UseInternalReporting));
        Assert.That(updatedRecord.UseExternalReporting, Is.EqualTo(settings.Csp.UseExternalReporting));
        Assert.That(updatedRecord.ExternalReportToUrl, Is.EqualTo(settings.Csp.ExternalReportToUrl));
        Assert.That(updatedRecord.ExternalReportUriUrl, Is.EqualTo(settings.Csp.ExternalReportUriUrl));
    }

    [Test]
    public async Task GivenSettingsDoesContainACsp_AndTheDataRecordDoesExist_ThenARecordIsUpdated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = true,
                IsReportOnly = true,
                IsAllowListEnabled = true,
                AllowListUrl = "https://www.example.com/one/",
                IsUpgradeInsecureRequestsEnabled = true,
                IsNonceEnabled = true,
                IsStrictDynamicEnabled = true,
                UseInternalReporting = true,
                UseExternalReporting = true,
                ExternalReportToUrl = "https://www.example.com/two/",
                ExternalReportUriUrl = "https://www.example.com/three/"
            }
        };

        var existingEntity = new CspSettings { Id = Guid.Empty };

        _inMemoryDatabase.CspSettings.Add(existingEntity);
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var recordCount = await _inMemoryDatabase.CspSettings.CountAsync();
        var updatedRecord = await _inMemoryDatabase.CspSettings.FirstOrDefaultAsync();

        // Assert
        Assert.That(recordCount, Is.EqualTo(1));
        Assert.That(updatedRecord, Is.Not.Null);
        Assert.That(updatedRecord.IsEnabled, Is.EqualTo(settings.Csp.IsEnabled));
        Assert.That(updatedRecord.IsReportOnly, Is.EqualTo(settings.Csp.IsReportOnly));
        Assert.That(updatedRecord.IsAllowListEnabled, Is.EqualTo(settings.Csp.IsAllowListEnabled));
        Assert.That(updatedRecord.AllowListUrl, Is.EqualTo(settings.Csp.AllowListUrl));
        Assert.That(updatedRecord.IsUpgradeInsecureRequestsEnabled, Is.EqualTo(settings.Csp.IsUpgradeInsecureRequestsEnabled));
        Assert.That(updatedRecord.IsNonceEnabled, Is.EqualTo(settings.Csp.IsNonceEnabled));
        Assert.That(updatedRecord.IsStrictDynamicEnabled, Is.EqualTo(settings.Csp.IsStrictDynamicEnabled));
        Assert.That(updatedRecord.UseInternalReporting, Is.EqualTo(settings.Csp.UseInternalReporting));
        Assert.That(updatedRecord.UseExternalReporting, Is.EqualTo(settings.Csp.UseExternalReporting));
        Assert.That(updatedRecord.ExternalReportToUrl, Is.EqualTo(settings.Csp.ExternalReportToUrl));
        Assert.That(updatedRecord.ExternalReportUriUrl, Is.EqualTo(settings.Csp.ExternalReportUriUrl));
    }

    [Test]
    [TestCase(true, true, true)]
    [TestCase(true, false, true)]
    [TestCase(false, false, false)]
    [TestCase(false, true, false)]
    public async Task GivenCspSettingsAreToBeCreated_ThenTheCspIsAlwaysSetToReportOnlyMode(
        bool isEnabled,
        bool reportOnlyMode,
        bool expectedReportOnlyMode)
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = isEnabled,
                IsReportOnly = reportOnlyMode,
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var createdRecord = await _inMemoryDatabase.CspSettings.FirstOrDefaultAsync();

        // Assert
        Assert.That(createdRecord, Is.Not.Null);
        Assert.That(createdRecord.IsReportOnly, Is.EqualTo(expectedReportOnlyMode));
    }

    [Test]
    [TestCase(true, true, true)]
    [TestCase(true, false, true)]
    [TestCase(false, false, false)]
    [TestCase(false, true, false)]
    public async Task GivenCspSettingsAreToBeUpdated_ThenTheCspIsAlwaysSetToReportOnlyMode(
        bool isEnabled,
        bool reportOnlyMode,
        bool expectedReportOnlyMode)
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = isEnabled,
                IsReportOnly = reportOnlyMode,
            }
        };

        var existingEntity = new CspSettings { Id = Guid.Empty };

        _inMemoryDatabase.CspSettings.Add(existingEntity);
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var updatedRecord = await _inMemoryDatabase.CspSettings.FirstOrDefaultAsync();

        // Assert
        Assert.That(updatedRecord, Is.Not.Null);
        Assert.That(updatedRecord.IsReportOnly, Is.EqualTo(expectedReportOnlyMode));
    }

    [Test]
    public async Task GivenSettingsDoesNotContainCspSandbox_AndTheDataRecordDoesNotExist_ThenARecordIsCreated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                Sandbox = null
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var recordCount = await _inMemoryDatabase.CspSandboxes.CountAsync();

        // Assert
        Assert.That(recordCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GivenSettingsDoesNotContainCspSandbox_AndTheDataRecordDoesExist_ThenARecordIsNotUpdated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                Sandbox = null
            }
        };

        var existingEntity = new CspSandbox { Id = Guid.Empty };

        _inMemoryDatabase.CspSandboxes.Add(existingEntity);
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var changesMade = _inMemoryDatabase.RecordsUpdated.Count(x => x.Equals(nameof(CspSandbox)));

        // Assert
        Assert.That(changesMade, Is.EqualTo(0));
    }

    [Test]
    public async Task GivenSettingsDoesContainACspSandbox_AndTheDataRecordDoesNotExist_ThenARecordIsCreated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                Sandbox = new SandboxModel()
            }
        };

        
        var originalCount = await _inMemoryDatabase.CspSandboxes.CountAsync();
        
        // Act
        await _repository.SaveAsync(settings, "Test User");

        var updatedCount = await _inMemoryDatabase.CspSandboxes.CountAsync();

        // Assert
        Assert.That(originalCount, Is.EqualTo(0));
        Assert.That(updatedCount, Is.EqualTo(1));
    }

    [Test]
    public async Task GivenSettingsDoesContainACspSandbox_AndTheDataRecordDoesExist_ThenARecordIsUpdated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                Sandbox = new SandboxModel
                {
                    IsSandboxEnabled = true,
                    IsAllowDownloadsEnabled = true,
                    IsAllowDownloadsWithoutGestureEnabled = true,
                    IsAllowFormsEnabled = true,
                    IsAllowModalsEnabled = true,
                    IsAllowOrientationLockEnabled = true,
                    IsAllowPointerLockEnabled = true,
                    IsAllowPopupsEnabled = true,
                    IsAllowPopupsToEscapeTheSandboxEnabled = true,
                    IsAllowPresentationEnabled = true,
                    IsAllowSameOriginEnabled = true,
                    IsAllowScriptsEnabled = true,
                    IsAllowStorageAccessByUserEnabled = true,
                    IsAllowTopNavigationEnabled = true,
                    IsAllowTopNavigationByUserEnabled = true,
                    IsAllowTopNavigationToCustomProtocolEnabled = true
                }
            }
        };

        var existingEntity = new CspSandbox { Id = Guid.Empty };

        _inMemoryDatabase.CspSandboxes.Add(existingEntity);
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var totalRecords = await _inMemoryDatabase.CspSandboxes.CountAsync();
        var sandboxRecord = await _inMemoryDatabase.CspSandboxes.OrderBy(x => x.Id).FirstOrDefaultAsync();

        // Assert
        Assert.That(totalRecords, Is.EqualTo(1));
        Assert.That(sandboxRecord, Is.Not.Null);
        Assert.That(sandboxRecord.IsSandboxEnabled, Is.EqualTo(settings.Csp.Sandbox.IsSandboxEnabled));
        Assert.That(sandboxRecord.IsAllowDownloadsEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowDownloadsEnabled));
        Assert.That(sandboxRecord.IsAllowDownloadsWithoutGestureEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowDownloadsWithoutGestureEnabled));
        Assert.That(sandboxRecord.IsAllowFormsEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowFormsEnabled));
        Assert.That(sandboxRecord.IsAllowModalsEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowModalsEnabled));
        Assert.That(sandboxRecord.IsAllowOrientationLockEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowOrientationLockEnabled));
        Assert.That(sandboxRecord.IsAllowPointerLockEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowPointerLockEnabled));
        Assert.That(sandboxRecord.IsAllowPopupsEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowPopupsEnabled));
        Assert.That(sandboxRecord.IsAllowPopupsToEscapeTheSandboxEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowPopupsToEscapeTheSandboxEnabled));
        Assert.That(sandboxRecord.IsAllowPresentationEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowPresentationEnabled));
        Assert.That(sandboxRecord.IsAllowSameOriginEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowSameOriginEnabled));
        Assert.That(sandboxRecord.IsAllowScriptsEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowScriptsEnabled));
        Assert.That(sandboxRecord.IsAllowStorageAccessByUserEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowStorageAccessByUserEnabled));
        Assert.That(sandboxRecord.IsAllowTopNavigationEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowTopNavigationEnabled));
        Assert.That(sandboxRecord.IsAllowTopNavigationByUserEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowTopNavigationByUserEnabled));
        Assert.That(sandboxRecord.IsAllowTopNavigationToCustomProtocolEnabled, Is.EqualTo(settings.Csp.Sandbox.IsAllowTopNavigationToCustomProtocolEnabled));
    }

    [Test]
    public async Task GivenThereAreNoCspSources_AndNoneExistInData_ThenNoChangesWillBeMade()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                Sources = new List<CspSourceModel>(0)
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var updatedCount = await _inMemoryDatabase.CspSandboxes.CountAsync();

        // Assert
        Assert.That(updatedCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GivenThereAreNoCspSources_AndSourcesExistInData_ThenSourcesWillBeDeleted()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                Sources = new List<CspSourceModel>(0)
            }
        };

        _inMemoryDatabase.CspSources.Add(new CspSource
        {
            Id = Guid.NewGuid(),
            Source = "https://www.example.com/One/",
            Directives = $"{CspConstants.Directives.DefaultSource}"
        });
        _inMemoryDatabase.CspSources.Add(new CspSource
        {
            Id = Guid.NewGuid(),
            Source = "https://www.example.com/Two/",
            Directives = $"{CspConstants.Directives.ScriptSource}"
        });
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var updatedCount = await _inMemoryDatabase.CspSources.CountAsync();

        // Assert
        Assert.That(updatedCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GivenThereAreCspSources_AndMatchingSourcesExistInData_ThenSourcesWillBeUpdated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                Sources =
                [
                    new()
                    {
                        Source = "https://www.example.com/One/",
                        Directives =
                        [
                            CspConstants.Directives.DefaultSource,
                            CspConstants.Directives.ScriptSource
                        ]
                    }
                ]
            }
        };

        _inMemoryDatabase.CspSources.Add(new CspSource
        {
            Id = Guid.NewGuid(),
            Source = "https://www.example.com/One/",
            Directives = $"{CspConstants.Directives.StyleSource}"
        });
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var totalRecords = await _inMemoryDatabase.CspSources.CountAsync();
        var updatedRecord = await _inMemoryDatabase.CspSources.FirstOrDefaultAsync();

        // Assert
        Assert.That(totalRecords, Is.EqualTo(1));
        Assert.That(updatedRecord, Is.Not.Null);
        Assert.That(updatedRecord.Source, Is.EqualTo("https://www.example.com/One/"));
        Assert.That(updatedRecord.Directives, Is.EqualTo("default-src,script-src"));
    }
}