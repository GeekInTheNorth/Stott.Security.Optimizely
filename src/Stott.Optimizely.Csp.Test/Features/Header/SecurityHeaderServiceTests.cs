using System.Collections.Generic;
using System.Linq;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Header;
using Stott.Optimizely.Csp.Features.Permissions.Repository;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Enums;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Repository;
using Stott.Optimizely.Csp.Features.Settings.Repository;

namespace Stott.Optimizely.Csp.Test.Features.Header
{
    [TestFixture]
    public class SecurityHeaderServiceTests
    {
        private Mock<ICspPermissionRepository> _cspPermissionRepository;

        private Mock<ICspSettingsRepository> _cspSettingsRepository;

        private Mock<ISecurityHeaderRepository> _securityHeaderRepository;

        private Mock<ICspContentBuilder> _headerBuilder;

        private SecurityHeaderService _service;

        [SetUp]
        public void SetUp()
        {
            _cspPermissionRepository = new Mock<ICspPermissionRepository>();

            _cspSettingsRepository = new Mock<ICspSettingsRepository>();
            _cspSettingsRepository.Setup(x => x.Get()).Returns(new CspSettings());

            _securityHeaderRepository = new Mock<ISecurityHeaderRepository>();
            _securityHeaderRepository.Setup(x => x.Get()).Returns(new SecurityHeaderSettings());

            _headerBuilder = new Mock<ICspContentBuilder>();

            _service = new SecurityHeaderService(
                _cspPermissionRepository.Object,
                _cspSettingsRepository.Object,
                _securityHeaderRepository.Object,
                _headerBuilder.Object);
        }

        [Test]
        [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetEmptySourceTestCases))]
        public void GetSecurityHeaders_PassesEmptyCollectionIntoHeaderBuilderWhenRepositoryReturnsNullOrEmptySources(
            IList<CspSource> configuredSources,
            IList<CspSource> requiredSources)
        {
            // Arrange
            _cspPermissionRepository.Setup(x => x.Get()).Returns(configuredSources);
            _cspPermissionRepository.Setup(x => x.GetCmsRequirements()).Returns(requiredSources);
            _cspSettingsRepository.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = true });

            List<CspSource> sourcesUsed = null;
            _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<CspSource>>()))
                          .Returns(_headerBuilder.Object)
                          .Callback<IEnumerable<CspSource>>(x => sourcesUsed = x.ToList());

            // Act
            _service.GetSecurityHeaders();

            // Assert
            Assert.That(sourcesUsed, Is.Not.Null);
            Assert.That(sourcesUsed, Is.Empty);
        }

        [Test]
        public void GetSecurityHeaders_MergesConfiguredAndRequiredSourcesToPassIntoTheHeaderBuilder()
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

            _cspPermissionRepository.Setup(x => x.Get()).Returns(configuredSources);
            _cspPermissionRepository.Setup(x => x.GetCmsRequirements()).Returns(requiredSources);
            _cspSettingsRepository.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = true });

            List<CspSource> sourcesUsed = null;
            _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<CspSource>>()))
                          .Returns(_headerBuilder.Object)
                          .Callback<IEnumerable<CspSource>>(x => sourcesUsed = x.ToList());

            // Act
            _service.GetSecurityHeaders();

            // Assert
            Assert.That(sourcesUsed, Is.Not.Null);
            Assert.That(sourcesUsed.Count, Is.EqualTo(4));
            Assert.That(sourcesUsed.IndexOf(configuredSources[0]), Is.GreaterThanOrEqualTo(0));
            Assert.That(sourcesUsed.IndexOf(configuredSources[1]), Is.GreaterThanOrEqualTo(0));
            Assert.That(sourcesUsed.IndexOf(requiredSources[0]), Is.GreaterThanOrEqualTo(0));
            Assert.That(sourcesUsed.IndexOf(requiredSources[1]), Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetSecurityHeaders_ContentSecurityHeaderIsAbsentWhenDisabled()
        {
            // Arrange
            _cspSettingsRepository.Setup(x => x.Get()).Returns(new CspSettings { IsEnabled = false });

            // Act
            var headers = _service.GetSecurityHeaders();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ContentSecurityPolicy), Is.False);
        }

        [Test]
        public void GetSecurityHeaders_XContentTypeOptionsHeaderIsAbsentWhenDisabled()
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.Get())
                                     .Returns(new SecurityHeaderSettings { IsXContentTypeOptionsEnabled = false });

            // Act
            var headers = _service.GetSecurityHeaders();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ContentTypeOptions), Is.False);
        }

        [Test]
        public void GetSecurityHeaders_XContentTypeOptionsHeaderIsPresentWhenEnabled()
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.Get())
                                     .Returns(new SecurityHeaderSettings { IsXContentTypeOptionsEnabled = true });

            // Act
            var headers = _service.GetSecurityHeaders();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ContentTypeOptions), Is.True);
        }

        [Test]
        public void GetSecurityHeaders_XssProtectionHeaderIsAbsentWhenDisabled()
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.Get())
                                     .Returns(new SecurityHeaderSettings { IsXXssProtectionEnabled = false });

            // Act
            var headers = _service.GetSecurityHeaders();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.XssProtection), Is.False);
        }

        [Test]
        public void GetSecurityHeaders_XssProtectionHeaderIsPresentWhenEnabled()
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.Get())
                                     .Returns(new SecurityHeaderSettings { IsXXssProtectionEnabled = true });

            // Act
            var headers = _service.GetSecurityHeaders();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.XssProtection), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetReferrerPolicyTestCases))]
        public void GetSecurityHeaders_ReferrerPolicyHeaderIsPresentWhenNotSetToNone(ReferrerPolicy referrerPolicy, bool headerShouldExist)
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.Get())
                                     .Returns(new SecurityHeaderSettings { ReferrerPolicy = referrerPolicy });

            // Act
            var headers = _service.GetSecurityHeaders();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.ReferrerPolicy), Is.EqualTo(headerShouldExist));
        }

        [Test]
        [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetFrameOptionsTestCases))]
        public void GetSecurityHeaders_FrameOptionsHeaderIsPresentWhenNotSetToNone(XFrameOptions frameOptions, bool headerShouldExist)
        {
            // Arrange
            _securityHeaderRepository.Setup(x => x.Get())
                                     .Returns(new SecurityHeaderSettings { FrameOptions = frameOptions });

            // Act
            var headers = _service.GetSecurityHeaders();

            // Assert
            Assert.That(headers.ContainsKey(CspConstants.HeaderNames.FrameOptions), Is.EqualTo(headerShouldExist));
        }
    }
}
