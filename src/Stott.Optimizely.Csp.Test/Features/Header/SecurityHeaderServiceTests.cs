using System.Collections.Generic;
using System.Linq;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Header;
using Stott.Optimizely.Csp.Features.Permissions.Repository;

namespace Stott.Optimizely.Csp.Test.Features.Header
{
    [TestFixture]
    public class SecurityHeaderServiceTests
    {
        private Mock<ICspPermissionRepository> _repository;

        private Mock<ICspContentBuilder> _headerBuilder;

        private SecurityHeaderService _service;

        [SetUp]
        public void SetUp()
        {
            _repository = new Mock<ICspPermissionRepository>();

            _headerBuilder = new Mock<ICspContentBuilder>();

            _service = new SecurityHeaderService(_repository.Object, _headerBuilder.Object);
        }

        [Test]
        [TestCaseSource(typeof(SecurityHeaderServiceTestCases), nameof(SecurityHeaderServiceTestCases.GetEmptySourceTestCases))]
        public void GetCspContent_PassesEmptyCollectionIntoHeaderBuilderWhenRepositoryReturnsNullOrEmptySources(
            IList<CspSource> configuredSources,
            IList<CspSource> requiredSources)
        {
            // Arrange
            _repository.Setup(x => x.Get()).Returns(configuredSources);
            _repository.Setup(x => x.GetCmsRequirements()).Returns(requiredSources);

            List<CspSource> sourcesUsed = null;
            _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<CspSource>>()))
                          .Returns(_headerBuilder.Object)
                          .Callback<IEnumerable<CspSource>>(x => sourcesUsed = x.ToList());

            // Act
            _service.GetCspContent();

            // Assert
            Assert.That(sourcesUsed, Is.Not.Null);
            Assert.That(sourcesUsed, Is.Empty);
        }

        [Test]
        public void GetCspContent_MergesConfiguredAndRequiredSourcesToPassIntoTheHeaderBuilder()
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

            _repository.Setup(x => x.Get()).Returns(configuredSources);
            _repository.Setup(x => x.GetCmsRequirements()).Returns(requiredSources);

            List<CspSource> sourcesUsed = null;
            _headerBuilder.Setup(x => x.WithSources(It.IsAny<IEnumerable<CspSource>>()))
                          .Returns(_headerBuilder.Object)
                          .Callback<IEnumerable<CspSource>>(x => sourcesUsed = x.ToList());

            // Act
            _service.GetCspContent();

            // Assert
            Assert.That(sourcesUsed, Is.Not.Null);
            Assert.That(sourcesUsed.Count, Is.EqualTo(4));
            Assert.That(sourcesUsed.IndexOf(configuredSources[0]), Is.GreaterThanOrEqualTo(0));
            Assert.That(sourcesUsed.IndexOf(configuredSources[1]), Is.GreaterThanOrEqualTo(0));
            Assert.That(sourcesUsed.IndexOf(requiredSources[0]), Is.GreaterThanOrEqualTo(0));
            Assert.That(sourcesUsed.IndexOf(requiredSources[1]), Is.GreaterThanOrEqualTo(0));
        }
    }
}
