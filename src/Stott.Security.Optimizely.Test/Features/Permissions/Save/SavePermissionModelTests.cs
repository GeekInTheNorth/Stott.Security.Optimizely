using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Permissions.Save;
using Stott.Security.Optimizely.Test.TestCases;

namespace Stott.Security.Optimizely.Test.Features.Permissions.Save
{
    [TestFixture]
    public class SavePermissionModelTests
    {
        [Test]
        [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
        public void ShouldReturnInvalidForAnEmptySource(string source)
        {
            // Arrange
            var model = new SavePermissionModel
            {
                Source = source,
                Directives = new List<string> { CspConstants.Directives.DefaultSource }
            };

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
            var model = new SavePermissionModel
            {
                Source = source,
                Directives = new List<string> { CspConstants.Directives.DefaultSource }
            };

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
            var model = new SavePermissionModel
            {
                Source = source,
                Directives = new List<string> { CspConstants.Directives.DefaultSource }
            };

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
            var model = new SavePermissionModel
            {
                Source = source,
                Directives = new List<string> { CspConstants.Directives.DefaultSource }
            };

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
            var model = new SavePermissionModel
            {
                Source = source,
                Directives = new List<string> { CspConstants.Directives.DefaultSource }
            };

            // Act
            var validationResult = ValidateModel(model);

            // Assert
            Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(SavePermissionModel.Source))), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetNullOrEmptyDirectivesTestCases))]
        public void ShouldReturnValidationErrorsWhenDirectivesIsNullOrEmpty(List<string> directives)
        {
            // Arrange
            var model = new SavePermissionModel
            {
                Source = CspConstants.Sources.Self,
                Directives = directives
            };

            // Act
            var validationResult = ValidateModel(model);

            // Assert
            Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(SavePermissionModel.Directives))), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetInvalidDirectivesTestCases))]
        public void ShouldReturnValidationErrorsWhenDirectivesContainsOneOrMoreInvalidValues(List<string> directives)
        {
            // Arrange
            var model = new SavePermissionModel
            {
                Source = CspConstants.Sources.Self,
                Directives = directives
            };

            // Act
            var validationResult = ValidateModel(model);

            // Assert
            Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(SavePermissionModel.Directives))), Is.True);
        }

        [Test]
        [TestCaseSource(typeof(SavePermissionModelTestCases), nameof(SavePermissionModelTestCases.GetValidDirectivesTestCases))]
        public void ShouldReturnValidResultWhenDirectivesContainsOneOrMoreValidDirectives(List<string> directives)
        {
            // Arrange
            var model = new SavePermissionModel
            {
                Source = CspConstants.Sources.Self,
                Directives = directives
            };

            // Act
            var validationResult = ValidateModel(model);

            // Assert
            Assert.That(validationResult.Any(x => x.MemberNames.Contains(nameof(SavePermissionModel.Directives))), Is.False);
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}
