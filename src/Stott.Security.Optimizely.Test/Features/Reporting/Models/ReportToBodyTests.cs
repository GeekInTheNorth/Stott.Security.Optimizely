namespace Stott.Security.Optimizely.Test.Features.Reporting.Models;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Reporting.Models;

[TestFixture]
public sealed class ReportToBodyTests
{
    [Test]
    [TestCaseSource(typeof(ReportModelTestCases), nameof(ReportModelTestCases.PartialMatchingDirectiveTestCases))]
    public void SelectsTheCorrectEffectiveDirectiveAsDataCleansingWhenTheDirectiveContainsExtraText(string assignedEffectiveDirective, string resolvedEffectiveDirective)
    {
        // Arrange
        var model = new ReportToBody
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
        var model = new ReportToBody
        {
            EffectiveDirective = effectiveDirective,
        };

        // Assert
        Assert.That(model.EffectiveDirective, Is.EqualTo(effectiveDirective));
    }
}