using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Nonce;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;
using Stott.Security.Optimizely.Features.Route;

namespace Stott.Security.Optimizely.Test.Features.Csp.Nonce;

[TestFixture]
public sealed class NonceServiceTests
{
    private Mock<ICspSettingsRepository> _mockCspSettingsRepository;

    private Mock<ICspPermissionRepository> _mockCspPermissionRepository;

    private Mock<ICacheWrapper> _mockCache;

    private NonceService _service;

    private SecurityRouteData _routeData;

    private const string TestAppId = "test-app-id";

    private const string TestHostName = "test-host-name";

    [SetUp]
    public void SetUp()
    {
        _mockCspSettingsRepository = new Mock<ICspSettingsRepository>();
        _mockCspPermissionRepository = new Mock<ICspPermissionRepository>();
        _mockCache = new Mock<ICacheWrapper>();

        _routeData = new SecurityRouteData
        {
            AppId = TestAppId,
            HostName = TestHostName,
            RouteType = SecurityRouteType.Default
        };

        _service = new NonceService(
            _mockCspSettingsRepository.Object,
            _mockCspPermissionRepository.Object,
            _mockCache.Object);
    }

    [Test]
    public async Task GivenCacheExists_ThenTheCachedSettingsShouldBeReturned()
    {
        var expectedSettings = new NonceSettings
        {
            IsEnabled = true,
            Directives = [CspConstants.Directives.ScriptSource]
        };

        _mockCache
            .Setup(x => x.Get<NonceSettings>(It.IsAny<string>()))
            .Returns(expectedSettings);

        var result = await _service.GetNonceSettingsAsync(_routeData);

        Assert.That(result.IsEnabled, Is.EqualTo(expectedSettings.IsEnabled));
        Assert.That(result.Directives, Is.EqualTo(expectedSettings.Directives));
        _mockCspSettingsRepository.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GivenCacheAndCspSettingsAreMissing_ThenNonceShouldBeDisabled()
    {
        var result = await _service.GetNonceSettingsAsync(_routeData);

        Assert.That(result.IsEnabled, Is.False);
        Assert.That(result.Directives, Is.Null);
        _mockCspSettingsRepository.Verify(x => x.GetAsync(TestAppId, TestHostName), Times.Once);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GivenCacheIsMissingAndCspSettingsAreDisabled_ThenNonceShouldBeDisabled()
    {
        _mockCache
            .Setup(x => x.Get<NonceSettings>(It.IsAny<string>()))
            .Returns((NonceSettings)null);

        _mockCspSettingsRepository
            .Setup(x => x.GetAsync(TestAppId, TestHostName))
            .ReturnsAsync(new CspSettings { IsEnabled = false });

        var result = await _service.GetNonceSettingsAsync(_routeData);

        Assert.That(result.IsEnabled, Is.False);
        Assert.That(result.Directives, Is.Null);
        _mockCspSettingsRepository.Verify(x => x.GetAsync(TestAppId, TestHostName), Times.Once);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GivenCacheIsMissingAndCspSettingsAreEnabledButNoNonceSource_ThenNonceShouldBeDisabled()
    {
        _mockCache
            .Setup(x => x.Get<NonceSettings>(It.IsAny<string>()))
            .Returns((NonceSettings)null);
        _mockCspSettingsRepository
            .Setup(x => x.GetAsync(TestAppId, TestHostName))
            .ReturnsAsync(new CspSettings { IsEnabled = true });
        _mockCspPermissionRepository
            .Setup(x => x.GetBySourceAsync(CspConstants.Sources.Nonce, TestAppId, TestHostName))
            .ReturnsAsync((CspSource)null);

        var result = await _service.GetNonceSettingsAsync(_routeData);

        Assert.That(result.IsEnabled, Is.False);
        Assert.That(result.Directives, Is.Null);
        _mockCspSettingsRepository.Verify(x => x.GetAsync(TestAppId, TestHostName), Times.Once);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(CspConstants.Sources.Nonce, TestAppId, TestHostName), Times.Once);
    }

    [Test]
    public async Task GivenCacheIsMissingAndCspSettingsAndNonceSourceAreEnabled_ThenNonceShouldBeReturned()
    {
        _mockCache
            .Setup(x => x.Get<NonceSettings>(It.IsAny<string>()))
            .Returns((NonceSettings)null);
        _mockCspSettingsRepository
            .Setup(x => x.GetAsync(TestAppId, TestHostName))
            .ReturnsAsync(new CspSettings { IsEnabled = true, IsNonceEnabled = true });
        _mockCspPermissionRepository
            .Setup(x => x.GetBySourceAsync(CspConstants.Sources.Nonce, TestAppId, TestHostName))
            .ReturnsAsync(new CspSource
            { Directives = $"{CspConstants.Directives.ScriptSource}, {CspConstants.Directives.StyleSource}" });
        var result = await _service.GetNonceSettingsAsync(_routeData);

        Assert.That(result.IsEnabled, Is.True);
        Assert.That(result.Directives, Is.EquivalentTo([CspConstants.Directives.ScriptSource, CspConstants.Directives.StyleSource]));
        _mockCspSettingsRepository.Verify(x => x.GetAsync(TestAppId, TestHostName), Times.Once);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(CspConstants.Sources.Nonce, TestAppId, TestHostName), Times.Once);
    }
}
