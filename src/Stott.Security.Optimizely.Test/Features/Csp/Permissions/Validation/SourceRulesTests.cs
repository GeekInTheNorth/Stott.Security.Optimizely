using NUnit.Framework;
using Stott.Security.Optimizely.Features.Csp.Permissions.Validation;

namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.Validation;

[TestFixture]
public sealed class SourceRulesTests
{
    [Test]
    [TestCaseSource(typeof(SourceRulesTestCases), nameof(SourceRulesTestCases.DirectiveValidationTestCases))]
    public void GetRuleForSource_ReturnsCorrectRule(string source, string[] validDirectives)
    {
        // Arrange
        var expectedRule = new SourceRule
        {
            Source = source,
            ValidDirectives = validDirectives
        };

        // Act
        var result = SourceRules.GetRuleForSource(source);

        // Assert
        Assert.That(result.Source, Is.EqualTo(expectedRule.Source));
        Assert.That(result.ValidDirectives, Is.EquivalentTo(expectedRule.ValidDirectives));
    }
}
