using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Csp.Nonce;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;
using Stott.Security.Optimizely.Features.Csp.Settings.Repository;

namespace Stott.Security.Optimizely.Test.Features.Csp.Nonce;

[TestFixture]
public sealed class NonceServiceTests
{
    private Mock<ICspSettingsRepository> _mockCspSettingsRepository;

    private Mock<ICspPermissionRepository> _mockCspPermissionRepository;

    private Mock<ICacheWrapper> _mockCache;

    private NonceService _service;

    [SetUp]
    public void SetUp()
    {
        _mockCspSettingsRepository = new Mock<ICspSettingsRepository>();
        _mockCspPermissionRepository = new Mock<ICspPermissionRepository>();
        _mockCache = new Mock<ICacheWrapper>();

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

        var result = await _service.GetNonceSettingsAsync();

        Assert.That(result.IsEnabled, Is.EqualTo(expectedSettings.IsEnabled));
        Assert.That(result.Directives, Is.EqualTo(expectedSettings.Directives));
        _mockCspSettingsRepository.Verify(x => x.GetAsync(), Times.Never);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GivenCacheAndCspSettingsAreMissing_ThenNonceShouldBeDisabled()
    {
        var result = await _service.GetNonceSettingsAsync();

        Assert.That(result.IsEnabled, Is.False);
        Assert.That(result.Directives, Is.Null);
        _mockCspSettingsRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GivenCacheIsMissingAndCspSettingsAreDisabled_ThenNonceShouldBeDisabled()
    {
        _mockCache
            .Setup(x => x.Get<NonceSettings>(It.IsAny<string>()))
            .Returns((NonceSettings)null);

        _mockCspSettingsRepository
            .Setup(x => x.GetAsync())
            .ReturnsAsync(new CspSettings { IsEnabled = false });

        var result = await _service.GetNonceSettingsAsync();

        Assert.That(result.IsEnabled, Is.False);
        Assert.That(result.Directives, Is.Null);
        _mockCspSettingsRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task GivenCacheIsMissingAndCspSettingsAreEnabledButNoNonceSource_ThenNonceShouldBeDisabled()
    {
        _mockCache
            .Setup(x => x.Get<NonceSettings>(It.IsAny<string>()))
            .Returns((NonceSettings)null);
        _mockCspSettingsRepository
            .Setup(x => x.GetAsync())
            .ReturnsAsync(new CspSettings { IsEnabled = true });
        _mockCspPermissionRepository
            .Setup(x => x.GetBySourceAsync(CspConstants.Sources.Nonce))
            .ReturnsAsync((CspSource)null);

        var result = await _service.GetNonceSettingsAsync();

        Assert.That(result.IsEnabled, Is.False);
        Assert.That(result.Directives, Is.Null);
        _mockCspSettingsRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task GivenCacheIsMissingAndCspSettingsAndNonceSourceAreEnabled_ThenNonceShouldBeReturned()
    {
        _mockCache
            .Setup(x => x.Get<NonceSettings>(It.IsAny<string>()))
            .Returns((NonceSettings)null);
        _mockCspSettingsRepository
            .Setup(x => x.GetAsync())
            .ReturnsAsync(new CspSettings { IsEnabled = true, IsNonceEnabled = true });
        _mockCspPermissionRepository
            .Setup(x => x.GetBySourceAsync(CspConstants.Sources.Nonce))
            .ReturnsAsync(new CspSource
            { Directives = $"{CspConstants.Directives.ScriptSource}, {CspConstants.Directives.StyleSource}" });
        var result = await _service.GetNonceSettingsAsync();

        Assert.That(result.IsEnabled, Is.True);
        Assert.That(result.Directives, Is.EquivalentTo(new []{CspConstants.Directives.ScriptSource, CspConstants.Directives.StyleSource}));
        _mockCspSettingsRepository.Verify(x => x.GetAsync(), Times.Once);
        _mockCspPermissionRepository.Verify(x => x.GetBySourceAsync(It.IsAny<string>()), Times.Once);
    }   
}