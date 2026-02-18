using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.Tools;

namespace Stott.Security.Optimizely.Test.Features.Tools;

[TestFixture]
public sealed class MigrationRepositoryDataTests
{
    private TestDataContext _inMemoryDatabase;

    private Lazy<ICspDataContext> _lazyInMemoryDatabase;

    private MigrationRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _lazyInMemoryDatabase = new Lazy<ICspDataContext>(() => _inMemoryDatabase);

        _repository = new MigrationRepository(_lazyInMemoryDatabase);
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
                ExternalReportToUrl = "https://www.example.com/two/"
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
        Assert.That(updatedRecord.IsNonceEnabled, Is.False);
        Assert.That(updatedRecord.IsStrictDynamicEnabled, Is.False);
        Assert.That(updatedRecord.UseInternalReporting, Is.EqualTo(settings.Csp.UseInternalReporting));
        Assert.That(updatedRecord.UseExternalReporting, Is.EqualTo(settings.Csp.UseExternalReporting));
        Assert.That(updatedRecord.ExternalReportToUrl, Is.EqualTo(settings.Csp.ExternalReportToUrl));
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
                ExternalReportToUrl = "https://www.example.com/two/"
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
        Assert.That(updatedRecord.IsNonceEnabled, Is.False);
        Assert.That(updatedRecord.IsStrictDynamicEnabled, Is.False);
        Assert.That(updatedRecord.UseInternalReporting, Is.EqualTo(settings.Csp.UseInternalReporting));
        Assert.That(updatedRecord.UseExternalReporting, Is.EqualTo(settings.Csp.UseExternalReporting));
        Assert.That(updatedRecord.ExternalReportToUrl, Is.EqualTo(settings.Csp.ExternalReportToUrl));
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

    [Test]
    public async Task GivenThereAreCspSources_AndNonMatchingSourcesExistInData_ThenANewSourceWillBeCreatedAnNonMatchingWillBeDeleted()
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
            Source = "https://www.example.com/Two/",
            Directives = $"{CspConstants.Directives.StyleSource}"
        });
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var records = await _inMemoryDatabase.CspSources.ToListAsync();

        // Assert
        Assert.That(records, Has.Count.EqualTo(1));
        Assert.That(records[0].Source, Is.EqualTo("https://www.example.com/One/"));
        Assert.That(records[0].Directives, Is.EqualTo("default-src,script-src"));
    }

    [Test]
    public async Task GivenThereAreNoCorsSettings_AndNoCorsRecordExists_ThenDataWillNotBeUpserted()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Cors = null
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var corsRecords = await _inMemoryDatabase.CorsSettings.CountAsync();

        // Assert
        Assert.That(corsRecords, Is.EqualTo(0));
    }

    [Test]
    public async Task GivenThereAreNoCorsSettings_AndCorsRecordsExist_ThenDataWillNotBeUpserted()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Cors = null
        };

        var existingEntity = new CorsSettings { Id = Guid.Empty };

        _inMemoryDatabase.CorsSettings.Add(existingEntity);
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var changesMade = _inMemoryDatabase.RecordsUpdated.Count(x => x.Equals(nameof(CorsSettings)));

        // Assert
        Assert.That(changesMade, Is.EqualTo(0));
    }

    [Test]
    public async Task GivenThereAreAndCorsSettings_AndCorsRecordsDoNotExist_ThenDataBeCreated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Cors = new CorsConfiguration
            {
                IsEnabled = true,
                AllowMethods = new CorsConfigurationMethods { IsAllowGetMethods = true },
                AllowOrigins = [ new() { Id = Guid.NewGuid(), Value = "https://www.example.com" } ],
                AllowHeaders = [ new() { Id = Guid.NewGuid(), Value = "Allow-Header" } ],
                ExposeHeaders = [ new() { Id = Guid.NewGuid(), Value = "Expose-Header" } ],
                MaxAge = 10,
                AllowCredentials = true
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var corsRecord = await _inMemoryDatabase.CorsSettings.FirstOrDefaultAsync();

        // Assert
        Assert.That(corsRecord, Is.Not.Null);
        Assert.That(corsRecord.IsEnabled, Is.EqualTo(settings.Cors.IsEnabled));
        Assert.That(corsRecord.AllowCredentials, Is.EqualTo(settings.Cors.AllowCredentials));
        Assert.That(corsRecord.AllowHeaders, Is.EqualTo("Allow-Header"));
        Assert.That(corsRecord.ExposeHeaders, Is.EqualTo("Expose-Header"));
        Assert.That(corsRecord.AllowMethods, Is.EqualTo("GET"));
        Assert.That(corsRecord.MaxAge, Is.EqualTo(settings.Cors.MaxAge));
    }

    [Test]
    public async Task GivenThereAreAndCorsSettings_AndCorsRecordsDoExist_ThenDataBeCreated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            Cors = new CorsConfiguration
            {
                IsEnabled = true,
                AllowMethods = new CorsConfigurationMethods { IsAllowGetMethods = true, IsAllowPostMethods = true },
                AllowOrigins = [
                    new() { Id = Guid.NewGuid(), Value = "https://www.example.com" },
                    new() { Id = Guid.NewGuid(), Value = "https://www.test.com" }
                ],
                AllowHeaders = [
                    new() { Id = Guid.NewGuid(), Value = "First-Allow-Header" },
                    new() { Id = Guid.NewGuid(), Value = "Second-Allow-Header" }
                ],
                ExposeHeaders = [
                    new() { Id = Guid.NewGuid(), Value = "First-Expose-Header" },
                    new() { Id = Guid.NewGuid(), Value = "Second-Expose-Header" }
                ],
                MaxAge = 1000,
                AllowCredentials = false
            }
        };

        var existingEntity = new CorsSettings
        { 
            Id = Guid.Empty,
            IsEnabled = true,
            AllowCredentials = true,
            AllowHeaders = "Existing-Allow-Header",
            ExposeHeaders = "Existing-Expose-Header",
            AllowMethods = "GET",
            MaxAge = settings.Cors.MaxAge,
        };

        _inMemoryDatabase.CorsSettings.Add(existingEntity);
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var corsRecord = await _inMemoryDatabase.CorsSettings.FirstOrDefaultAsync();

        // Assert
        Assert.That(corsRecord, Is.Not.Null);
        Assert.That(corsRecord.IsEnabled, Is.EqualTo(settings.Cors.IsEnabled));
        Assert.That(corsRecord.AllowCredentials, Is.EqualTo(settings.Cors.AllowCredentials));
        Assert.That(corsRecord.AllowOrigins, Is.EqualTo("https://www.example.com,https://www.test.com"));
        Assert.That(corsRecord.AllowHeaders, Is.EqualTo("First-Allow-Header,Second-Allow-Header"));
        Assert.That(corsRecord.ExposeHeaders, Is.EqualTo("First-Expose-Header,Second-Expose-Header"));
        Assert.That(corsRecord.AllowMethods, Is.EqualTo("GET,POST"));
        Assert.That(corsRecord.MaxAge, Is.EqualTo(settings.Cors.MaxAge));
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public async Task GivenThereAreSettingsAndNullSources_WhenImporting_ThenNonceAndStrictDynamicAreAlwaysUnset(bool strictDynamic, bool nonce)
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = true,
                IsNonceEnabled = nonce,
                IsStrictDynamicEnabled = strictDynamic,
                Sources = null
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");
        var createdSettings = await _inMemoryDatabase.CspSettings.FirstOrDefaultAsync();
        var createdSources = await _inMemoryDatabase.CspSources.ToListAsync();

        // Assert
        Assert.That(createdSettings, Is.Not.Null);
        Assert.That(createdSettings.IsNonceEnabled, Is.False);
        Assert.That(createdSettings.IsStrictDynamicEnabled, Is.False);
        Assert.That(createdSources, Is.Empty);
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public async Task GivenThereAreSettingsAndEmptySources_WhenImporting_ThenNonceAndStrictDynamicAreAlwaysUnset(bool strictDynamic, bool nonce)
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = true,
                IsNonceEnabled = nonce,
                IsStrictDynamicEnabled = strictDynamic,
                Sources = []
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");
        var createdSettings = await _inMemoryDatabase.CspSettings.FirstOrDefaultAsync();
        var createdSources = await _inMemoryDatabase.CspSources.ToListAsync();

        // Assert
        Assert.That(createdSettings, Is.Not.Null);
        Assert.That(createdSettings.IsNonceEnabled, Is.False);
        Assert.That(createdSettings.IsStrictDynamicEnabled, Is.False);
        Assert.That(createdSources, Is.Empty);
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public async Task GivenThereAreSettingsAndNoNonceDirectives_WhenImporting_ThenNonceAndStrictDynamicAreAlwaysUnset(bool strictDynamic, bool nonce)
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = true,
                IsNonceEnabled = nonce,
                IsStrictDynamicEnabled = strictDynamic,
                Sources =
                [
                    new CspSourceModel
                    {
                        Source = "https://www.example.com/",
                        Directives = [CspConstants.Directives.DefaultSource]
                    }
                ]
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");
        var createdSettings = await _inMemoryDatabase.CspSettings.FirstOrDefaultAsync();
        var createdSources = await _inMemoryDatabase.CspSources.ToListAsync();

        // Assert
        Assert.That(createdSettings, Is.Not.Null);
        Assert.That(createdSettings.IsNonceEnabled, Is.False);
        Assert.That(createdSettings.IsStrictDynamicEnabled, Is.False);
        Assert.That(createdSources.Any(x => x.Source == CspConstants.Sources.Nonce), Is.False);
        Assert.That(createdSources.Any(x => x.Source == CspConstants.Sources.StrictDynamic), Is.False);
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public async Task GivenThereAreSettingsAndNonceDirectives_WhenImporting_ThenNonceAndStrictDynamicAreAlwaysUnset(bool strictDynamic, bool nonce)
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = true,
                IsNonceEnabled = nonce,
                IsStrictDynamicEnabled = strictDynamic,
                Sources =
                [
                    new CspSourceModel
                    {
                        Source = "https://www.example.com/",
                        Directives = [CspConstants.Directives.ScriptSource]
                    }
                ]
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");
        var createdSettings = await _inMemoryDatabase.CspSettings.FirstOrDefaultAsync();
        var createdSources = await _inMemoryDatabase.CspSources.ToListAsync();

        // Assert
        Assert.That(createdSettings, Is.Not.Null);
        Assert.That(createdSettings.IsNonceEnabled, Is.False);
        Assert.That(createdSettings.IsStrictDynamicEnabled, Is.False);
        Assert.That(createdSources.Any(x => x.Source == CspConstants.Sources.Nonce), Is.EqualTo(nonce));
        Assert.That(createdSources.Any(x => x.Source == CspConstants.Sources.StrictDynamic), Is.EqualTo(strictDynamic));
    }

    [Test]
    [TestCase("script-src")]
    [TestCase("style-src")]
    [TestCase("script-src-elem")]
    [TestCase("style-src-elem")]
    [TestCase("script-src,style-src")]
    [TestCase("script-src-elem,style-src-elem")]
    [TestCase("script-src,style-src,script-src-elem,style-src-elem")]
    public async Task GivenThereAreSettingsAndNonceDirectives_WhenImporting_ThenTheCorrectDirectivesAreCreated(string directives)
    {
        // Arrange
        var directiveList = directives.Split(',').ToList();
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = true,
                IsNonceEnabled = true,
                IsStrictDynamicEnabled = true,
                Sources =
                [
                    new CspSourceModel
                    {
                        Source = "https://www.example.com/",
                        Directives = directiveList
                    }
                ]
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");
        var createdSources = await _inMemoryDatabase.CspSources.ToListAsync();

        var nonceSource = createdSources.FirstOrDefault(x => x.Source == CspConstants.Sources.Nonce);
        var strictDynamicSource = createdSources.FirstOrDefault(x => x.Source == CspConstants.Sources.StrictDynamic);

        Assert.That(directiveList.All(nonceSource.Directives.Contains), Is.True);
        Assert.That(directiveList.All(strictDynamicSource.Directives.Contains), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(MigrationRepositoryTestCases), nameof(MigrationRepositoryTestCases.GetNonceDirectiveTestCases))]
    public async Task GivenThereAreSettings_WhenImporting_ThenNonceAndStrictDynamicAreOnlyCreatedForValidSources(string directive, bool shouldExist)
    {
        // Arrange
        var settings = new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                IsEnabled = true,
                IsNonceEnabled = true,
                IsStrictDynamicEnabled = true,
                Sources =
                [
                    new CspSourceModel
                    {
                        Source = "https://www.example.com/",
                        Directives = [directive]
                    }
                ]
            }
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");
        var createdSources = await _inMemoryDatabase.CspSources.ToListAsync();

        Assert.That(createdSources.Any(x => x.Source == CspConstants.Sources.Nonce), Is.EqualTo(shouldExist));
        Assert.That(createdSources.Any(x => x.Source == CspConstants.Sources.StrictDynamic), Is.EqualTo(shouldExist));
    }

    [Test]
    public async Task GivenSettingsDoNotContainCustomHeaders_ThenNoCustomHeaderChangesAreMade()
    {
        // Arrange
        var settings = new SettingsModel { CustomHeaders = null };

        _inMemoryDatabase.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Existing-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "existing-value"
        });
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var records = await _inMemoryDatabase.CustomHeaders.ToListAsync();

        // Assert
        Assert.That(records, Has.Count.EqualTo(1));
        Assert.That(records[0].HeaderName, Is.EqualTo("X-Existing-Header"));
    }

    [Test]
    public async Task GivenEmptyCustomHeaders_AndHeadersExistInData_ThenExistingHeadersAreDeleted()
    {
        // Arrange
        var settings = new SettingsModel
        {
            CustomHeaders = []
        };

        _inMemoryDatabase.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Old-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "old-value"
        });
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var records = await _inMemoryDatabase.CustomHeaders.ToListAsync();

        // Assert
        Assert.That(records, Is.Empty);
    }

    [Test]
    public async Task GivenNewCustomHeaders_AndNoHeadersExistInData_ThenHeadersAreCreated()
    {
        // Arrange
        var settings = new SettingsModel
        {
            CustomHeaders =
            [
                new CustomHeaderModel
                {
                    HeaderName = "X-Custom-One",
                    Behavior = CustomHeaderBehavior.Add,
                    HeaderValue = "value-one"
                },
                new CustomHeaderModel
                {
                    HeaderName = "X-Powered-By",
                    Behavior = CustomHeaderBehavior.Remove,
                    HeaderValue = null
                }
            ]
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var records = await _inMemoryDatabase.CustomHeaders.OrderBy(x => x.HeaderName).ToListAsync();

        // Assert
        Assert.That(records, Has.Count.EqualTo(2));
        Assert.That(records[0].HeaderName, Is.EqualTo("X-Custom-One"));
        Assert.That(records[0].Behavior, Is.EqualTo(CustomHeaderBehavior.Add));
        Assert.That(records[0].HeaderValue, Is.EqualTo("value-one"));
        Assert.That(records[0].ModifiedBy, Is.EqualTo("Test User"));
        Assert.That(records[1].HeaderName, Is.EqualTo("X-Powered-By"));
        Assert.That(records[1].Behavior, Is.EqualTo(CustomHeaderBehavior.Remove));
        Assert.That(records[1].HeaderValue, Is.Null);
    }

    [Test]
    public async Task GivenMatchingCustomHeaders_ThenExistingHeadersAreUpdated()
    {
        // Arrange
        _inMemoryDatabase.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Frame-Options",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "DENY"
        });
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        var settings = new SettingsModel
        {
            CustomHeaders =
            [
                new CustomHeaderModel
                {
                    HeaderName = "X-Frame-Options",
                    Behavior = CustomHeaderBehavior.Add,
                    HeaderValue = "SAMEORIGIN"
                }
            ]
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var records = await _inMemoryDatabase.CustomHeaders.ToListAsync();

        // Assert
        Assert.That(records, Has.Count.EqualTo(1));
        Assert.That(records[0].HeaderName, Is.EqualTo("X-Frame-Options"));
        Assert.That(records[0].Behavior, Is.EqualTo(CustomHeaderBehavior.Add));
        Assert.That(records[0].HeaderValue, Is.EqualTo("SAMEORIGIN"));
        Assert.That(records[0].ModifiedBy, Is.EqualTo("Test User"));
    }

    [Test]
    public async Task GivenCustomHeadersWithDifferentCase_ThenHeadersAreMatchedCaseInsensitively()
    {
        // Arrange
        _inMemoryDatabase.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Frame-Options",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "DENY"
        });
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        var settings = new SettingsModel
        {
            CustomHeaders =
            [
                new CustomHeaderModel
                {
                    HeaderName = "x-frame-options",
                    Behavior = CustomHeaderBehavior.Add,
                    HeaderValue = "SAMEORIGIN"
                }
            ]
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var records = await _inMemoryDatabase.CustomHeaders.ToListAsync();

        // Assert
        Assert.That(records, Has.Count.EqualTo(1));
        Assert.That(records[0].HeaderValue, Is.EqualTo("SAMEORIGIN"));
    }

    [Test]
    public async Task GivenMixOfNewAndExistingAndRemovedCustomHeaders_ThenCorrectOperationsArePerformed()
    {
        // Arrange
        _inMemoryDatabase.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Keep-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "old-value"
        });
        _inMemoryDatabase.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Delete-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "delete-me"
        });
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        var settings = new SettingsModel
        {
            CustomHeaders =
            [
                new CustomHeaderModel
                {
                    HeaderName = "X-Keep-Header",
                    Behavior = CustomHeaderBehavior.Add,
                    HeaderValue = "new-value"
                },
                new CustomHeaderModel
                {
                    HeaderName = "X-New-Header",
                    Behavior = CustomHeaderBehavior.Remove,
                    HeaderValue = null
                }
            ]
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var records = await _inMemoryDatabase.CustomHeaders.OrderBy(x => x.HeaderName).ToListAsync();

        // Assert
        Assert.That(records, Has.Count.EqualTo(2));
        Assert.That(records[0].HeaderName, Is.EqualTo("X-Keep-Header"));
        Assert.That(records[0].HeaderValue, Is.EqualTo("new-value"));
        Assert.That(records[1].HeaderName, Is.EqualTo("X-New-Header"));
        Assert.That(records[1].Behavior, Is.EqualTo(CustomHeaderBehavior.Remove));
    }

    [Test]
    public async Task GivenCustomHeadersWithBlankNames_ThenBlankNamesAreIgnored()
    {
        // Arrange
        var settings = new SettingsModel
        {
            CustomHeaders =
            [
                new CustomHeaderModel
                {
                    HeaderName = "X-Valid-Header",
                    Behavior = CustomHeaderBehavior.Add,
                    HeaderValue = "valid"
                },
                new CustomHeaderModel
                {
                    HeaderName = "",
                    Behavior = CustomHeaderBehavior.Add,
                    HeaderValue = "invalid"
                },
                new CustomHeaderModel
                {
                    HeaderName = null,
                    Behavior = CustomHeaderBehavior.Add,
                    HeaderValue = "also-invalid"
                }
            ]
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var records = await _inMemoryDatabase.CustomHeaders.ToListAsync();

        // Assert
        Assert.That(records, Has.Count.EqualTo(1));
        Assert.That(records[0].HeaderName, Is.EqualTo("X-Valid-Header"));
    }

    [Test]
    public async Task GivenCustomHeaderUpdate_ThenBehaviorIsUpdated()
    {
        // Arrange
        _inMemoryDatabase.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Powered-By",
            Behavior = CustomHeaderBehavior.Disabled,
            HeaderValue = null
        });
        await _inMemoryDatabase.SaveChangesAsync();
        _inMemoryDatabase.ClearTracking();

        var settings = new SettingsModel
        {
            CustomHeaders =
            [
                new CustomHeaderModel
                {
                    HeaderName = "X-Powered-By",
                    Behavior = CustomHeaderBehavior.Remove,
                    HeaderValue = null
                }
            ]
        };

        // Act
        await _repository.SaveAsync(settings, "Test User");

        var records = await _inMemoryDatabase.CustomHeaders.ToListAsync();

        // Assert
        Assert.That(records, Has.Count.EqualTo(1));
        Assert.That(records[0].Behavior, Is.EqualTo(CustomHeaderBehavior.Remove));
    }
}