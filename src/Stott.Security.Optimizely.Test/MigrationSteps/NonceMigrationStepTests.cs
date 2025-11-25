using System;
using System.Collections.Generic;
using System.Linq;

using EPiServer.ServiceLocation;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;
using Stott.Security.Optimizely.MigrationSteps;

namespace Stott.Security.Optimizely.Test.MigrationSteps;

[TestFixture]
public sealed class NonceMigrationStepTests
{
    private Mock<ICspSettingsService> _settingsServiceMock;

    private Mock<ICspPermissionService> _permissionServiceMock;

    private Mock<IServiceProvider> _serviceProviderMock;

    private NonceMigrationStep _migrationStep;

    [SetUp]
    public void SetUp()
    {
        _settingsServiceMock = new Mock<ICspSettingsService>();

        _permissionServiceMock = new Mock<ICspPermissionService>();

        _serviceProviderMock = new Mock<IServiceProvider>();
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(ICspSettingsService))).Returns(_settingsServiceMock.Object);
        _serviceProviderMock.Setup(sp => sp.GetService(typeof(ICspPermissionService))).Returns(_permissionServiceMock.Object);

        ServiceLocator.SetServiceProvider(_serviceProviderMock.Object);

        _migrationStep = new NonceMigrationStep();
    }

    [Test]
    public void GivenSettingsServiceReturnsAnNullCspSettings_ThenNoConversionShouldBePerformed()
    {
        // Arrange
        _settingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync((CspSettings)null);

        // Act
        _migrationStep.AddChanges();

        // Assert
        _settingsServiceMock.Verify(s => s.SaveAsync(It.IsAny<CspSettings>(), It.IsAny<string>()), Times.Never);
        _permissionServiceMock.Verify(s => s.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GivenSettingsServiceReturnsCspSettingsWithNonceDisabled_ThenNoConversionShouldBePerformed()
    {
        // Arrange
        var settings = new CspSettings { IsNonceEnabled = false, IsStrictDynamicEnabled = false };
        _settingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(settings);
        
        // Act
        _migrationStep.AddChanges();
        
        // Assert
        _settingsServiceMock.Verify(s => s.SaveAsync(It.IsAny<CspSettings>(), It.IsAny<string>()), Times.Never);
        _permissionServiceMock.Verify(s => s.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GivenSettingsServiceReturnsCspSettingsWithNonceEnabled_ThenCspSettingsShouldHaveNonceAndStrictDynamicDisabled()
    {
        // Arrange
        var settings = new CspSettings { IsNonceEnabled = true, IsStrictDynamicEnabled = true };
        _settingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(settings);
        
        // Act
        _migrationStep.AddChanges();

        // Assert
        _settingsServiceMock.Verify(s => s.SaveAsync(It.IsAny<CspSettings>(), It.IsAny<string>()), Times.Once);
        _settingsServiceMock.Verify(s => s.SaveAsync(It.Is<CspSettings>(s => !s.IsNonceEnabled && !s.IsStrictDynamicEnabled), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GivenPermissionServiceReturnsNull_ThenNoSourcesWillBeCreated()
    {
        // Arrange
        var settings = new CspSettings { IsNonceEnabled = true, IsStrictDynamicEnabled = true };
        _settingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(settings);
        _permissionServiceMock.Setup(s => s.GetAsync()).ReturnsAsync((List<CspSource>)null);

        // Act
        _migrationStep.AddChanges();

        // Assert
        _permissionServiceMock.Verify(s => s.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GivenPermissionServiceReturnsNoSources_ThenNoSourcesWillBeCreated()
    {
        // Arrange
        var settings = new CspSettings { IsNonceEnabled = true, IsStrictDynamicEnabled = true };
        _settingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(settings);
        _permissionServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(new List<CspSource>());
        
        // Act
        _migrationStep.AddChanges();
        
        // Assert
        _permissionServiceMock.Verify(s => s.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(nameof(GetNonNonceDirectiveTestCases))]
    public void GivenPermissionServiceReturnsSourcesWithoutScriptOrStyleDirectives_ThenNoSourcesWillBeCreated(string directive)
    {
        // Arrange
        var settings = new CspSettings { IsNonceEnabled = true, IsStrictDynamicEnabled = true };
        _settingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(settings);
        _permissionServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(new List<CspSource>
        {
            new() { Source = "https://example.com", Directives = directive }
        });
        
        // Act
        _migrationStep.AddChanges();
        
        // Assert
        _permissionServiceMock.Verify(s => s.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(nameof(GetNonceDirectiveTestCases))]
    public void GivenPermissionServiceReturnsSourcesWithScriptOrStyleDirectives_ThenNonceSourcesWillBeCreated(string directive)
    {
        // Arrange
        var settings = new CspSettings { IsNonceEnabled = true, IsStrictDynamicEnabled = true };
        _settingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(settings);
        _permissionServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(new List<CspSource>
        {
            new() { Source = "https://example.com", Directives = directive }
        });

        // Act
        _migrationStep.AddChanges();

        // Assert
        _permissionServiceMock.Verify(s => s.SaveAsync(It.IsAny<Guid>(), CspConstants.Sources.Nonce, It.IsAny<List<string>>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    [TestCaseSource(nameof(GetNonceDirectiveTestCases))]
    public void GivenPermissionServiceReturnsSourcesWithScriptOrStyleDirectivesAndStrictDynamicIsDisabled_ThenStrictDynamicSourcesWillNotBeCreated(string directive)
    {
        // Arrange
        var settings = new CspSettings { IsNonceEnabled = true, IsStrictDynamicEnabled = false };
        _settingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(settings);
        _permissionServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(new List<CspSource>
        {
            new() { Source = "https://example.com", Directives = directive }
        });

        // Act
        _migrationStep.AddChanges();

        // Assert
        _permissionServiceMock.Verify(s => s.SaveAsync(It.IsAny<Guid>(), CspConstants.Sources.StrictDynamic, It.IsAny<List<string>>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    [TestCaseSource(nameof(GetNonceDirectiveTestCases))]
    public void GivenPermissionServiceReturnsSourcesWithScriptOrStyleDirectivesAndStrictDynamicIsEnabled_ThenStrictDynamicSourcesWillBeCreated(string directive)
    {
        // Arrange
        var settings = new CspSettings { IsNonceEnabled = true, IsStrictDynamicEnabled = true };
        _settingsServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(settings);
        _permissionServiceMock.Setup(s => s.GetAsync()).ReturnsAsync(new List<CspSource>
        {
            new() { Source = "https://example.com", Directives = directive }
        });

        // Act
        _migrationStep.AddChanges();

        // Assert
        _permissionServiceMock.Verify(s => s.SaveAsync(It.IsAny<Guid>(), CspConstants.Sources.StrictDynamic, It.IsAny<List<string>>(), It.IsAny<string>()), Times.Once);
    }

    public static IEnumerable<TestCaseData> GetNonceDirectiveTestCases
    {
        get
        {
            return CspConstants.NonceDirectives.Select(x => new TestCaseData(x));
        }
    }

    public static IEnumerable<TestCaseData> GetNonNonceDirectiveTestCases
    {
        get
        {
            return CspConstants.AllDirectives.Where(x => !CspConstants.NonceDirectives.Contains(x)).Select(x => new TestCaseData(x));
        }
    }
}
