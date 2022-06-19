using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Caching;
using Stott.Security.Core.Features.Header;
using Stott.Security.Core.Features.Permissions.Repository;
using Stott.Security.Core.Features.SecurityHeaders.Enums;
using Stott.Security.Core.Features.SecurityHeaders.Repository;
using Stott.Security.Core.Features.Settings.Repository;

namespace Stott.Security.Core.Test.Features.Header
{
    [TestFixture]
    public class HeaderCompilationServiceTests
    {
        private Mock<ICspPermissionRepository> _cspPermissionRepository;

        private Mock<ICspSettingsRepository> _cspSettingsRepository;

        private Mock<ISecurityHeaderRepository> _securityHeaderRepository;

        private Mock<ICspContentBuilder> _headerBuilder;

        private ICacheWrapper _cacheWrapper;

        private HeaderCompilationService _service;

        [SetUp]
        public void SetUp()
        {
            _cspPermissionRepository = new Mock<ICspPermissionRepository>();

            _cspSettingsRepository = new Mock<ICspSettingsRepository>();
            _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings());

            _securityHeaderRepository = new Mock<ISecurityHeaderRepository>();
            _securityHeaderRepository.Setup(x => x.GetAsync()).ReturnsAsync(new SecurityHeaderSettings());

            _headerBuilder = new Mock<ICspContentBuilder>();

            _cacheWrapper = new InactiveCacheWrapper();

            _service = new HeaderCompilationService(
                _cspPermissionRepository.Object,
                _cspSettingsRepository.Object,
                _securityHeaderRepository.Object,
                _headerBuilder.Object,
                _cacheWrapper);
        }

        [Test]
        [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetEmptySourceTestCases))]
        public async Task GetSecurityHeaders_PassesEmptyCollectionIntoHeaderBuilderWhenRepositoryReturnsNullOrEmptySources(
            IList<CspSource> configuredSources,
            IList<CspSource> requiredSources)
        {
            // Arrange
            _cspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(configuredSources);
            _cspPermissionRepository.Setup(x => x.GetCmsRequirements()).Returns(requiredSources);
            _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsEnabled = true });

            List<CspSource> sourcesUsed = null;
            _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<CspSource>>()))
                          .Returns(_headerBuilder.Object)
                          .Callback<IEnumerable<CspSource>>(x => sourcesUsed = x.ToList());

            // Act
            await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(sourcesUsed, Is.Not.Null);
            Assert.That(sourcesUsed, Is.Empty);
        }

        [Test]
        public async Task GetSecurityHeaders_MergesConfiguredAndRequiredSourcesToPassIntoTheHeaderBuilder()
        {
            // Arrange
            var configuredSources = new List<CspSource>
            {
                new CspSource { Source = "https://www.google.com", Directives = $"{CspConstants.Directives.ScriptSource},{CspConstants.Directives.StyleSource}"},
                new CspSource { Source = "https://www.example.com", Directives = $"{CspConstants.Directives.ScriptSource}"}
            };

            var requiredSources = new List<CspSource>
            {
                new CspSource { Source = CspConstants.Sources.UnsafeInline, Directives = $"{CspConstants.Directives.ScriptSource},{CspConstants.Directives.StyleSource}"},
                new CspSource { Source = CspConstants.Sources.UnsafeEval, Directives = $"{CspConstants.Directives.ScriptSource}"}
            };

            _cspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(configuredSources);
            _cspPermissionRepository.Setup(x => x.GetCmsRequirements()).Returns(requiredSources);
            _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsEnabled = true });

            List<CspSource> sourcesUsed = null;
            _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<CspSource>>()))
                          .Returns(_headerBuilder.Object)
                          .Callback<IEnumerable<CspSource>>(x => sourcesUsed = x.ToList());

            // Act
            await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(sourcesUsed, Is.Not.Null);
            Assert.That(sourcesUsed.Count, Is.EqualTo(4));
            Assert.That(sourcesUsed.IndexOf(configuredSources[0]), Is.GreaterThanOrEqualTo(0));
            Assert.That(sourcesUsed.IndexOf(configuredSources[1]), Is.GreaterThanOrEqualTo(0));
            Assert.That(sourcesUsed.IndexOf(requiredSources[0]), Is.GreaterThanOrEqualTo(0));
            Assert.That(sourcesUsed.IndexOf(requiredSources[1]), Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task GetSecurityHeaders_ContentSecurityHeaderIsAbsentWhenDisabled()
        {
            // Arrange
            _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsEnabled = false });

            // Act
            var headers = await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ContentSecurityPolicy), Is.False);
        }

        [Test]
        [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetCspReportOnlyTestCases))]
        public async Task GetSecurityHeaders_ContentSecurityHeaderIsCorrentlySetToReportOnly(bool isReportOnlyMode, string expectedHeader)
        {
            // Arrange
            var configuredSources = new List<CspSource>
            {
                new CspSource { Source = "https://www.google.com", Directives = $"{CspConstants.Directives.ScriptSource},{CspConstants.Directives.StyleSource}"},
                new CspSource { Source = "https://www.example.com", Directives = $"{CspConstants.Directives.ScriptSource}"}
            };

            _cspPermissionRepository.Setup(x => x.GetAsync()).ReturnsAsync(configuredSources);
            _cspPermissionRepository.Setup(x => x.GetCmsRequirements()).Returns(new List<CspSource>());
            _cspSettingsRepository.Setup(x => x.GetAsync()).ReturnsAsync(new CspSettings { IsEnabled = true, IsReportOnly = isReportOnlyMode });

            _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<CspSource>>()))
                          .Returns(_headerBuilder.Object);

            // Act
            var headers = await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(headers.ContainsKey(expectedHeader), Is.True);
        }

        [Test]
        public async Task GetSecurityHeaders_XContentTypeOptionsHeaderIsAbsentWhenDisabled()
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.GetAsync())
                                     .ReturnsAsync(new SecurityHeaderSettings { IsXContentTypeOptionsEnabled = false });

            // Act
            var headers = await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ContentTypeOptions), Is.False);
        }

        [Test]
        public async Task GetSecurityHeaders_XContentTypeOptionsHeaderIsPresentWhenEnabled()
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.GetAsync())
                                     .ReturnsAsync(new SecurityHeaderSettings { IsXContentTypeOptionsEnabled = true });

            // Act
            var headers = await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ContentTypeOptions), Is.True);
        }

        [Test]
        public async Task GetSecurityHeaders_XssProtectionHeaderIsAbsentWhenDisabled()
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.GetAsync())
                                     .ReturnsAsync(new SecurityHeaderSettings { IsXXssProtectionEnabled = false });

            // Act
            var headers = await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.XssProtection), Is.False);
        }

        [Test]
        public async Task GetSecurityHeaders_XssProtectionHeaderIsPresentWhenEnabled()
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.GetAsync())
                                     .ReturnsAsync(new SecurityHeaderSettings { IsXXssProtectionEnabled = true });

            // Act
            var headers = await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.XssProtection), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetReferrerPolicyTestCases))]
        public async Task GetSecurityHeaders_ReferrerPolicyHeaderIsPresentWhenNotSetToNone(ReferrerPolicy referrerPolicy, bool headerShouldExist)
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.GetAsync())
                                     .ReturnsAsync(new SecurityHeaderSettings { ReferrerPolicy = referrerPolicy });

            // Act
            var headers = await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ReferrerPolicy), Is.EqualTo(headerShouldExist));
        }

        [Test]
        [TestCaseSource(typeof(HeaderCompilationServiceTestCases), nameof(HeaderCompilationServiceTestCases.GetFrameOptionsTestCases))]
        public async Task GetSecurityHeaders_FrameOptionsHeaderIsPresentWhenNotSetToNone(XFrameOptions frameOptions, bool headerShouldExist)
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.GetAsync())
                                     .ReturnsAsync(new SecurityHeaderSettings { FrameOptions = frameOptions });

            // Act
            var headers = await _service.GetSecurityHeadersAsync();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.FrameOptions), Is.EqualTo(headerShouldExist));
        }
    }
}
