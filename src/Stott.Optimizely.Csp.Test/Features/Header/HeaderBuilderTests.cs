using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.Header;
using Stott.Optimizely.Csp.Test.TestCases;

namespace Stott.Optimizely.Csp.Test.Features.Header
{
    [TestFixture]
    public class HeaderBuilderTests
    {
        private HeaderBuilder _headerBuilder;

        [SetUp]
        public void SetUp()
        {
            _headerBuilder = new HeaderBuilder();
        }

        [Test]
        public void Build_GivenNoCspSources_ThenAnEmptyStringIsReturned()
        {
            // Act
            var policy = _headerBuilder.WithSources(null).Build();

            // Assert
            Assert.That(policy, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Build_GivenAnEmptyCspSources_ThenAnEmptyStringIsReturned()
        {
            // Act
            var policy = _headerBuilder.WithSources(new List<CspSource>(0)).Build();

            // Assert
            Assert.That(policy, Is.EqualTo(string.Empty));
        }

        [Test]
        [TestCaseSource(typeof(HeaderBuilderTestCases), nameof(HeaderBuilderTestCases.NonMatchingSourceTestCases))]
        public void Build_GivenMultipleRecords_ThenThePolicyShouldContainEntriesForAllRecords(
            List<CspSource> sources,
            string expectedPolicy)
        {
            // Act
            var policy = _headerBuilder.WithSources(sources).Build();

            // Assert
            Assert.That(policy, Is.EqualTo(expectedPolicy));
        }

        [Test]
        [TestCaseSource(typeof(HeaderBuilderTestCases), nameof(HeaderBuilderTestCases.MultipleMatchingSourceTestCases))]
        public void Build_GivenMultipleRecordsWithMatchingSources_ThenDirectivesShouldContainUniqueSources(
            List<CspSource> sources,
            string expectedPolicy)
        {
            // Act
            var policy = _headerBuilder.WithSources(sources).Build();

            // Assert
            Assert.That(policy, Is.EqualTo(expectedPolicy));
        }

        [Test]
        public void Build_GivenAVarietyOfAllSourceTypes_ThenSourcesShouldBeCorrectlyOrdered()
        {
            // Arrange
            var sources = CspConstants.AllSources
                                      .Select(x => new CspSource { Source = x, Directives = CspConstants.Directives.DefaultSource })
                                      .OrderBy(x => Guid.NewGuid())
                                      .ToList();
            sources.Add(new CspSource { Source = "https://www.example.com", Directives = CspConstants.Directives.DefaultSource });

            // Act
            var policy = _headerBuilder.WithSources(sources).Build();
            var expectedPolicy = "default-src 'self' 'unsafe-eval' 'unsafe-inline' 'unsafe-hashes' 'none' blob: data: filesystem: http: https: mediastream: https://www.example.com;";

            // Assert
            Assert.That(policy, Is.EqualTo(expectedPolicy));
        }

        [Test]
        public void Build_GivenReportingIsNotConfigured_ThenReportToShouldBeAbsent()
        {
            // Arrange
            var sources = new List<CspSource>
            {
                new CspSource { Source = "https://www.example.com", Directives = CspConstants.Directives.DefaultSource } 
            };

            // Act
            var policy = _headerBuilder.WithSources(sources)
                                       .WithReporting(false, "/csp-violation-url/")
                                       .Build();
            var expectedPolicy = "default-src https://www.example.com;";

            // Assert
            Assert.That(policy, Is.EqualTo(expectedPolicy));
        }

        [Test]
        [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
        public void Build_GivenNullOrEmptyReportingUrl_ThenReportToShouldBeAbsent(string reportUrl)
        {
            // Arrange
            var sources = new List<CspSource>
            {
                new CspSource { Source = "https://www.example.com", Directives = CspConstants.Directives.DefaultSource }
            };

            // Act
            var policy = _headerBuilder.WithSources(sources)
                                       .WithReporting(true, reportUrl)
                                       .Build();
            var expectedPolicy = "default-src https://www.example.com;";

            // Assert
            Assert.That(policy, Is.EqualTo(expectedPolicy));
        }

        [Test]
        public void Build_GivenReportingIsConfiguredWithAReportingUrl_ThenReportToShouldBePresent()
        {
            // Arrange
            var sources = new List<CspSource>
            {
                new CspSource { Source = "https://www.example.com", Directives = CspConstants.Directives.DefaultSource }
            };

            // Act
            var policy = _headerBuilder.WithSources(sources)
                                       .WithReporting(true, "/csp-violation-url/")
                                       .Build();
            var expectedPolicy = "default-src https://www.example.com; report-to /csp-violation-url/;";

            // Assert
            Assert.That(policy, Is.EqualTo(expectedPolicy));
        }
    }
}
