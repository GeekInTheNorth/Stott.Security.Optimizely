namespace Stott.Security.Optimizely.Test.Features.Csp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.Csp.Sandbox.Service;
using Stott.Security.Optimizely.Features.Csp.Settings.Service;
using Stott.Security.Optimizely.Features.Pages;
using Stott.Security.Optimizely.Features.Route;

[TestFixture]
public sealed class CspServiceTests
{
    private CspService _cspService;

    private Mock<ICspSettingsService> _mockSettingsService;

    private Mock<ICspSandboxService> _mockSandboxService;

    private Mock<ICspPermissionService> _mockPermissionService;

    private Mock<TestPageData> _mockPage;

    private SecurityRouteData _routeData;

    [SetUp]
    public void SetUp()
    {
        _mockSettingsService = new Mock<ICspSettingsService>();

        _mockSandboxService = new Mock<ICspSandboxService>();

        _mockPermissionService = new Mock<ICspPermissionService>();

        _mockPage = new Mock<TestPageData>(MockBehavior.Loose);

        _routeData = new SecurityRouteData
        {
            Content = _mockPage.Object,
            RouteType = SecurityRouteType.Default
        };

        _cspService = new CspService(
            _mockSettingsService.Object,
            _mockPermissionService.Object,
            _mockSandboxService.Object);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenNullCspSourcesAndNullPageSources_NoHeadersAreReturned()
    {
        // Arrange
        SetupCspSettings(true);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync((IList<CspSource>)null);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);

        // Assert
        Assert.That(policy, Is.Empty);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenEmptyCspSourcesAndNullPageSources_NoHeadersAreReturned()
    {
        // Arrange
        SetupCspSettings(true);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync([]);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);

        // Assert
        Assert.That(policy, Is.Empty);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenNullCspSourcesAndEmptyPageSources_NoHeadersAreReturned()
    {
        // Arrange
        SetupCspSettings(true);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync((IList<CspSource>)null);
        _mockPage.Setup(x => x.ContentSecurityPolicySources).Returns([]);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);

        // Assert
        Assert.That(policy, Is.Empty);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenEmptyCspSourcesAndEmptyPageSources_NoHeadersAreReturned()
    {
        // Arrange
        SetupCspSettings(true);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync([]);
        _mockPage.Setup(x => x.ContentSecurityPolicySources).Returns([]);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);

        // Assert
        Assert.That(policy, Is.Empty);
    }

    [Test]
    [TestCaseSource(typeof(CspServiceTestCases), nameof(CspServiceTestCases.NonMatchingSourceTestCases))]
    public async Task GetCompiledHeaders_GivenMultipleRecords_ThenThePolicyShouldContainEntriesForAllRecords(
        List<CspSource> sources,
        string expectedPolicy)
    {
        // Arrange
        SetupCspSettings(true);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var cspHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy);

        // Assert
        Assert.That(cspHeader, Is.Not.Null);
        Assert.That(cspHeader.Value, Is.EqualTo(expectedPolicy));
    }

    [Test]
    [TestCaseSource(typeof(CspServiceTestCases), nameof(CspServiceTestCases.MultipleMatchingSourceTestCases))]
    public async Task GetCompiledHeaders_GivenMultipleRecordsWithMatchingSources_ThenDirectivesShouldContainUniqueSources(
        List<CspSource> sources,
        string expectedPolicy)
    {
        // Arrange
        SetupCspSettings(true);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var cspHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy);

        // Assert
        Assert.That(cspHeader, Is.Not.Null);
        Assert.That(cspHeader.Value, Is.EqualTo(expectedPolicy));

    }

    [Test]
    public async Task GetCompiledHeaders_GivenAVarietyOfAllSourceTypes_ThenSourcesShouldBeCorrectlyOrdered()
    {
        // Arrange
        var sources = CspConstants.AllSources
                                  .Where(x => !x.Equals(CspConstants.Sources.None))
                                  .Select(x => new CspSource { Source = x, Directives = CspConstants.Directives.DefaultSource })
                                  .OrderBy(x => Guid.NewGuid())
                                  .ToList();
        sources.Add(new CspSource { Source = "https://www.example.com", Directives = CspConstants.Directives.DefaultSource });

        SetupCspSettings(true);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy);
        var expectedHeader = "default-src 'nonce-random' 'strict-dynamic' 'self' 'unsafe-eval' 'wasm-unsafe-eval' 'unsafe-inline' 'unsafe-hashes' 'inline-speculation-rules' blob: data: filesystem: http: https: ws: wss: mediastream: https://www.example.com;";

        // Assert
        Assert.That(actualHeader.Value, Is.EqualTo(expectedHeader));
    }

    [Test]
    public async Task GetCompiledHeaders_GivenReportingIsNotConfigured_ThenReportToShouldBeAbsent()
    {
        // Arrange
        var sources = new List<CspSource>
        {
            new() { Source = "https://www.example.com", Directives = CspConstants.Directives.DefaultSource }
        };

        SetupCspSettings(true, false, false, false);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy);
        var expectedHeader = "default-src https://www.example.com;";

        // Assert
        Assert.That(actualHeader.Value, Is.EqualTo(expectedHeader));
    }

    [Test]
    public async Task GetCompiledHeaders_GivenReportingIsConfiguredWithAReportingUrl_ThenReportToShouldBePresent()
    {
        // Arrange
        var sources = new List<CspSource>
        {
            new() { Source = "https://www.example.com", Directives = CspConstants.Directives.DefaultSource }
        };

        SetupCspSettings(true, false, true, false);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.First(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy);

        // Assert
        Assert.That(actualHeader.Value.Contains(CspConstants.Directives.ReportTo), Is.True);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCspSettingsAreAbsentThenTheSandboxIsNotIncluded()
    {
        // Arrange
        var sandbox = new SandboxModel { IsSandboxEnabled = true };
        var sources = new List<CspSource>
        {
            new() { Source = "https://www.example.com", Directives = CspConstants.Directives.DefaultSource }
        };

        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);
        _mockSandboxService.Setup(x => x.GetAsync()).ReturnsAsync(sandbox);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy);
        var actualHeaderValue = actualHeader?.Value ?? string.Empty;

        // Assert
        Assert.That(actualHeaderValue.Contains(CspConstants.Directives.Sandbox), Is.False);
    }

    [Test]
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task GetCompiledHeaders_GivenCspIsNotEnabledOrIsReportOnlyThenSandboxShouldBeAbsent(bool isEnabled, bool isReportOnly)
    {
        // Arrange
        var sandbox = new SandboxModel { IsSandboxEnabled = true, IsAllowDownloadsEnabled = true };
        var sources = new List<CspSource>
        {
            new() { Source = "https://www.example.com", Directives = CspConstants.Directives.DefaultSource }
        };

        SetupCspSettings(isEnabled, isReportOnly);
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);
        _mockSandboxService.Setup(x => x.GetAsync()).ReturnsAsync(sandbox);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy || x.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        var actualHeaderValue = actualHeader?.Value ?? string.Empty;

        // Assert
        Assert.That(actualHeaderValue.Contains(CspConstants.Directives.Sandbox), Is.False);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenSandboxIsDisabledButCspIsActiveThenSandboxShouldBeAbsent()
    {
        // Arrange
        SetupCspSettings(true, false);

        var sandbox = new SandboxModel { IsSandboxEnabled = false };
        _mockSandboxService.Setup(x => x.GetAsync()).ReturnsAsync(sandbox);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy || x.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        var actualHeaderValue = actualHeader?.Value ?? string.Empty;

        // Assert
        Assert.That(actualHeaderValue.Contains(CspConstants.Directives.Sandbox), Is.False);
    }

    [Test]
    [TestCaseSource(typeof(CspServiceTestCases), nameof(CspServiceTestCases.GetSandboxContentTestCases))]
    public async Task GetCompiledHeaders_GivenSandboxIsEnabledThenValuesShouldBeAddedAccordingly(
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
        bool isAllowTopNavigationToCustomProtocolEnabled,
        string expectedSandbox)
    {
        // Arrange
        SetupCspSettings(true, false);
        var sandbox = new SandboxModel
        {
            IsSandboxEnabled = true,
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
        _mockSandboxService.Setup(x => x.GetAsync()).ReturnsAsync(sandbox);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy || x.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        var actualHeaderValue = actualHeader.Value ?? string.Empty;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(actualHeaderValue, Is.Not.Null);
            Assert.That(actualHeaderValue, Is.EqualTo(expectedSandbox));
        });
    }

    [Test]
    [TestCaseSource(typeof(CspServiceTestCases), nameof(CspServiceTestCases.GetNoneKeywordTestCases))]
    public async Task GetCompiledHeaders_GivenDirectiveContainsTheNoneKeywordThenItShouldOnlyContainTheNoneKeyword(
        string noneDirectives,
        string otherSource,
        string otherDirectives,
        string expectedPolicy)
    {
        // Arrange
        SetupCspSettings(true, false);
        var sources = new List<CspSource>
        {
            new() { Source = CspConstants.Sources.None, Directives = noneDirectives },
            new() { Source = otherSource, Directives = otherDirectives }
        };
        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy || x.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        var actualHeaderValue = actualHeader.Value ?? string.Empty;

        // Assert
        Assert.That(actualHeaderValue, Is.EqualTo(expectedPolicy));
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCspSettingsIsNull_ThenTheUpgradeInsecureRequestsDirectiveShouldBeAbsent()
    {
        // Arrange
        _mockSettingsService.Setup(x => x.GetAsync()).ReturnsAsync((CspSettings)null);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy || x.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        var actualHeaderValue = actualHeader?.Value ?? string.Empty;

        // Assert
        Assert.That(actualHeaderValue.Contains(CspConstants.Directives.UpgradeInsecureRequests), Is.False);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCspSettingsIsPresentAndUpgradeInsecureRequestsIsFalse_ThenTheUpgradeInsecureRequestsDirectiveShouldBeAbsent()
    {
        // Arrange
        SetupCspSettings(true, isUpgradeInsecureRequestsEnabled: false);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy || x.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        var actualHeaderValue = actualHeader?.Value ?? string.Empty;

        // Assert
        Assert.That(actualHeaderValue.Contains(CspConstants.Directives.UpgradeInsecureRequests), Is.False);
    }

    [Test]
    public async Task GetCompiledHeaders_GivenCspSettingsIsPresentAndUpgradeInsecureRequestsIsTrue_ThenTheUpgradeInsecureRequestsDirectiveShouldBePresent()
    {
        // Arrange
        SetupCspSettings(true, isUpgradeInsecureRequestsEnabled: true);

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy || x.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        var actualHeaderValue = actualHeader.Value ?? string.Empty;

        // Assert
        Assert.That(actualHeaderValue.Contains($"{CspConstants.Directives.UpgradeInsecureRequests};"), Is.True);
    }

    [Test]
    [TestCase(SecurityRouteType.Default, true)]
    [TestCase(SecurityRouteType.ContentSpecific, true)]
    [TestCase(SecurityRouteType.NoNonceOrHash, false)]
    [TestCase(SecurityRouteType.ContentSpecificNoNonceOrHash, false)]
    public async Task GetCompiledHeaders_WhenOnANoNonceOrHashRoute_ThenHashesAndUnsafeHashesAreRemovedFromGlobalSources(SecurityRouteType routeType, bool expectedValue)
    {
        // Arrange
        SetupCspSettings(true, isUpgradeInsecureRequestsEnabled: true);
        var sources = new List<CspSource>
        {
            new() { Source = CspConstants.Sources.UnsafeHashes, Directives = CspConstants.Directives.ScriptSource },
            new() { Source = "'sha256-abc'", Directives = CspConstants.Directives.ScriptSource },
            new() { Source = "'sha384-def'", Directives = CspConstants.Directives.ScriptSource },
            new() { Source = "'sha512-ghi'", Directives = CspConstants.Directives.ScriptSource },
            new() { Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.ScriptSource }
        };

        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(sources);
        _routeData.RouteType = routeType;

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy || x.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        var actualHeaderValue = actualHeader.Value ?? string.Empty;

        // Assert
        Assert.That(actualHeaderValue.Contains(CspConstants.Sources.UnsafeHashes), Is.EqualTo(expectedValue));
        Assert.That(actualHeaderValue.Contains("'sha256-"), Is.EqualTo(expectedValue));
        Assert.That(actualHeaderValue.Contains("'sha384-"), Is.EqualTo(expectedValue));
        Assert.That(actualHeaderValue.Contains("'sha512-"), Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCase(SecurityRouteType.Default, true)]
    [TestCase(SecurityRouteType.ContentSpecific, true)]
    [TestCase(SecurityRouteType.NoNonceOrHash, false)]
    [TestCase(SecurityRouteType.ContentSpecificNoNonceOrHash, false)]
    public async Task GetCompiledHeaders_WhenOnANoNonceOrHashRoute_ThenHashesAndUnsafeHashesAreRemovedFromPageSources(SecurityRouteType routeType, bool expectedValue)
    {
        // Arrange
        SetupCspSettings(true, isUpgradeInsecureRequestsEnabled: true);
        var sources = new List<PageCspSourceMapping>
        {
            new() { Source = CspConstants.Sources.UnsafeHashes, Directives = CspConstants.Directives.ScriptSource },
            new() { Source = "'sha256-abc'", Directives = CspConstants.Directives.ScriptSource },
            new() { Source = "'sha384-def'", Directives = CspConstants.Directives.ScriptSource },
            new() { Source = "'sha512-ghi'", Directives = CspConstants.Directives.ScriptSource },
            new() { Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.ScriptSource }
        };

        _mockPermissionService.Setup(x => x.GetAsync()).ReturnsAsync(new List<CspSource>());
        _mockPage.Setup(x => x.ContentSecurityPolicySources).Returns(sources);
        _routeData.RouteType = routeType;

        // Act
        var policy = await _cspService.GetCompiledHeaders(_routeData);
        var actualHeader = policy.FirstOrDefault(x => x.Key == CspConstants.HeaderNames.ContentSecurityPolicy || x.Key == CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy);
        var actualHeaderValue = actualHeader.Value ?? string.Empty;

        // Assert
        Assert.That(actualHeaderValue.Contains(CspConstants.Sources.UnsafeHashes), Is.EqualTo(expectedValue));
        Assert.That(actualHeaderValue.Contains("'sha256-"), Is.EqualTo(expectedValue));
        Assert.That(actualHeaderValue.Contains("'sha384-"), Is.EqualTo(expectedValue));
        Assert.That(actualHeaderValue.Contains("'sha512-"), Is.EqualTo(expectedValue));
    }

    private void SetupCspSettings(bool isEnabled, bool isReportOnly = false, bool useInternalReporting = false, bool useExternalReporting = false, bool isUpgradeInsecureRequestsEnabled = false)
    {
        var settings = new CspSettings
        {
            IsEnabled = isEnabled,
            IsReportOnly = isReportOnly,
            IsAllowListEnabled = false,
            IsNonceEnabled = false,
            IsStrictDynamicEnabled = false,
            IsUpgradeInsecureRequestsEnabled = isUpgradeInsecureRequestsEnabled,
            UseExternalReporting = useExternalReporting,
            UseInternalReporting = useInternalReporting,
            ExternalReportToUrl = useExternalReporting ? "https://www.example.com" : string.Empty
        };

        _mockSettingsService.Setup(x => x.GetAsync()).ReturnsAsync(settings);
    }
}