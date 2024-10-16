using System;
using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Permissions.Validation;

namespace Stott.Security.Optimizely.Test.Features.Permissions.Validation;

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
            ValidDirectives = new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement }
        };

        // Act
        var result = sourceRule.IsValid(directives);

        // Assert
        Assert.That(result, Is.EqualTo(expectedOutcome));
    }
}

public static class SourceRuleTestCases
{
    public static IEnumerable<TestCaseData> DirectiveValidationTestCases
    {
        get
        {
            yield return new TestCaseData(null, false);
            yield return new TestCaseData(Array.Empty<string>(), false);
            yield return new TestCaseData(new[] { CspConstants.Directives.ScriptSource }, true);
            yield return new TestCaseData(new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement }, true);
            yield return new TestCaseData(new[] { CspConstants.Directives.ScriptSourceElement }, true);
            yield return new TestCaseData(new[] { CspConstants.Directives.ScriptSource, CspConstants.Directives.ScriptSourceElement, CspConstants.Directives.StyleSourceElement }, false);
            yield return new TestCaseData(new[] { CspConstants.Directives.StyleSourceElement }, false);
        }
    }
}