namespace Stott.Security.Optimizely.Test.Features.Reporting;

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
        var summary = new ViolationReportSummary { Source = source };

        // Assert
        Assert.That(summary.SanitizedSource, Is.EqualTo(expectedSanitizedSource));
    }

    [Test]
    [TestCaseSource(typeof(ViolationReportSummaryTestCases), nameof(ViolationReportSummaryTestCases.DomainSuggestionTestCases))]
    public void CreatesAppropriateSuggestionsForWildCardDomains(string source, IList<string> expectedSuggestions)
    {
        // Arrange
        var summary = new ViolationReportSummary { Source = source };

        // Act
        var suggestions = summary.GetDomainSuggestions();

        // Assert
        Assert.That(suggestions, Is.EquivalentTo(expectedSuggestions));
    }
}