namespace Stott.Security.Optimizely.Test.Features.Csp.Reporting;

using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Csp.Reporting;

[TestFixture]
public sealed class ViolationReportSummaryTests
{
    [Test]
    [TestCaseSource(typeof(ViolationReportSummaryTestCases), nameof(ViolationReportSummaryTestCases.SanitizedSourceTestCases))]
    public void SanitizedSourceReturnsEitherTheFullDomainOrOriginalString(
        string source,
        string expectedSanitizedSource)
    {
        // Arrange
        var summary = new ViolationReportSummary(Guid.NewGuid(), source, string.Empty, 1, DateTime.Now);

        // Assert
        Assert.That(summary.SanitizedSource, Is.EqualTo(expectedSanitizedSource));
    }

    [Test]
    [TestCaseSource(typeof(ViolationReportSummaryTestCases), nameof(ViolationReportSummaryTestCases.SourceSuggestionTestCases))]
    public void CreatesAppropriateSuggestionsForWildCardDomains(string source, IList<string> expectedSuggestions)
    {
        // Arrange
        var summary = new ViolationReportSummary(Guid.NewGuid(), source, string.Empty, 1, DateTime.Now);

        // Assert
        Assert.That(summary.SourceSuggestions, Is.EquivalentTo(expectedSuggestions));
    }

    [Test]
    [TestCaseSource(typeof(ViolationReportSummaryTestCases), nameof(ViolationReportSummaryTestCases.NonUrlSourceSuggestionTestCases))]
    public void CreatesASingleSuggestionMatchingTheSourceWhenSourceIsNotAUrl(string source)
    {
        // Arrange
        var summary = new ViolationReportSummary(Guid.NewGuid(), source, string.Empty, 1, DateTime.Now);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(summary.SourceSuggestions, Has.Count.EqualTo(1));
            Assert.That(summary.SourceSuggestions.First(), Is.EqualTo(source));
        });
    }

    [Test]
    [TestCaseSource(typeof(ViolationReportSummaryTestCases), nameof(ViolationReportSummaryTestCases.DirectiveSuggestionTestCases))]
    public void CreatesAppropriateSuggestionsForDirectives(string directive, IList<string> expectedDirectives)
    {
        // Arrange
        var summary = new ViolationReportSummary(Guid.NewGuid(), string.Empty, directive, 1, DateTime.Now);

        // Assert
        Assert.That(summary.DirectiveSuggestions, Is.EquivalentTo(expectedDirectives));
    }
}