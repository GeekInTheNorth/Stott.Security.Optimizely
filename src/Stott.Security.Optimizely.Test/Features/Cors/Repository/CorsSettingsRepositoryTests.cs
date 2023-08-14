namespace Stott.Security.Optimizely.Test.Features.Cors.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Cors.Repository;
using Stott.Security.Optimizely.Test.TestCases;

public sealed class CorsSettingsRepositoryTests
{
    private TestDataContext _inMemoryDatabase;

    private CorsSettingsRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _repository = new CorsSettingsRepository(_inMemoryDatabase);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _inMemoryDatabase.Reset();
    }

    [Test]
    public async Task GetAsync_GivenARecordDoesNotExist_ThenAnEmptyConfigurationWillBeReturned()
    {
        // Act
        var configuration = await _repository.GetAsync();

        // Assert
        Assert.That(configuration, Is.Not.Null);
        Assert.That(configuration.IsEnabled, Is.False);
        Assert.That(configuration.AllowMethods, Is.Not.Null);
        Assert.That(configuration.AllowOrigins, Is.Not.Null);
        Assert.That(configuration.AllowOrigins, Is.Empty);
        Assert.That(configuration.AllowHeaders, Is.Not.Null);
        Assert.That(configuration.AllowHeaders, Is.Empty);
        Assert.That(configuration.ExposeHeaders, Is.Not.Null);
        Assert.That(configuration.ExposeHeaders, Is.Empty);
        Assert.That(configuration.MaxAge, Is.EqualTo(1));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToModelMethodMappingTestCases))]
    public async Task GetAsync_GivenARecordExistsWithAllowMethods_ThenAllowMethodsWillBeConverted(
        [CanBeNull] string allowMethods,
        bool expectedGet,
        bool expectedHead,
        bool expectedConnect,
        bool expectedDelete,
        bool expectedOptions,
        bool expectedPatch,
        bool expectedPost,
        bool expectedPut,
        bool expectedTrace)
    {
        // Arrange
        var corsSettings = new CorsSettings { Id = Guid.NewGuid(),  AllowMethods = allowMethods };
        _inMemoryDatabase.CorsSettings.Add(corsSettings);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var configuration = await _repository.GetAsync();

        // Assert
        Assert.That(configuration.AllowMethods.IsAllowGetMethods, Is.EqualTo(expectedGet));
        Assert.That(configuration.AllowMethods.IsAllowHeadMethods, Is.EqualTo(expectedHead));
        Assert.That(configuration.AllowMethods.IsAllowConnectMethods, Is.EqualTo(expectedConnect));
        Assert.That(configuration.AllowMethods.IsAllowDeleteMethods, Is.EqualTo(expectedDelete));
        Assert.That(configuration.AllowMethods.IsAllowOptionsMethods, Is.EqualTo(expectedOptions));
        Assert.That(configuration.AllowMethods.IsAllowPatchMethods, Is.EqualTo(expectedPatch));
        Assert.That(configuration.AllowMethods.IsAllowPostMethods, Is.EqualTo(expectedPost));
        Assert.That(configuration.AllowMethods.IsAllowPutMethods, Is.EqualTo(expectedPut));
        Assert.That(configuration.AllowMethods.IsAllowTraceMethods, Is.EqualTo(expectedTrace));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToModelHeaderTestCases))]
    public async Task GetAsync_GivenARecordExistsWithAllowHeaders_ThenAllowHeadersWillBeConverted(
        [CanBeNull] string dataHeaders,
        List<string> expectedHeaders)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid(), AllowHeaders = dataHeaders };
        _inMemoryDatabase.CorsSettings.Add(entity);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var configuration = await _repository.GetAsync();

        // Assert
        Assert.That(configuration.AllowHeaders, Has.Count.EqualTo(expectedHeaders.Count));
        Assert.That(expectedHeaders.All(x => configuration.AllowHeaders.Any(y => y.Value == x)), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToModelHeaderTestCases))]
    public async Task GetAsync_GivenARecordExistsWitExposeHeaders_ThenExposeHeadersWillBeConverted(
        [CanBeNull] string dataHeaders,
        List<string> expectedHeaders)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid(), ExposeHeaders = dataHeaders };
        _inMemoryDatabase.CorsSettings.Add(entity);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var configuration = await _repository.GetAsync();

        // Assert
        Assert.That(configuration.ExposeHeaders, Has.Count.EqualTo(expectedHeaders.Count));
        Assert.That(expectedHeaders.All(x => configuration.ExposeHeaders.Any(y => y.Value == x)), Is.True);
    }

    [Test]
    public void SaveAsync_GivenANullModel_ThenAnArgumentExceptionWillBeThrown()
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveAsync(null, "a.user"));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveAsync_GivenANullModel_ThenAnArgumentExceptionWillBeThrown([CanBeNull] string userName)
    {
        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveAsync(new CorsConfiguration(), userName));
    }

    [Test]
    public async Task SaveAsync_GivenAnEntityDoesNotExist_ThenANewEntityWillBeAdded()
    {
        // Act
        var recordsBefore = await _inMemoryDatabase.CorsSettings.CountAsync();

        await _repository.SaveAsync(new CorsConfiguration(), "a.user");

        var recordsAfter = await _inMemoryDatabase.CorsSettings.CountAsync();

        // Assert
        Assert.That(recordsBefore, Is.EqualTo(0));
        Assert.That(recordsAfter, Is.EqualTo(1));
    }

    [Test]
    public async Task SaveAsync_GivenAnEntityDoesExist_ThenANewEntityWillBeAdded()
    {
        // Arrange
        var startingEntity = new CorsSettings { Id = Guid.NewGuid(), IsEnabled = false };
        await _inMemoryDatabase.CorsSettings.AddAsync(startingEntity);
        await _inMemoryDatabase.SaveChangesAsync();

        var newConfiguration = new CorsConfiguration
        {
            IsEnabled = true
        };

        // Act
        await _repository.SaveAsync(newConfiguration, "a.user");

        var recordsAfter = await _inMemoryDatabase.CorsSettings.ToListAsync();

        // Assert
        Assert.That(recordsAfter, Has.Count.EqualTo(1));
        Assert.That(recordsAfter[0].IsEnabled, Is.EqualTo(newConfiguration.IsEnabled));
    }
}