namespace Stott.Security.Optimizely.Test.Features.Reporting;

using System;
using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Reporting;

[TestFixture]
public class ViolationReportSummaryTests
{
    [Test]
    [TestCaseSource(typeof(ViolationReportSummaryTestCases), nameof(ViolationReportSummaryTestCases.SanitizedSourceTestCases))]
    public void SanitizedSourceReturnsEitherTheFullDomainOrOriginalString(
        string source, 
        string expectedSanitizedSource)
    {
        // Arrange
        var summary = new ViolationReportSummary(1, source, string.Empty, 1, DateTime.Now);

        // Assert
        Assert.That(summary.SanitizedSource, Is.EqualTo(expectedSanitizedSource));
    }

    [Test]
    [TestCaseSource(typeof(ViolationReportSummaryTestCases), nameof(ViolationReportSummaryTestCases.SourceSuggestionTestCases))]
    public void CreatesAppropriateSuggestionsForWildCardDomains(string source, IList<string> expectedSuggestions)
    {
        // Arrange
        var summary = new ViolationReportSummary(1, source, string.Empty, 1, DateTime.Now);

        // Assert
        Assert.That(summary.SourceSuggestions, Is.EquivalentTo(expectedSuggestions));
    }

    [Test]
    [TestCaseSource(typeof(ViolationReportSummaryTestCases), nameof(ViolationReportSummaryTestCases.DirectiveSuggestionTestCases))]
    public void CreatesAppropriateSuggestionsForDirectives(string directive, IList<string> expectedDirectives)
    {
        // Arrange
        var summary = new ViolationReportSummary(1, string.Empty, directive, 1, DateTime.Now);

        // Assert
        Assert.That(summary.DirectiveSuggestions, Is.EquivalentTo(expectedDirectives));
    }
}