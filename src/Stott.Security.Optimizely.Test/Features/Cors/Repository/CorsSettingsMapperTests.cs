namespace Stott.Security.Optimizely.Test.Features.Cors.Repository;

using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Cors.Repository;

[TestFixture]
public sealed class CorsSettingsMapperTests
{
    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.IsEnabledTestCases))]
    public void MapToModel_CorrectlyMapsIsEnabled(bool isEnabled)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid(), IsEnabled = isEnabled };
        var model = new CorsConfiguration();

        // Act
        CorsSettingsMapper.MapToModel(entity, model);

        // Assert
        Assert.That(model.IsEnabled, Is.EqualTo(isEnabled));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.IsEnabledTestCases))]
    public void MapToModel_CorrectlyMapsAllowCredentials(bool allowCredentials)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid(), AllowCredentials = allowCredentials };
        var model = new CorsConfiguration();

        // Act
        CorsSettingsMapper.MapToModel(entity, model);

        // Assert
        Assert.That(model.AllowCredentials, Is.EqualTo(allowCredentials));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToModelMethodMappingTestCases))]
    public void MapToModel_CorrectlyMapsAllowMethods(
        string allowMethods,
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
        var entity = new CorsSettings { Id = Guid.NewGuid(), AllowMethods = allowMethods };
        var model = new CorsConfiguration();

        // Act
        CorsSettingsMapper.MapToModel(entity, model);

        // Assert
        Assert.That(model.AllowMethods.IsAllowGetMethods, Is.EqualTo(expectedGet));
        Assert.That(model.AllowMethods.IsAllowHeadMethods, Is.EqualTo(expectedHead));
        Assert.That(model.AllowMethods.IsAllowConnectMethods, Is.EqualTo(expectedConnect));
        Assert.That(model.AllowMethods.IsAllowDeleteMethods, Is.EqualTo(expectedDelete));
        Assert.That(model.AllowMethods.IsAllowOptionsMethods, Is.EqualTo(expectedOptions));
        Assert.That(model.AllowMethods.IsAllowPatchMethods, Is.EqualTo(expectedPatch));
        Assert.That(model.AllowMethods.IsAllowPostMethods, Is.EqualTo(expectedPost));
        Assert.That(model.AllowMethods.IsAllowPutMethods, Is.EqualTo(expectedPut));
        Assert.That(model.AllowMethods.IsAllowTraceMethods, Is.EqualTo(expectedTrace));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToModelHeaderTestCases))]
    public void MapToModel_CorrectlyMapsAllowHeaders(
        string dataHeaders,
        List<string> expectedHeaders)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid(), AllowHeaders = dataHeaders };
        var model = new CorsConfiguration();

        // Act
        CorsSettingsMapper.MapToModel(entity, model);

        // Assert
        Assert.That(model.AllowHeaders, Has.Count.EqualTo(expectedHeaders.Count));
        Assert.That(expectedHeaders.All(x => model.AllowHeaders.Any(y => y.Value == x)), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToModelHeaderTestCases))]
    public void MapToModel_CorrectlyMapsExposeHeaders(
        string dataHeaders,
        List<string> expectedHeaders)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid(), ExposeHeaders = dataHeaders };
        var model = new CorsConfiguration();

        // Act
        CorsSettingsMapper.MapToModel(entity, model);

        // Assert
        Assert.That(model.ExposeHeaders, Has.Count.EqualTo(expectedHeaders.Count));
        Assert.That(expectedHeaders.All(x => model.ExposeHeaders.Any(y => y.Value == x)), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToModelOriginTestCases))]
    public void MapToModel_CorrectlyMapsAllowOrigins(
        string dataOrigins,
        List<string> expectedOrigins)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid(), AllowOrigins = dataOrigins };
        var model = new CorsConfiguration();

        // Act
        CorsSettingsMapper.MapToModel(entity, model);

        // Assert
        Assert.That(model.AllowOrigins, Has.Count.EqualTo(expectedOrigins.Count));
        Assert.That(expectedOrigins.All(x => model.AllowOrigins.Any(y => y.Value == x)), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToModelMaxAgeTestCases))]
    public void MapToModel_CorrectlyMapsMaxAge(
        int dataMaxAge,
        int expectedMaxAge)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid(), MaxAge = dataMaxAge };
        var model = new CorsConfiguration();

        // Act
        CorsSettingsMapper.MapToModel(entity, model);

        // Assert
        Assert.That(model.MaxAge, Is.EqualTo(expectedMaxAge));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.IsEnabledTestCases))]
    public void MapToEntity_CorrectlyMapsIsEnabled(bool isEnabled)
    {
        // Arrange
        var model = new CorsConfiguration { IsEnabled = isEnabled };
        var entity = new CorsSettings();

        // Act
        CorsSettingsMapper.MapToEntity(model, entity);

        // Assert
        Assert.That(entity.IsEnabled, Is.EqualTo(isEnabled));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.IsEnabledTestCases))]
    public void MapToEntity_CorrectlyMapsAllowCredentials(bool allowCredentials)
    {
        // Arrange
        var model = new CorsConfiguration { AllowCredentials = allowCredentials };
        var entity = new CorsSettings();

        // Act
        CorsSettingsMapper.MapToEntity(model, entity);

        // Assert
        Assert.That(entity.AllowCredentials, Is.EqualTo(allowCredentials));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToEntitiesHeaderTestCases))]
    public void MapToEntity_CorrectlyMapsAllowHeaders(
        List<CorsConfigurationItem> headerList,
        string expectedHeaders)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid() };
        var model = new CorsConfiguration();
        model.AllowHeaders.AddRange(headerList);

        // Act
        CorsSettingsMapper.MapToEntity(model, entity);

        // Assert
        Assert.That(entity.AllowHeaders, Is.EqualTo(expectedHeaders));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToEntitiesHeaderTestCases))]
    public void MapToEntity_CorrectlyMapsExposeHeaders(
        List<CorsConfigurationItem> headerList,
        string expectedHeaders)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid() };
        var model = new CorsConfiguration();
        model.ExposeHeaders.AddRange(headerList);

        // Act
        CorsSettingsMapper.MapToEntity(model, entity);

        // Assert
        Assert.That(entity.ExposeHeaders, Is.EqualTo(expectedHeaders));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToEntitiesOriginTestCases))]
    public void MapToEntity_CorrectlyMapsAllowOrigins(
        List<CorsConfigurationItem> allowOrigins,
        string expectedOrigins)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid() };
        var model = new CorsConfiguration
        {
            AllowOrigins = allowOrigins
        };

        // Act
        CorsSettingsMapper.MapToEntity(model, entity);

        // Assert
        Assert.That(entity.AllowOrigins, Is.EqualTo(expectedOrigins));
    }

    [Test]
    [TestCaseSource(typeof(CorsSettingsMapperTestCases), nameof(CorsSettingsMapperTestCases.MapToEntitiesMethodMappingTestCases))]
    public void MapToEntity_CorrectlyMapsAllowMethods(
        bool actualGet,
        bool actualHead,
        bool actualConnect,
        bool actualDelete,
        bool actualOptions,
        bool actualPatch,
        bool actualPost,
        bool actualPut,
        bool actualTrace,
        string expectedMethods)
    {
        // Arrange
        var entity = new CorsSettings { Id = Guid.NewGuid() };
        var model = new CorsConfiguration
        {
            AllowMethods = new CorsConfigurationMethods
            {
                IsAllowConnectMethods = actualConnect,
                IsAllowDeleteMethods = actualDelete,
                IsAllowGetMethods = actualGet,
                IsAllowHeadMethods = actualHead,
                IsAllowOptionsMethods = actualOptions,
                IsAllowPatchMethods = actualPatch,
                IsAllowPostMethods = actualPost,
                IsAllowPutMethods = actualPut,
                IsAllowTraceMethods = actualTrace
            }
        };

        // Act
        CorsSettingsMapper.MapToEntity(model, entity);

        // Assert
        Assert.That(entity.AllowMethods, Is.EqualTo(expectedMethods));
    }
}