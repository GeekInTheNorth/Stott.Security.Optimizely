using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Cms.Shell.UI.Components;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;
using Stott.Security.Optimizely.Test.TestCases;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy.Repository;

[TestFixture]
public sealed class PermissionPolicyRepositoryTests
{
    private Lazy<IStottSecurityDataContext> _lazyInMemoryDatabase;

    private TestDataContext _inMemoryDatabase;

    private PermissionPolicyRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _lazyInMemoryDatabase = new Lazy<IStottSecurityDataContext>(() => _inMemoryDatabase);

        _repository = new PermissionPolicyRepository(_lazyInMemoryDatabase);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _inMemoryDatabase.Reset();
    }

    [Test]
    public async Task GetSettingsAsync_WhenContextHasNoData_ThenDefaultSettingsAreReturned()
    {
        // Act
        var result = await _repository.GetSettingsAsync(null, null);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsEnabled, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.BooleanTestCases))]
    public async Task GetSettingsAsync_WhenContextHasData_ThenSettingsAreReturned(bool actualValue)
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = actualValue });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.GetSettingsAsync(null, null);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsEnabled, Is.EqualTo(actualValue));
    }

    [Test]
    public async Task ListDirectivesAsync_WhenContextHasNoData_ThenEmptyListIsReturned()
    {
        // Act
        var result = await _repository.ListDirectivesAsync(null, null);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ListDirectivesAsync_WhenContextHasData_ThenDirectivesAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(
            new Entities.PermissionPolicy
            {
                Directive = PermissionPolicyConstants.Accelerometer,
                EnabledState = PermissionPolicyEnabledState.ThisSite.ToString()
            });
        _inMemoryDatabase.PermissionPolicies.Add(
            new Entities.PermissionPolicy
            {
                Directive = PermissionPolicyConstants.AmbientLightSensor,
                EnabledState = PermissionPolicyEnabledState.ThisAndSpecificSites.ToString(),
                Origins = "https://example.com"
            });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.ListDirectivesAsync(null, null);
        result = result.OrderBy(x => x.Name).ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0].Name, Is.EqualTo(PermissionPolicyConstants.Accelerometer));
        Assert.That(result[0].EnabledState, Is.EqualTo(PermissionPolicyEnabledState.ThisSite));
        Assert.That(result[0].Sources, Has.Count.EqualTo(0));
        Assert.That(result[1].Name, Is.EqualTo(PermissionPolicyConstants.AmbientLightSensor));
        Assert.That(result[1].EnabledState, Is.EqualTo(PermissionPolicyEnabledState.ThisAndSpecificSites));
        Assert.That(result[1].Sources, Has.Count.EqualTo(1));
        Assert.That(result[1].Sources[0].Url, Is.EqualTo("https://example.com"));
    }

    [Test]
    public async Task ListDirectiveFragments_WhenContextHasNoData_ThenEmptyListIsReturned()
    {
        // Act
        var result = await _repository.ListDirectiveFragments(null, null);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ListDirectiveFragments_WhenContextHasData_ThenDirectiveFragmentsAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(
            new Entities.PermissionPolicy
            {
                Directive = PermissionPolicyConstants.Accelerometer,
                EnabledState = PermissionPolicyEnabledState.ThisSite.ToString()
            });
        _inMemoryDatabase.PermissionPolicies.Add(
            new Entities.PermissionPolicy
            {
                Directive = PermissionPolicyConstants.AmbientLightSensor,
                EnabledState = PermissionPolicyEnabledState.ThisAndSpecificSites.ToString(),
                Origins = "https://example.com"
            });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.ListDirectiveFragments(null, null);
        result = result.OrderBy(x => x).ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0], Is.EqualTo("accelerometer=(self)"));
        Assert.That(result[1], Is.EqualTo("ambient-light-sensor=(self \"https://example.com\")"));
    }

    [Test]
    public void SaveDirectiveAsync_WhenModelIsNull_ThenArgumentNullExceptionIsThrown()
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveDirectiveAsync(null, "modifiedBy", null, null));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveDirectiveAsync_WhenModifiedByIsNullOrWhitespace_ThenArgumentNullExceptionIsThrown(string modifiedBy)
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveDirectiveAsync(new SavePermissionPolicyModel(), modifiedBy, null, null));
    }

    [Test]
    public async Task SaveDirectiveAsync_WhenRecordDoesNotExist_ThenRecordIsAdded()
    {
        // Arrange
        var model = new SavePermissionPolicyModel
        {
            Name = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.ThisSite,
            Sources = new List<string> { "https://example.com" }
        };

        // Act
        var previousCount = await _inMemoryDatabase.PermissionPolicies.CountAsync();

        _repository.SaveDirectiveAsync(model, "modifiedBy", null, null).Wait();

        var newCount = await _inMemoryDatabase.PermissionPolicies.CountAsync();
        var record = _inMemoryDatabase.PermissionPolicies.FirstOrDefault(x => x.Directive == PermissionPolicyConstants.Accelerometer);

        // Assert
        Assert.That(previousCount, Is.EqualTo(0));
        Assert.That(newCount, Is.EqualTo(1));
        Assert.That(record, Is.Not.Null);
        Assert.That(record.Directive, Is.EqualTo(PermissionPolicyConstants.Accelerometer));
        Assert.That(record.EnabledState, Is.EqualTo(PermissionPolicyEnabledState.ThisSite.ToString()));
        Assert.That(record.Origins, Is.EqualTo("https://example.com"));
    }

    [Test]
    public async Task SaveDirectiveAsync_WhenRecordDoesExist_ThenRecordIsUpdated()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(
            new Entities.PermissionPolicy
            {
                Directive = PermissionPolicyConstants.Accelerometer,
                EnabledState = PermissionPolicyEnabledState.ThisSite.ToString()
            });
        _inMemoryDatabase.SaveChanges();

        var model = new SavePermissionPolicyModel
        {
            Name = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.ThisAndSpecificSites,
            Sources = new List<string> { "https://example.com" }
        };

        // Act
        var previousCount = await _inMemoryDatabase.PermissionPolicies.CountAsync();

        _repository.SaveDirectiveAsync(model, "modifiedBy", null, null).Wait();

        var newCount = await _inMemoryDatabase.PermissionPolicies.CountAsync();
        var record = _inMemoryDatabase.PermissionPolicies.FirstOrDefault(x => x.Directive == PermissionPolicyConstants.Accelerometer);

        // Assert
        Assert.That(previousCount, Is.EqualTo(1));
        Assert.That(newCount, Is.EqualTo(1));
        Assert.That(record, Is.Not.Null);
        Assert.That(record.Directive, Is.EqualTo(PermissionPolicyConstants.Accelerometer));
        Assert.That(record.EnabledState, Is.EqualTo(PermissionPolicyEnabledState.ThisAndSpecificSites.ToString()));
        Assert.That(record.Origins, Is.EqualTo("https://example.com"));
    }

    [Test]
    public void SaveSettingsAsync_WhenSettingsIsNull_ThenArgumentNullExceptionIsThrown()
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveSettingsAsync(null, "modifiedBy", null, null));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveSettingsAsync_WhenModifiedByIsNullOrWhitespace_ThenArgumentNullExceptionIsThrown(string modifiedBy)
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveSettingsAsync(new PermissionPolicySettingsModel(), modifiedBy, null, null));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.BooleanTestCases))]
    public async Task SaveSettingsAsync_WhenRecordDoesNotExist_ThenRecordIsAdded(bool isEnabled)
    {
        // Arrange
        var settings = new PermissionPolicySettingsModel { IsEnabled = isEnabled };

        // Act
        var previousCount = await _inMemoryDatabase.PermissionPolicySettings.CountAsync();

        _repository.SaveSettingsAsync(settings, "modifiedBy", null, null).Wait();

        var newCount = await _inMemoryDatabase.PermissionPolicySettings.CountAsync();
        var record = _inMemoryDatabase.PermissionPolicySettings.FirstOrDefault();

        // Assert
        Assert.That(previousCount, Is.EqualTo(0));
        Assert.That(newCount, Is.EqualTo(1));
        Assert.That(record, Is.Not.Null);
        Assert.That(record.IsEnabled, Is.EqualTo(isEnabled));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.BooleanTestCases))]
    public async Task SaveSettingsAsync_WhenRecordDoesExist_ThenRecordIsUpdated(bool isEnabled)
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = false });
        _inMemoryDatabase.SaveChanges();

        var settings = new PermissionPolicySettingsModel { IsEnabled = isEnabled };

        // Act
        var previousCount = await _inMemoryDatabase.PermissionPolicySettings.CountAsync();

        _repository.SaveSettingsAsync(settings, "modifiedBy", null, null).Wait();

        var newCount = await _inMemoryDatabase.PermissionPolicySettings.CountAsync();
        var record = _inMemoryDatabase.PermissionPolicySettings.FirstOrDefault();

        // Assert
        Assert.That(previousCount, Is.EqualTo(1));
        Assert.That(newCount, Is.EqualTo(1));
        Assert.That(record, Is.Not.Null);
        Assert.That(record.IsEnabled, Is.EqualTo(isEnabled));
    }

    #region Multi-Site Settings Fallback Tests

    [Test]
    public async Task GetSettingsAsync_GivenAppIdSettings_WhenRequestedForAppId_ThenAppIdSettingsAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = false });
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true, AppId = "app1" });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.GetSettingsAsync("app1", null);

        // Assert
        Assert.That(result.IsEnabled, Is.True);
    }

    [Test]
    public async Task GetSettingsAsync_GivenAppIdSettings_WhenRequestedForDifferentAppId_ThenGlobalSettingsAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = false });
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true, AppId = "app1" });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.GetSettingsAsync("app2", null);

        // Assert
        Assert.That(result.IsEnabled, Is.False);
    }

    [Test]
    public async Task GetSettingsAsync_GivenHostNameSettings_WhenRequestedForExactMatch_ThenHostNameSettingsAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = false });
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true, AppId = "app1", HostName = "host1" });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.GetSettingsAsync("app1", "host1");

        // Assert
        Assert.That(result.IsEnabled, Is.True);
    }

    [Test]
    public async Task GetSettingsAsync_GivenNoHostNameSettings_WhenRequestedForHostName_ThenAppIdSettingsAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = false });
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true, AppId = "app1" });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.GetSettingsAsync("app1", "host1");

        // Assert
        Assert.That(result.IsEnabled, Is.True);
    }

    [Test]
    public async Task GetSettingsAsync_GivenNoAppIdSettings_WhenRequestedForHostName_ThenGlobalSettingsAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.GetSettingsAsync("app1", "host1");

        // Assert
        Assert.That(result.IsEnabled, Is.True);
    }

    #endregion

    #region Multi-Site GetSettingsByContext Tests

    [Test]
    public async Task GetSettingsByContextAsync_WhenExactMatchExists_ThenSettingsAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true, AppId = "app1", HostName = "host1" });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.GetSettingsByContextAsync("app1", "host1");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.IsEnabled, Is.True);
    }

    [Test]
    public async Task GetSettingsByContextAsync_WhenNoExactMatchExists_ThenNullIsReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.GetSettingsByContextAsync("app1", "host1");

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region Multi-Site SaveSettings Tests

    [Test]
    public async Task SaveSettingsAsync_WhenSavingForAppId_ThenRecordHasAppId()
    {
        // Arrange
        var settings = new PermissionPolicySettingsModel { IsEnabled = true };

        // Act
        await _repository.SaveSettingsAsync(settings, "modifiedBy", "app1", null);
        var record = _inMemoryDatabase.PermissionPolicySettings.FirstOrDefault(x => x.AppId == "app1");

        // Assert
        Assert.That(record, Is.Not.Null);
        Assert.That(record!.AppId, Is.EqualTo("app1"));
        Assert.That(record.HostName, Is.Null);
        Assert.That(record.IsEnabled, Is.True);
    }

    [Test]
    public async Task SaveSettingsAsync_WhenSavingForHostName_ThenRecordHasAppIdAndHostName()
    {
        // Arrange
        var settings = new PermissionPolicySettingsModel { IsEnabled = true };

        // Act
        await _repository.SaveSettingsAsync(settings, "modifiedBy", "app1", "host1");
        var record = _inMemoryDatabase.PermissionPolicySettings.FirstOrDefault(x => x.AppId == "app1" && x.HostName == "host1");

        // Assert
        Assert.That(record, Is.Not.Null);
        Assert.That(record!.AppId, Is.EqualTo("app1"));
        Assert.That(record.HostName, Is.EqualTo("host1"));
        Assert.That(record.IsEnabled, Is.True);
    }

    #endregion

    #region Multi-Site Directive Fallback Tests

    [Test]
    public async Task ListDirectivesAsync_GivenGlobalAndAppIdDirectives_WhenRequestedForAppId_ThenAppIdDirectivesAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.None.ToString()
        });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString(),
            AppId = "app1"
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.ListDirectivesAsync("app1", null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].EnabledState, Is.EqualTo(PermissionPolicyEnabledState.All));
    }

    [Test]
    public async Task ListDirectivesAsync_GivenOnlyGlobalDirectives_WhenRequestedForAppId_ThenGlobalDirectivesAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.ThisSite.ToString()
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.ListDirectivesAsync("app1", null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].EnabledState, Is.EqualTo(PermissionPolicyEnabledState.ThisSite));
    }

    [Test]
    public async Task ListDirectivesAsync_GivenGlobalAppIdAndHostNameDirectives_WhenRequestedForHostName_ThenHostNameDirectivesAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.None.ToString()
        });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.ThisSite.ToString(),
            AppId = "app1"
        });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString(),
            AppId = "app1",
            HostName = "host1"
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.ListDirectivesAsync("app1", "host1");

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].EnabledState, Is.EqualTo(PermissionPolicyEnabledState.All));
    }

    [Test]
    public async Task ListDirectivesAsync_GivenGlobalAndAppIdDirectives_WhenRequestedForHostName_ThenAppIdDirectivesAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.None.ToString()
        });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.ThisSite.ToString(),
            AppId = "app1"
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.ListDirectivesAsync("app1", "host1");

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].EnabledState, Is.EqualTo(PermissionPolicyEnabledState.ThisSite));
    }

    #endregion

    #region Multi-Site ListDirectivesByContext Tests

    [Test]
    public async Task ListDirectivesByContextAsync_WhenExactMatchExists_ThenDirectivesAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString(),
            AppId = "app1"
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.ListDirectivesByContextAsync("app1", null);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task ListDirectivesByContextAsync_WhenNoExactMatchExists_ThenNullIsReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString()
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.ListDirectivesByContextAsync("app1", null);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region Multi-Site SaveDirective Tests

    [Test]
    public async Task SaveDirectiveAsync_WhenSavingForAppId_ThenRecordHasAppId()
    {
        // Arrange
        var model = new SavePermissionPolicyModel
        {
            Name = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All,
            Sources = new List<string>()
        };

        // Act
        await _repository.SaveDirectiveAsync(model, "modifiedBy", "app1", null);
        var record = _inMemoryDatabase.PermissionPolicies.FirstOrDefault(x => x.AppId == "app1");

        // Assert
        Assert.That(record, Is.Not.Null);
        Assert.That(record!.AppId, Is.EqualTo("app1"));
        Assert.That(record.HostName, Is.Null);
    }

    [Test]
    public async Task SaveDirectiveAsync_WhenSavingForAppId_ThenGlobalRecordIsNotAffected()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.None.ToString()
        });
        _inMemoryDatabase.SaveChanges();

        var model = new SavePermissionPolicyModel
        {
            Name = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All,
            Sources = new List<string>()
        };

        // Act
        await _repository.SaveDirectiveAsync(model, "modifiedBy", "app1", null);
        var globalRecord = _inMemoryDatabase.PermissionPolicies.FirstOrDefault(x => x.AppId == null);
        var appRecord = _inMemoryDatabase.PermissionPolicies.FirstOrDefault(x => x.AppId == "app1");
        var totalCount = await _inMemoryDatabase.PermissionPolicies.CountAsync();

        // Assert
        Assert.That(totalCount, Is.EqualTo(2));
        Assert.That(globalRecord!.EnabledState, Is.EqualTo(PermissionPolicyEnabledState.None.ToString()));
        Assert.That(appRecord!.EnabledState, Is.EqualTo(PermissionPolicyEnabledState.All.ToString()));
    }

    #endregion

    #region Multi-Site CreateDirectiveOverride Tests

    [Test]
    public async Task CreateDirectiveOverrideAsync_CopiesGlobalDirectivesToAppId()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString(),
            Origins = "https://example.com"
        });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Autoplay,
            EnabledState = PermissionPolicyEnabledState.ThisSite.ToString()
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.CreateOverrideAsync(null, null, "app1", null, "modifiedBy");
        var appDirectives = _inMemoryDatabase.PermissionPolicies.Where(x => x.AppId == "app1").ToList();

        // Assert
        Assert.That(appDirectives, Has.Count.EqualTo(2));
        Assert.That(appDirectives.Any(x => x.Directive == PermissionPolicyConstants.Accelerometer && x.EnabledState == PermissionPolicyEnabledState.All.ToString()), Is.True);
        Assert.That(appDirectives.Any(x => x.Directive == PermissionPolicyConstants.Autoplay && x.EnabledState == PermissionPolicyEnabledState.ThisSite.ToString()), Is.True);
    }

    [Test]
    public async Task CreateDirectiveOverrideAsync_WhenTargetAlreadyHasDirectives_ThenNothingIsCopied()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString()
        });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.None.ToString(),
            AppId = "app1"
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.CreateOverrideAsync(null, null, "app1", null, "modifiedBy");
        var appDirectives = _inMemoryDatabase.PermissionPolicies.Where(x => x.AppId == "app1").ToList();

        // Assert
        Assert.That(appDirectives, Has.Count.EqualTo(1));
        Assert.That(appDirectives[0].EnabledState, Is.EqualTo(PermissionPolicyEnabledState.None.ToString()));
    }

    [Test]
    public void CreateDirectiveOverrideAsync_WhenModifiedByIsNullOrWhitespace_ThenArgumentNullExceptionIsThrown()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.CreateOverrideAsync(null, null, "app1", null, ""));
    }

    #endregion

    #region Multi-Site DeleteDirectivesByContext Tests

    [Test]
    public async Task DeleteByContextAsync_WhenAppIdIsNull_ThenNothingIsDeleted()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString()
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.DeleteByContextAsync(null, null, "deletedBy");
        var settingsCount = await _inMemoryDatabase.PermissionPolicySettings.CountAsync();
        var directivesCount = await _inMemoryDatabase.PermissionPolicies.CountAsync();

        // Assert
        Assert.That(settingsCount, Is.EqualTo(1));
        Assert.That(directivesCount, Is.EqualTo(1));
    }

    [Test]
    public async Task DeleteByContextAsync_WhenRecordsExist_ThenRecordsAreDeleted()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true, AppId = "app1" });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString(),
            AppId = "app1"
        });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Autoplay,
            EnabledState = PermissionPolicyEnabledState.ThisSite.ToString(),
            AppId = "app1"
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.DeleteByContextAsync("app1", null, "deletedBy");
        var settingsCount = await _inMemoryDatabase.PermissionPolicySettings.CountAsync();
        var directivesCount = await _inMemoryDatabase.PermissionPolicies.CountAsync();

        // Assert
        Assert.That(settingsCount, Is.EqualTo(0));
        Assert.That(directivesCount, Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteByContextAsync_WhenDeletingAppId_ThenGlobalDirectivesAreNotDeleted()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true });
        _inMemoryDatabase.PermissionPolicySettings.Add(new PermissionPolicySettings { IsEnabled = true, AppId = "app1" });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString()
        });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.None.ToString(),
            AppId = "app1"
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.DeleteByContextAsync("app1", null, "deletedBy");
        var globalSettingsCount = _inMemoryDatabase.PermissionPolicySettings.Count(x => x.AppId == null);
        var appSettingsCount = _inMemoryDatabase.PermissionPolicySettings.Count(x => x.AppId == "app1");
        var globalDirectivesCount = _inMemoryDatabase.PermissionPolicies.Count(x => x.AppId == null);
        var appDirectivesCount = _inMemoryDatabase.PermissionPolicies.Count(x => x.AppId == "app1");

        // Assert
        Assert.That(globalSettingsCount, Is.EqualTo(1));
        Assert.That(appSettingsCount, Is.EqualTo(0));
        Assert.That(globalDirectivesCount, Is.EqualTo(1));
        Assert.That(appDirectivesCount, Is.EqualTo(0));
    }

    #endregion

    #region Multi-Site ListDirectiveFragments Fallback Tests

    [Test]
    public async Task ListDirectiveFragments_GivenAppIdDirectives_WhenRequestedForAppId_ThenAppIdFragmentsAreReturned()
    {
        // Arrange
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.None.ToString()
        });
        _inMemoryDatabase.PermissionPolicies.Add(new Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.All.ToString(),
            AppId = "app1"
        });
        _inMemoryDatabase.SaveChanges();

        // Act
        var result = await _repository.ListDirectiveFragments("app1", null);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.EqualTo("accelerometer=*"));
    }

    #endregion
}
