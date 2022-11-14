﻿namespace Stott.Security.Optimizely.Test.Features.Header;

using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Header;
using Stott.Security.Optimizely.Features.Sandbox;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public class CspContentBuilderTests
{
    private CspContentBuilder _headerBuilder;

    [SetUp]
    public void SetUp()
    {
        _headerBuilder = new CspContentBuilder();
    }

    [Test]
    public void Build_GivenNoCspSources_ThenAnEmptyStringIsReturned()
    {
        // Act
        var policy = _headerBuilder.WithSources(null).BuildAsync();

        // Assert
        Assert.That(policy, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Build_GivenAnEmptyCspSources_ThenAnEmptyStringIsReturned()
    {
        // Act
        var policy = _headerBuilder.WithSources(new List<CspSource>(0)).BuildAsync();

        // Assert
        Assert.That(policy, Is.EqualTo(string.Empty));
    }

    [Test]
    [TestCaseSource(typeof(CspContentBuilderTestCases), nameof(CspContentBuilderTestCases.NonMatchingSourceTestCases))]
    public void Build_GivenMultipleRecords_ThenThePolicyShouldContainEntriesForAllRecords(
        List<CspSource> sources,
        string expectedPolicy)
    {
        // Act
        var policy = _headerBuilder.WithSources(sources).BuildAsync();

        // Assert
        Assert.That(policy, Is.EqualTo(expectedPolicy));
    }

    [Test]
    [TestCaseSource(typeof(CspContentBuilderTestCases), nameof(CspContentBuilderTestCases.MultipleMatchingSourceTestCases))]
    public void Build_GivenMultipleRecordsWithMatchingSources_ThenDirectivesShouldContainUniqueSources(
        List<CspSource> sources,
        string expectedPolicy)
    {
        // Act
        var policy = _headerBuilder.WithSources(sources).BuildAsync();

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
        var policy = _headerBuilder.WithSources(sources).BuildAsync();
        var expectedPolicy = "default-src 'self' 'unsafe-eval' 'wasm-unsafe-eval' 'unsafe-inline' 'unsafe-hashes' 'none' blob: data: filesystem: http: https: mediastream: https://www.example.com;";

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
                                   .BuildAsync();
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
                                   .BuildAsync();
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
                                   .BuildAsync();
        var expectedPolicy = "default-src https://www.example.com; report-to /csp-violation-url/;";

        // Assert
        Assert.That(policy, Is.EqualTo(expectedPolicy));
    }

    [Test]
    public void Build_GivenCspSettingsAreAbsentThenTheSandboxIsNotIncluded()
    {
        // Arrange
        var sandbox = new SandboxModel { IsSandboxEnabled = true };

        // Act
        var policy = _headerBuilder.WithSources(Enumerable.Empty<CspSource>())
                                   .WithSandbox(sandbox)
                                   .BuildAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(policy, Is.Not.Null);
            Assert.That(policy.Contains("sandbox"), Is.False);
        });
    }

    [Test]
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public void Build_GivenCspIsNotEnabledOrIsReportOnlyThenSandboxShouldBeAbsent(bool isEnabled, bool isReportOnly)
    {
        // Arrange
        var settings = new CspSettings { IsEnabled = isEnabled, IsReportOnly = isReportOnly };
        var sandbox = new SandboxModel { IsSandboxEnabled = true };

        // Act
        var policy = _headerBuilder.WithSources(Enumerable.Empty<CspSource>())
                                   .WithSandbox(sandbox)
                                   .WithSettings(settings)
                                   .BuildAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(policy, Is.Not.Null);
            Assert.That(policy.Contains("sandbox"), Is.False);
        });
    }

    [Test]
    public void Build_GivenSandboxIsDisabledButCspIsActiveThenSandboxShouldBeAbsent()
    {
        // Arrange
        var settings = new CspSettings { IsEnabled = true, IsReportOnly = false };
        var sandbox = new SandboxModel { IsSandboxEnabled = false };

        // Act
        var policy = _headerBuilder.WithSources(Enumerable.Empty<CspSource>())
                                   .WithSandbox(sandbox)
                                   .WithSettings(settings)
                                   .BuildAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(policy, Is.Not.Null);
            Assert.That(policy.Contains("sandbox"), Is.False);
        });
    }

    [Test]
    [TestCaseSource(typeof(CspContentBuilderTestCases), nameof(CspContentBuilderTestCases.GetSandboxContentTestCases))]
    public void Build_GivenSandboxIsEnabledThenValuesShouldBeAddedAccordingly(
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
        var settings = new CspSettings { IsEnabled = true, IsReportOnly = false };
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

        // Act
        var policy = _headerBuilder.WithSources(Enumerable.Empty<CspSource>())
                                   .WithSandbox(sandbox)
                                   .WithSettings(settings)
                                   .BuildAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(policy, Is.Not.Null);
            Assert.That(policy, Is.EqualTo(expectedSandbox));
        });
    }
}        