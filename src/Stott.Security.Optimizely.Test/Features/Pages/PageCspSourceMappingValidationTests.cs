namespace Stott.Security.Optimizely.Test.Features.Pages;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Permissions.Save;
using Stott.Security.Optimizely.Features.Pages;
using Stott.Security.Optimizely.Test.Features.Csp.Permissions.Save;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public sealed class PageCspSourceMappingValidationTests
{
    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void ShouldReturnInvalidForAnEmptySource(string source)
    {
        // Arrange
        var model = new PageCspSourceMapping { Source = source, Directives = CspConstants.Directives.DefaultSource };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(SavePermissionModel.Source))), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetSchemeSourceTestCases))]
    public void ShouldValidateSchemeSources(string source, bool shouldError)
    {
        // Arrange
        var model = new PageCspSourceMapping { Source = source, Directives = CspConstants.Directives.DefaultSource };

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
        var model = new PageCspSourceMapping { Source = source, Directives = CspConstants.Directives.DefaultSource };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(SavePermissionModel.Source))), Is.EqualTo(shouldError));
    }

    [Test]
    [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetValidUrlTestCases))]
    public void ShouldValidateUrlsWithOrWithoutWildcards(string source)
    {
        // Arrange
        var model = new PageCspSourceMapping { Source = source, Directives = CspConstants.Directives.DefaultSource };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(SavePermissionModel.Source))), Is.False);
    }

    [Test]
    [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetInValidUrlTestCases))]
    public void ShouldReturnValidationErrorsForInvalidUrls(string source)
    {
        // Arrange
        var model = new PageCspSourceMapping { Source = source, Directives = CspConstants.Directives.DefaultSource };

        // Act
        var validationResult = ValidateModel(model);

        // Assert
        Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(SavePermissionModel.Source))), Is.True);
    }

    private static IList<ValidationResult> ValidateModel(PageCspSourceMapping cspSourceMapping)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(cspSourceMapping);
        Validator.TryValidateObject(cspSourceMapping, context, validationResults, true);

        return validationResults;
    }
}