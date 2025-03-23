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
    private Lazy<ICspDataContext> _lazyInMemoryDatabase;

    private TestDataContext _inMemoryDatabase;

    private PermissionPolicyRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _lazyInMemoryDatabase = new Lazy<ICspDataContext>(() => _inMemoryDatabase);

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
        var result = await _repository.GetSettingsAsync();

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
        var result = await _repository.GetSettingsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IsEnabled, Is.EqualTo(actualValue));
    }

    [Test]
    public async Task ListDirectivesAsync_WhenContextHasNoData_ThenEmptyListIsReturned()
    {
        // Act
        var result = await _repository.ListDirectivesAsync();

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
        var result = await _repository.ListDirectivesAsync();
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
        var result = await _repository.ListDirectiveFragments();

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
        var result = await _repository.ListDirectiveFragments();

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
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveDirectiveAsync(null, "modifiedBy"));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveDirectiveAsync_WhenModifiedByIsNullOrWhitespace_ThenArgumentNullExceptionIsThrown(string modifiedBy)
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveDirectiveAsync(new SavePermissionPolicyModel(), modifiedBy));
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

        _repository.SaveDirectiveAsync(model, "modifiedBy").Wait();

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

        _repository.SaveDirectiveAsync(model, "modifiedBy").Wait();

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
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveSettingsAsync(null, "modifiedBy"));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveSettingsAsync_WhenModifiedByIsNullOrWhitespace_ThenArgumentNullExceptionIsThrown(string modifiedBy)
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveSettingsAsync(new PermissionPolicySettingsModel(), modifiedBy));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.BooleanTestCases))]
    public async Task SaveSettingsAsync_WhenRecordDoesNotExist_ThenRecordIsAdded(bool isEnabled)
    {
        // Arrange
        var settings = new PermissionPolicySettingsModel { IsEnabled = isEnabled };

        // Act
        var previousCount = await _inMemoryDatabase.PermissionPolicySettings.CountAsync();

        _repository.SaveSettingsAsync(settings, "modifiedBy").Wait();

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

        _repository.SaveSettingsAsync(settings, "modifiedBy").Wait();

        var newCount = await _inMemoryDatabase.PermissionPolicySettings.CountAsync();
        var record = _inMemoryDatabase.PermissionPolicySettings.FirstOrDefault();

        // Assert
        Assert.That(previousCount, Is.EqualTo(1));
        Assert.That(newCount, Is.EqualTo(1));
        Assert.That(record, Is.Not.Null);
        Assert.That(record.IsEnabled, Is.EqualTo(isEnabled));
    }
}