namespace Stott.Security.Optimizely.Test.Features.Csp.Reporting.Models;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Csp.Reporting.Models;

[TestFixture]
public sealed class ReportUriBodyTests
{
    [Test]
    [TestCaseSource(typeof(ReportModelTestCases), nameof(ReportModelTestCases.PartialMatchingDirectiveTestCases))]
    public void SelectsTheCorrectViolatedDirectiveAsDataCleansingWhenTheDirectiveContainsExtraText(string assignedViolatedDirective, string resolvedViolatedDirective)
    {
        // Arrange
        var model = new ReportUriBody
        {
            ViolatedDirective = assignedViolatedDirective,
        };

        // Assert
        Assert.That(model.ViolatedDirective, Is.EqualTo(resolvedViolatedDirective));
    }

    [Test]
    [TestCaseSource(typeof(ReportModelTestCases), nameof(ReportModelTestCases.MatchingDirectiveTestCases))]
    public void SelectsTheCorrectViolatedDirectiveAsDataCleansingWhenTheDirectiveIsValid(string violatedDirective)
    {
        // Arrange
        var model = new ReportUriBody
        {
            ViolatedDirective = violatedDirective,
        };

        // Assert
        Assert.That(model.ViolatedDirective, Is.EqualTo(violatedDirective));
    }

    [Test]
    [TestCaseSource(typeof(ReportModelTestCases), nameof(ReportModelTestCases.PartialMatchingDirectiveTestCases))]
    public void SelectsTheCorrectEffectiveDirectiveAsDataCleansingWhenTheDirectiveContainsExtraText(string assignedEffectiveDirective, string resolvedEffectiveDirective)
    {
        // Arrange
        var model = new ReportUriBody
        {
            EffectiveDirective = assignedEffectiveDirective,
        };

        // Assert
        Assert.That(model.EffectiveDirective, Is.EqualTo(resolvedEffectiveDirective));
    }

    [Test]
    [TestCaseSource(typeof(ReportModelTestCases), nameof(ReportModelTestCases.MatchingDirectiveTestCases))]
    public void SelectsTheCorrectEffectiveDirectiveAsDataCleansingWhenTheDirectiveIsValid(string effectiveDirective)
    {
        // Arrange
        var model = new ReportUriBody
        {
            EffectiveDirective = effectiveDirective,
        };

        // Assert
        Assert.That(model.EffectiveDirective, Is.EqualTo(effectiveDirective));
    }
}