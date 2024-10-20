namespace Stott.Security.Optimizely.Test.Features.Permissions.Save;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Permissions.Save;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public sealed class AppendPermissionModelTests
{
    [Test]
    [TestCaseSource(typeof(AppendPermissionModelTestCases), nameof(AppendPermissionModelTestCases.DirectiveTestCases))]
    public void ShouldReturnInvalidIfDirectiveIsNotValid(string directive, bool shouldError)
    {
        // Arrange
        var model = new AppendPermissionModel { Source = "https://www.example.com", Directive = directive };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(AppendPermissionModel.Directive))), Is.EqualTo(shouldError));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void ShouldReturnInvalidForAnEmptySource(string source)
    {
        // Arrange
        var model = new AppendPermissionModel { Source = source, Directive = CspConstants.Directives.DefaultSource };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(AppendPermissionModel.Source))), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetSchemeSourceTestCases))]
    public void ShouldValidateSchemeSources(string source, bool shouldError)
    {
        // Arrange
        var model = new AppendPermissionModel { Source = source, Directive = CspConstants.Directives.DefaultSource };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(SavePermissionModel.Source))), Is.EqualTo(shouldError));
    }

    [Test]
    [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetQuotedSourceTestCases))]
    public void ShouldValidateQuotedSources(string source, bool shouldError)
    {
        // Arrange
        var model = new AppendPermissionModel { Source = source, Directive = CspConstants.Directives.ScriptSource };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(AppendPermissionModel.Source))), Is.EqualTo(shouldError));
    }

    [Test]
    [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetValidUrlTestCases))]
    public void ShouldValidateUrlsWithOrWithoutWildcards(string source)
    {
        // Arrange
        var model = new AppendPermissionModel { Source = source, Directive = CspConstants.Directives.DefaultSource };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(AppendPermissionModel.Source))), Is.False);
    }

    [Test]
    [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetInValidUrlTestCases))]
    public void ShouldReturnValidationErrorsForInvalidUrls(string source)
    {
        // Arrange
        var model = new AppendPermissionModel { Source = source, Directive = CspConstants.Directives.DefaultSource };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(AppendPermissionModel.Source))), Is.True);
    }

    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }
}