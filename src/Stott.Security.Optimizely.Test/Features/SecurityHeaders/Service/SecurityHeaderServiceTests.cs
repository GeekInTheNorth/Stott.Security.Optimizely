namespace Stott.Security.Optimizely.Test.Features.SecurityHeaders.Service;

using System;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;
using Stott.Security.Optimizely.Features.SecurityHeaders.Service;

[TestFixture]
public sealed class SecurityHeaderServiceTests
{
    private Mock<ISecurityHeaderRepository> _mockRepository;

    private Mock<ICacheWrapper> _mockCache;

    private SecurityHeaderService _service;

    private const string UserName = "test.user";

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ISecurityHeaderRepository>();

        _mockCache = new Mock<ICacheWrapper>();

        _service = new SecurityHeaderService(_mockRepository.Object, _mockCache.Object);
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionWhenGivenANullRepository()
    {
        Assert.Throws<ArgumentNullException>(() => new SecurityHeaderService(null, _mockCache.Object));
    }

    [Test]
    public void Constructor_ThrowsArgumentNullExceptionWhenGivenANullCache()
    {
        Assert.Throws<ArgumentNullException>(() => new SecurityHeaderService(_mockRepository.Object, null));
    }

    [Test]
    public void Constructor_DoesNotThrowsAnExceptionWhenGivenAValidParameters()
    {
        Assert.DoesNotThrow(() => new SecurityHeaderService(_mockRepository.Object, _mockCache.Object));
    }

    [Test]
    public async Task GetAsync_CallsGetAsyncOnTheRepository()
    {
        // Act
        _ = await _service.GetAsync();

        // Verify
        _mockRepository.Verify(x => x.GetAsync(), Times.Once);
    }

    [Test]
    [TestCase(XContentTypeOptions.None, XssProtection.Enabled, ReferrerPolicy.NoReferrer, XFrameOptions.Deny)]
    [TestCase(XContentTypeOptions.NoSniff, XssProtection.EnabledWithBlocking, ReferrerPolicy.UnsafeUrl, XFrameOptions.SameOrigin)]
    public async Task SaveAsync_SavesANewSettingsRecordWhenOneDoesNotExist_ForOtherSecurityHeaders(
        XContentTypeOptions xContentTypeOptions,
        XssProtection xssProtection,
        ReferrerPolicy referrerPolicy,
        XFrameOptions xFrameOptions)
    {
        // Arrange
        SecurityHeaderSettings savedRecord = null;
        _mockRepository.Setup(x => x.GetAsync()).ReturnsAsync((SecurityHeaderSettings)null);
        _mockRepository.Setup(x => x.SaveAsync(It.IsAny<SecurityHeaderSettings>())).Callback<SecurityHeaderSettings>(x => savedRecord = x);

        // Act
        await _service.SaveAsync(xContentTypeOptions, xssProtection, referrerPolicy, xFrameOptions, UserName);

        // Verify
        Assert.Multiple(() =>
        {
            _mockRepository.Verify(x => x.SaveAsync(It.IsAny<SecurityHeaderSettings>()), Times.Once);
            Assert.That(savedRecord, Is.Not.Null);
            Assert.That(savedRecord.Id, Is.EqualTo(Guid.Empty));
            Assert.That(savedRecord.XContentTypeOptions, Is.EqualTo(xContentTypeOptions));
            Assert.That(savedRecord.XssProtection, Is.EqualTo(xssProtection));
            Assert.That(savedRecord.ReferrerPolicy, Is.EqualTo(referrerPolicy));
            Assert.That(savedRecord.FrameOptions, Is.EqualTo(xFrameOptions));
            Assert.That(savedRecord.Modified, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(3)));
            Assert.That(savedRecord.ModifiedBy, Is.EqualTo(UserName));
        });
    }

    [Test]
    [TestCase(XContentTypeOptions.None, XssProtection.Enabled, ReferrerPolicy.NoReferrer, XFrameOptions.Deny)]
    [TestCase(XContentTypeOptions.NoSniff, XssProtection.EnabledWithBlocking, ReferrerPolicy.UnsafeUrl, XFrameOptions.SameOrigin)]
    public async Task SaveAsync_UpdatesExistingSettingsWhenARecordExists_ForOtherSecurityHeaders(
        XContentTypeOptions xContentTypeOptions,
        XssProtection xssProtection,
        ReferrerPolicy referrerPolicy,
        XFrameOptions xFrameOptions)
    {
        // Arrange
        var existingSettings = new SecurityHeaderSettings { Id = Guid.NewGuid(), };

        _mockRepository.Setup(x => x.GetAsync()).ReturnsAsync(existingSettings);

        // Act
        await _service.SaveAsync(xContentTypeOptions, xssProtection, referrerPolicy, xFrameOptions, UserName);

        // Verify
        Assert.Multiple(() =>
        {
            _mockRepository.Verify(x => x.SaveAsync(It.IsAny<SecurityHeaderSettings>()), Times.Once);
            _mockRepository.Verify(x => x.SaveAsync(existingSettings), Times.Once);
            Assert.That(existingSettings.XContentTypeOptions, Is.EqualTo(xContentTypeOptions));
            Assert.That(existingSettings.XssProtection, Is.EqualTo(xssProtection));
            Assert.That(existingSettings.ReferrerPolicy, Is.EqualTo(referrerPolicy));
            Assert.That(existingSettings.FrameOptions, Is.EqualTo(xFrameOptions));
            Assert.That(existingSettings.Modified, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(3)));
            Assert.That(existingSettings.ModifiedBy, Is.EqualTo(UserName));
        });
    }

    [Test]
    [TestCase(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginOpenerPolicy.SameOrigin, CrossOriginResourcePolicy.SameOrigin)]
    [TestCase(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginOpenerPolicy.UnsafeNone, CrossOriginResourcePolicy.CrossOrigin)]
    public async Task SaveAsync_SavesANewSettingsRecordWhenOneDoesNotExist_ForCrossOriginHeaders(
        CrossOriginEmbedderPolicy crossOriginEmbedderPolicy,
        CrossOriginOpenerPolicy crossOriginOpenerPolicy,
        CrossOriginResourcePolicy crossOriginResourcePolicy)
    {
        // Arrange
        SecurityHeaderSettings savedRecord = null;
        _mockRepository.Setup(x => x.GetAsync()).ReturnsAsync((SecurityHeaderSettings)null);
        _mockRepository.Setup(x => x.SaveAsync(It.IsAny<SecurityHeaderSettings>())).Callback<SecurityHeaderSettings>(x => savedRecord = x);

        // Act
        await _service.SaveAsync(crossOriginEmbedderPolicy, crossOriginOpenerPolicy, crossOriginResourcePolicy, UserName);

        // Verify
        Assert.Multiple(() =>
        {
            _mockRepository.Verify(x => x.SaveAsync(It.IsAny<SecurityHeaderSettings>()), Times.Once);
            Assert.That(savedRecord, Is.Not.Null);
            Assert.That(savedRecord.Id, Is.EqualTo(Guid.Empty));
            Assert.That(savedRecord.CrossOriginEmbedderPolicy, Is.EqualTo(crossOriginEmbedderPolicy));
            Assert.That(savedRecord.CrossOriginOpenerPolicy, Is.EqualTo(crossOriginOpenerPolicy));
            Assert.That(savedRecord.CrossOriginResourcePolicy, Is.EqualTo(crossOriginResourcePolicy));
            Assert.That(savedRecord.Modified, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(3)));
            Assert.That(savedRecord.ModifiedBy, Is.EqualTo(UserName));
        });
    }

    [Test]
    [TestCase(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginOpenerPolicy.SameOrigin, CrossOriginResourcePolicy.SameOrigin)]
    [TestCase(CrossOriginEmbedderPolicy.RequireCorp, CrossOriginOpenerPolicy.UnsafeNone, CrossOriginResourcePolicy.CrossOrigin)]
    public async Task SaveAsync_UpdatesExistingSettingsWhenARecordExists_ForCrossOriginHeaders(
        CrossOriginEmbedderPolicy crossOriginEmbedderPolicy,
        CrossOriginOpenerPolicy crossOriginOpenerPolicy,
        CrossOriginResourcePolicy crossOriginResourcePolicy)
    {
        // Arrange
        var existingSettings = new SecurityHeaderSettings { Id = Guid.NewGuid(), };

        _mockRepository.Setup(x => x.GetAsync()).ReturnsAsync(existingSettings);

        // Act
        await _service.SaveAsync(crossOriginEmbedderPolicy, crossOriginOpenerPolicy, crossOriginResourcePolicy, UserName);

        // Verify
        Assert.Multiple(() =>
        {
            _mockRepository.Verify(x => x.SaveAsync(It.IsAny<SecurityHeaderSettings>()), Times.Once);
            _mockRepository.Verify(x => x.SaveAsync(existingSettings), Times.Once);
            Assert.That(existingSettings.CrossOriginEmbedderPolicy, Is.EqualTo(crossOriginEmbedderPolicy));
            Assert.That(existingSettings.CrossOriginOpenerPolicy, Is.EqualTo(crossOriginOpenerPolicy));
            Assert.That(existingSettings.CrossOriginResourcePolicy, Is.EqualTo(crossOriginResourcePolicy));
            Assert.That(existingSettings.Modified, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(3)));
            Assert.That(existingSettings.ModifiedBy, Is.EqualTo(UserName));
        });
    }

    [Test]
    [TestCase(true, false, 123)]
    [TestCase(false, true, 456)]
    public async Task SaveAsync_SavesANewSettingsRecordWhenOneDoesNotExist_ForStrictTransportSecurityHeaders(
        bool isStrictTransportSecurityEnabled,
        bool isStrictTransportSecuritySubDomainsEnabled,
        int strictTransportSecurityMaxAge)
    {
        // Arrange
        SecurityHeaderSettings savedRecord = null;
        _mockRepository.Setup(x => x.GetAsync()).ReturnsAsync((SecurityHeaderSettings)null);
        _mockRepository.Setup(x => x.SaveAsync(It.IsAny<SecurityHeaderSettings>())).Callback<SecurityHeaderSettings>(x => savedRecord = x);

        // Act
        await _service.SaveAsync(isStrictTransportSecurityEnabled, isStrictTransportSecuritySubDomainsEnabled, strictTransportSecurityMaxAge, UserName);

        // Verify
        Assert.Multiple(() =>
        {
            _mockRepository.Verify(x => x.SaveAsync(It.IsAny<SecurityHeaderSettings>()), Times.Once);
            Assert.That(savedRecord, Is.Not.Null);
            Assert.That(savedRecord.Id, Is.EqualTo(Guid.Empty));
            Assert.That(savedRecord.IsStrictTransportSecurityEnabled, Is.EqualTo(isStrictTransportSecurityEnabled));
            Assert.That(savedRecord.IsStrictTransportSecuritySubDomainsEnabled, Is.EqualTo(isStrictTransportSecuritySubDomainsEnabled));
            Assert.That(savedRecord.StrictTransportSecurityMaxAge, Is.EqualTo(strictTransportSecurityMaxAge));
            Assert.That(savedRecord.Modified, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(3)));
            Assert.That(savedRecord.ModifiedBy, Is.EqualTo(UserName));
        });
    }

    [Test]
    [TestCase(true, false, 123)]
    [TestCase(false, true, 456)]
    public async Task SaveAsync_UpdatesExistingSettingsWhenARecordExists_ForStrictTransportSecurityHeaders(
        bool isStrictTransportSecurityEnabled,
        bool isStrictTransportSecuritySubDomainsEnabled,
        int strictTransportSecurityMaxAge)
    {
        // Arrange
        var existingSettings = new SecurityHeaderSettings { Id = Guid.NewGuid(), };

        _mockRepository.Setup(x => x.GetAsync()).ReturnsAsync(existingSettings);

        // Act
        await _service.SaveAsync(isStrictTransportSecurityEnabled, isStrictTransportSecuritySubDomainsEnabled, strictTransportSecurityMaxAge, UserName);

        // Verify
        Assert.Multiple(() =>
        {
            _mockRepository.Verify(x => x.SaveAsync(It.IsAny<SecurityHeaderSettings>()), Times.Once);
            Assert.That(existingSettings.IsStrictTransportSecurityEnabled, Is.EqualTo(isStrictTransportSecurityEnabled));
            Assert.That(existingSettings.IsStrictTransportSecuritySubDomainsEnabled, Is.EqualTo(isStrictTransportSecuritySubDomainsEnabled));
            Assert.That(existingSettings.StrictTransportSecurityMaxAge, Is.EqualTo(strictTransportSecurityMaxAge));
            Assert.That(existingSettings.Modified, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(3)));
            Assert.That(existingSettings.ModifiedBy, Is.EqualTo(UserName));
        });
    }

    [Test]
    public async Task SaveAsync_ClearsTheCompiledCspCacheAfterSaving_ForOtherSecurityHeaders()
    {
        // Act
        await _service.SaveAsync(XContentTypeOptions.None, XssProtection.None, ReferrerPolicy.None, XFrameOptions.None, UserName);

        // Verify
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task SaveAsync_ClearsTheCompiledCspCacheAfterSaving_ForCrossOriginHeaders()
    {
        // Act
        await _service.SaveAsync(CrossOriginEmbedderPolicy.None, CrossOriginOpenerPolicy.None, CrossOriginResourcePolicy.None, UserName);

        // Verify
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    [Test]
    public async Task SaveAsync_ClearsTheCompiledCspCacheAfterSaving_ForStrictTransportSecurityHeaders()
    {
        // Act
        await _service.SaveAsync(false, false, 0, UserName);

        // Verify
        _mockCache.Verify(x => x.RemoveAll(), Times.Once);
    }

    // public async Task GetCompiledHeaders

    [Test]
    public async Task GetCompiledHeaders_MiscellaneousSecurityHeadersAreAbsentWhenSecurityHeaderRepositoryReturnsANull()
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAsync()).ReturnsAsync((SecurityHeaderSettings)null);

        // Act
        var headers = await _service.GetCompiledHeaders();

        // Assert
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.ContentTypeOptions)), Is.False);
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.XssProtection)), Is.False);
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.ReferrerPolicy)), Is.False);
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.FrameOptions)), Is.False);
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.CrossOriginEmbedderPolicy)), Is.False);
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.CrossOriginOpenerPolicy)), Is.False);
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.CrossOriginResourcePolicy)), Is.False);
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.StrictTransportSecurity)), Is.False);
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetXContentTypeOptionsHeaderTestCases))]
    public async Task GetCompiledHeaders_XContentTypeOptionsHeaderIsPresentWhenEnabled(XContentTypeOptions xContentTypeOptions, bool shouldExist)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAsync())
                       .ReturnsAsync(new SecurityHeaderSettings { XContentTypeOptions = xContentTypeOptions });

        // Act
        var headers = await _service.GetCompiledHeaders();

        // Assert
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.ContentTypeOptions)), Is.EqualTo(shouldExist));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetXssProtectionHeaderTestCases))]
    public async Task GetCompiledHeaders_XssProtectionHeaderIsPresentWhenNotSetToNone(XssProtection xssProtection, bool shouldHeaderExist)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAsync())
                       .ReturnsAsync(new SecurityHeaderSettings { XssProtection = xssProtection });

        // Act
        var headers = await _service.GetCompiledHeaders();

        // Assert
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.XssProtection)), Is.EqualTo(shouldHeaderExist));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetReferrerPolicyTestCases))]
    public async Task GetCompiledHeaders_ReferrerPolicyHeaderIsPresentWhenNotSetToNone(ReferrerPolicy referrerPolicy, bool headerShouldExist)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAsync())
                       .ReturnsAsync(new SecurityHeaderSettings { ReferrerPolicy = referrerPolicy });

        // Act
        var headers = await _service.GetCompiledHeaders();

        // Assert
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.ReferrerPolicy)), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetFrameOptionsTestCases))]
    public async Task GetCompiledHeaders_FrameOptionsHeaderIsPresentWhenNotSetToNone(XFrameOptions frameOptions, bool headerShouldExist)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAsync())
                       .ReturnsAsync(new SecurityHeaderSettings { FrameOptions = frameOptions });

        // Act
        var headers = await _service.GetCompiledHeaders();

        // Assert
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.FrameOptions)), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetCrossOriginEmbedderPolicyTestCases))]
    public async Task GetCompiledHeaders_CrossOriginEmbedderPolicyHeaderIsPresentWhenNotSetToNone(CrossOriginEmbedderPolicy crossOriginEmbedderPolicy, bool headerShouldExist)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAsync())
                       .ReturnsAsync(new SecurityHeaderSettings { CrossOriginEmbedderPolicy = crossOriginEmbedderPolicy });

        // Act
        var headers = await _service.GetCompiledHeaders();

        // Assert
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.CrossOriginEmbedderPolicy)), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetCrossOriginEmbedderPolicyTestCases))]
    public async Task GetCompiledHeaders_CrossOriginOpenerPolicyHeaderIsPresentWhenNotSetToNone(CrossOriginOpenerPolicy crossOriginEmbedderPolicy, bool headerShouldExist)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAsync())
                       .ReturnsAsync(new SecurityHeaderSettings { CrossOriginOpenerPolicy = crossOriginEmbedderPolicy });

        // Act
        var headers = await _service.GetCompiledHeaders();

        // Assert
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.CrossOriginOpenerPolicy)), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetCrossOriginEmbedderPolicyTestCases))]
    public async Task GetCompiledHeaders_CrossOriginResourcePolicyHeaderIsPresentWhenNotSetToNone(CrossOriginResourcePolicy crossOriginEmbedderPolicy, bool headerShouldExist)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAsync())
                       .ReturnsAsync(new SecurityHeaderSettings { CrossOriginResourcePolicy = crossOriginEmbedderPolicy });

        // Act
        var headers = await _service.GetCompiledHeaders();

        // Assert
        Assert.That(headers.Any(x => x.Key.Equals(CspConstants.HeaderNames.CrossOriginResourcePolicy)), Is.EqualTo(headerShouldExist));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetStrictTransportSecurityTestCases))]
    public async Task GetCompiledHeaders_CorrectlySetsTheStrictTransportSecurityHeaderWhenEnabled(bool isEnabled, int maxAge, bool includeSubdomains, string expectedValue)
    {
        // Arrange
        _mockRepository.Setup(x => x.GetAsync())
                       .ReturnsAsync(new SecurityHeaderSettings
                       {
                           IsStrictTransportSecurityEnabled = isEnabled,
                           StrictTransportSecurityMaxAge = maxAge,
                           IsStrictTransportSecuritySubDomainsEnabled = includeSubdomains
                       });

        // Act
        var headers = await _service.GetCompiledHeaders();
        var header = headers.FirstOrDefault(x => x.Key.Equals(CspConstants.HeaderNames.StrictTransportSecurity));

        // Assert
        Assert.That(header?.Value, Is.EqualTo(expectedValue));
    }
}