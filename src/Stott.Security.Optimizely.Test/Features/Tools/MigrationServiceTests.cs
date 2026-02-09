using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Cors.Repository;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Repository;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.CustomHeaders.Repository;
using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;
using Stott.Security.Optimizely.Features.Tools;

namespace Stott.Security.Optimizely.Test.Features.Tools;

[TestFixture]
public sealed class MigrationServiceTests
{
    private Mock<ICspSettingsRepository> _mockCspSettingsRepository;

    private Mock<ICspPermissionRepository> _mockCspPermissionRepository;

    private Mock<ICspSandboxRepository> _mockCspSandboxRepository;

    private Mock<ICorsSettingsRepository> _mockCorsSettingsRepository;

    private Mock<IPermissionPolicyRepository> _mockPermissionPolicyRepository;

    private Mock<ICustomHeaderRepository> _mockCustomHeaderRepository;

    private Mock<IMigrationRepository> _mockMigrationRepository;

    private Mock<ICacheWrapper> _mockCacheWrapper;

    private MigrationService _service;

    [SetUp]
    public void SetUp()
    {
        _mockCspSettingsRepository = new Mock<ICspSettingsRepository>();
        _mockCspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings());

        _mockCspPermissionRepository = new Mock<ICspPermissionRepository>();
        _mockCspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(new List<CspSource>());

        _mockCspSandboxRepository = new Mock<ICspSandboxRepository>();
        _mockCspSandboxRepository.Setup(x => x.GetAsync()).ReturnsAsync(new SandboxModel());

        _mockCorsSettingsRepository = new Mock<ICorsSettingsRepository>();
        _mockCorsSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CorsConfiguration());

        _mockPermissionPolicyRepository = new Mock<IPermissionPolicyRepository>();
        _mockPermissionPolicyRepository.Setup(x => x.GetSettingsAsync()).ReturnsAsync(new PermissionPolicySettingsModel());
        _mockPermissionPolicyRepository.Setup(x => x.ListDirectivesAsync()).ReturnsAsync(new List<PermissionPolicyDirectiveModel>());

        _mockCustomHeaderRepository = new Mock<ICustomHeaderRepository>();
        _mockCustomHeaderRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>());

        _mockMigrationRepository = new Mock<IMigrationRepository>();

        _mockCacheWrapper = new Mock<ICacheWrapper>();

        _service = new MigrationService(
            _mockCspSettingsRepository.Object,
            _mockCspPermissionRepository.Object,
            _mockCspSandboxRepository.Object,
            _mockCorsSettingsRepository.Object,
            _mockPermissionPolicyRepository.Object,
            _mockCustomHeaderRepository.Object,
            _mockMigrationRepository.Object,
            _mockCacheWrapper.Object);
    }

    [Test]
    public async Task Export_CallsGetAsyncOnCspSettingsRepository()
    {
        // Act
        await _service.Export();

        // Assert
        _mockCspSettingsRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Test]
    public async Task Export_CallsGetAsyncOnCspPermissionRepository()
    {
        // Act
        await _service.Export();

        // Assert
        _mockCspPermissionRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Test]
    public async Task Export_CallsGetAsyncOnCspSandboxRepository()
    {
        // Act
        await _service.Export();

        // Assert
        _mockCspSandboxRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Test]
    public async Task Export_CallsGetAsyncOnCorsSettingsRepository()
    {
        // Act
        await _service.Export();

        // Assert
        _mockCorsSettingsRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Test]
    public async Task Export_CallsGetSettingsAsyncOnPermissionPolicyRepository()
    {
        // Act
        await _service.Export();

        // Assert
        _mockPermissionPolicyRepository.Verify(x => x.GetSettingsAsync(), Times.Once);
    }

    [Test]
    public async Task Export_CallsListDirectivesAsyncOnPermissionPolicyRepository()
    {
        // Act
        await _service.Export();

        // Assert
        _mockPermissionPolicyRepository.Verify(x => x.ListDirectivesAsync(), Times.Once);
    }

    [Test]
    public async Task Export_CallsGetAllAsyncOnCustomHeaderRepository()
    {
        // Act
        await _service.Export();

        // Assert
        _mockCustomHeaderRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Export_MapsCspIsEnabled(bool isEnabled)
    {
        // Arrange
        _mockCspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsEnabled = isEnabled });

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.IsEnabled, Is.EqualTo(isEnabled));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Export_MapsCspIsReportOnly(bool isReportOnly)
    {
        // Arrange
        _mockCspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsReportOnly = isReportOnly });

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.IsReportOnly, Is.EqualTo(isReportOnly));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Export_MapsCspIsAllowListEnabled(bool isAllowListEnabled)
    {
        // Arrange
        _mockCspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsAllowListEnabled = isAllowListEnabled });

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.IsAllowListEnabled, Is.EqualTo(isAllowListEnabled));
    }

    [Test]
    [TestCase("https://example.com/allowlist")]
    [TestCase(null)]
    public async Task Export_MapsCspAllowListUrl(string allowListUrl)
    {
        // Arrange
        _mockCspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { AllowListUrl = allowListUrl });

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.AllowListUrl, Is.EqualTo(allowListUrl));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Export_MapsCspIsUpgradeInsecureRequestsEnabled(bool isUpgradeInsecureRequestsEnabled)
    {
        // Arrange
        _mockCspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsUpgradeInsecureRequestsEnabled = isUpgradeInsecureRequestsEnabled });

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.IsUpgradeInsecureRequestsEnabled, Is.EqualTo(isUpgradeInsecureRequestsEnabled));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Export_MapsCspUseInternalReporting(bool useInternalReporting)
    {
        // Arrange
        _mockCspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { UseInternalReporting = useInternalReporting });

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.UseInternalReporting, Is.EqualTo(useInternalReporting));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Export_MapsCspUseExternalReporting(bool useExternalReporting)
    {
        // Arrange
        _mockCspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { UseExternalReporting = useExternalReporting });

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.UseExternalReporting, Is.EqualTo(useExternalReporting));
    }

    [Test]
    [TestCase("https://example.com/report")]
    [TestCase(null)]
    public async Task Export_MapsCspExternalReportToUrl(string externalReportToUrl)
    {
        // Arrange
        _mockCspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { ExternalReportToUrl = externalReportToUrl });

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.ExternalReportToUrl, Is.EqualTo(externalReportToUrl));
    }

    [Test]
    public async Task Export_MapsCspSandbox()
    {
        // Arrange
        var sandbox = new SandboxModel { IsSandboxEnabled = true, IsAllowScriptsEnabled = true };
        _mockCspSandboxRepository.Setup(x => x.GetAsync()).ReturnsAsync(sandbox);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.Sandbox, Is.Not.Null);
        Assert.That(result.Csp.Sandbox!.IsSandboxEnabled, Is.True);
        Assert.That(result.Csp.Sandbox.IsAllowScriptsEnabled, Is.True);
    }

    [Test]
    public async Task Export_DefaultsSandboxWhenNull()
    {
        // Arrange
        _mockCspSandboxRepository.Setup(x => x.GetAsync()).ReturnsAsync((SandboxModel)null!);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.Sandbox, Is.Not.Null);
    }

    [Test]
    public async Task Export_MapsCspSourceDomain()
    {
        // Arrange
        var sources = new List<CspSource>
        {
            new() { Source = "https://example.com", Directives = "script-src" }
        };
        _mockCspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.Sources!.First().Source, Is.EqualTo("https://example.com"));
    }

    [Test]
    public async Task Export_SplitsCspSourceDirectivesByComma()
    {
        // Arrange
        var sources = new List<CspSource>
        {
            new() { Source = "https://example.com", Directives = "script-src,style-src" }
        };
        _mockCspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.Sources!.First().Directives, Has.Count.EqualTo(2));
        Assert.That(result.Csp.Sources!.First().Directives, Does.Contain("script-src"));
        Assert.That(result.Csp.Sources!.First().Directives, Does.Contain("style-src"));
    }

    [Test]
    public async Task Export_SplitsCspSourceDirectivesBySpace()
    {
        // Arrange
        var sources = new List<CspSource>
        {
            new() { Source = "https://example.com", Directives = "script-src style-src" }
        };
        _mockCspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.Sources!.First().Directives, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Export_ReturnsEmptyDirectivesWhenNull()
    {
        // Arrange
        var sources = new List<CspSource>
        {
            new() { Source = "https://example.com", Directives = null }
        };
        _mockCspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.Sources!.First().Directives, Is.Empty);
    }

    [Test]
    public async Task Export_ReturnsEmptySourcesWhenNoneExist()
    {
        // Arrange
        _mockCspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(new List<CspSource>());

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Csp!.Sources, Is.Empty);
    }

    [Test]
    public async Task Export_ReturnsCorsSettings()
    {
        // Arrange
        var corsSettings = new CorsConfiguration();
        _mockCorsSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(corsSettings);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.Cors, Is.SameAs(corsSettings));
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task Export_MapsPermissionPolicyIsEnabled(bool isEnabled)
    {
        // Arrange
        _mockPermissionPolicyRepository.Setup(x => x.GetSettingsAsync()).ReturnsAsync(new PermissionPolicySettingsModel { IsEnabled = isEnabled });

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.PermissionPolicy!.IsEnabled, Is.EqualTo(isEnabled));
    }

    [Test]
    public async Task Export_MapsPermissionPolicyDirectives()
    {
        // Arrange
        var directives = new List<PermissionPolicyDirectiveModel>
        {
            new() { Name = "camera", EnabledState = PermissionPolicyEnabledState.ThisSite }
        };
        _mockPermissionPolicyRepository.Setup(x => x.ListDirectivesAsync()).ReturnsAsync(directives);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.PermissionPolicy!.Directives, Is.SameAs(directives));
    }

    [Test]
    public async Task Export_ReturnsCustomHeadersInModel()
    {
        // Arrange
        var customHeaders = new List<CustomHeader>
        {
            new() { HeaderName = "X-Custom-One", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value-one" },
            new() { HeaderName = "X-Powered-By", Behavior = CustomHeaderBehavior.Remove, HeaderValue = null }
        };
        _mockCustomHeaderRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(customHeaders);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.CustomHeaders, Is.Not.Null);
        Assert.That(result.CustomHeaders, Has.Count.EqualTo(2));
    }

    [Test]
    [TestCase("X-Custom-Header")]
    [TestCase(CspConstants.HeaderNames.CrossOriginEmbedderPolicy)]
    [TestCase(CspConstants.HeaderNames.CrossOriginOpenerPolicy)]
    [TestCase(CspConstants.HeaderNames.CrossOriginResourcePolicy)]
    [TestCase(CspConstants.HeaderNames.FrameOptions)]
    [TestCase(CspConstants.HeaderNames.ReferrerPolicy)]
    [TestCase(CspConstants.HeaderNames.StrictTransportSecurity)]
    [TestCase(CspConstants.HeaderNames.XssProtection)]
    public async Task Export_MapsCustomHeaderNameCorrectly(string headerName)
    {
        // Arrange
        var customHeaders = new List<CustomHeader>
        {
            new() { HeaderName = headerName, Behavior = CustomHeaderBehavior.Add, HeaderValue = "test-value" }
        };
        _mockCustomHeaderRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(customHeaders);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.CustomHeaders!.First().HeaderName, Is.EqualTo(headerName));
    }

    [Test]
    [TestCase(CustomHeaderBehavior.Disabled)]
    [TestCase(CustomHeaderBehavior.Add)]
    [TestCase(CustomHeaderBehavior.Remove)]
    public async Task Export_MapsCustomHeaderBehaviorCorrectly(CustomHeaderBehavior behaviour)
    {
        // Arrange
        var customHeaders = new List<CustomHeader>
        {
            new() { HeaderName = "X-Powered-By", Behavior = behaviour, HeaderValue = null }
        };
        _mockCustomHeaderRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(customHeaders);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.CustomHeaders!.First().Behavior, Is.EqualTo(behaviour));
    }

    [Test]
    [TestCase("value-one")]
    [TestCase("value-two")]
    public async Task Export_MapsCustomHeaderValueCorrectly(string headerValue)
    {
        // Arrange
        var customHeaders = new List<CustomHeader>
        {
            new() { HeaderName = "X-Custom-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = headerValue }
        };
        _mockCustomHeaderRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(customHeaders);

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.CustomHeaders!.First().HeaderValue, Is.EqualTo(headerValue));
    }

    [Test]
    public async Task Export_ReturnsEmptyCustomHeadersWhenNoneExist()
    {
        // Arrange
        _mockCustomHeaderRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CustomHeader>());

        // Act
        var result = await _service.Export();

        // Assert
        Assert.That(result.CustomHeaders, Is.Not.Null);
        Assert.That(result.CustomHeaders, Is.Empty);
    }

    [Test]
    public async Task Import_GivenNullSettings_DoesNotCallRepository()
    {
        // Act
        await _service.Import(null, "test-user");

        // Assert
        _mockMigrationRepository.Verify(x => x.SaveAsync(It.IsAny<SettingsModel>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Import_GivenNullModifiedBy_DoesNotCallRepository()
    {
        // Act
        await _service.Import(new SettingsModel(), null);

        // Assert
        _mockMigrationRepository.Verify(x => x.SaveAsync(It.IsAny<SettingsModel>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Import_GivenEmptyModifiedBy_DoesNotCallRepository()
    {
        // Act
        await _service.Import(new SettingsModel(), string.Empty);

        // Assert
        _mockMigrationRepository.Verify(x => x.SaveAsync(It.IsAny<SettingsModel>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Import_GivenValidSettings_CallsSaveAsyncOnRepository()
    {
        // Arrange
        var settings = new SettingsModel();

        // Act
        await _service.Import(settings, "test-user");

        // Assert
        _mockMigrationRepository.Verify(x => x.SaveAsync(settings, "test-user"), Times.Once);
    }

    [Test]
    public async Task Import_GivenValidSettings_ClearsCacheAfterSave()
    {
        // Arrange
        var settings = new SettingsModel();

        // Act
        await _service.Import(settings, "test-user");

        // Assert
        _mockCacheWrapper.Verify(x => x.RemoveAll(), Times.Once);
    }
}
