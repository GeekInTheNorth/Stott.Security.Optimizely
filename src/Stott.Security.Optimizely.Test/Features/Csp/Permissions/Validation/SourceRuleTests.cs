using System.Collections.Generic;

using NUnit.Framework;
using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Permissions.Validation;

namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.Validation;

[TestFixture]
public sealed class SourceRuleTests
{
    [Test]
    [TestCaseSource(typeof(SourceRuleTestCases), nameof(SourceRuleTestCases.DirectiveValidationTestCases))]
    public void IsValid_CorrectlyValidatesDirectivesAreValid(IList<string> directives, bool expectedOutcome)
    {
        // Arrange
        var sourceRule = new SourceRule
        {
            Source = CspConstants.Sources.UnsafeInline,
            ValidDirectives = [CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement]
        };

        // Act
        var result = sourceRule.IsValid(directives);

        // Assert
        Assert.That(result, Is.EqualTo(expectedOutcome));
    }
}