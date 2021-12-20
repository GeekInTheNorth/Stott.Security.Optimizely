using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Features.Permissions.Save;
using Stott.Optimizely.Csp.Test.TestCases;

namespace Stott.Optimizely.Csp.Test.Features.Permissions.Save
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
        [TestCaseSource(typeof(SavePermissionModelTestCases),nameof(SavePermissionModelTestCases.GetSchemeSourceTestCases))]
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

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }

    public static class SavePermissionModelTestCases
    {
        public static IEnumerable<TestCaseData> GetSchemeSourceTestCases
        {
            get
            {
                yield return new TestCaseData(CspConstants.Sources.SchemeBlob, false);
                yield return new TestCaseData(CspConstants.Sources.SchemeData, false);
                yield return new TestCaseData(CspConstants.Sources.SchemeFileSystem, false);
                yield return new TestCaseData(CspConstants.Sources.SchemeHttp, false);
                yield return new TestCaseData(CspConstants.Sources.SchemeHttps, false);
                yield return new TestCaseData(CspConstants.Sources.SchemeMediaStream, false);
                yield return new TestCaseData("notascheme:", true);
                yield return new TestCaseData(CspConstants.Sources.SchemeBlob.Replace(":", string.Empty), true);
                yield return new TestCaseData(CspConstants.Sources.SchemeData.Replace(":", string.Empty), true);
                yield return new TestCaseData(CspConstants.Sources.SchemeFileSystem.Replace(":", string.Empty), true);
                yield return new TestCaseData(CspConstants.Sources.SchemeHttp.Replace(":", string.Empty), true);
                yield return new TestCaseData(CspConstants.Sources.SchemeHttps.Replace(":", string.Empty), true);
                yield return new TestCaseData(CspConstants.Sources.SchemeMediaStream.Replace(":", string.Empty), true);
            }
        }

        public static IEnumerable<TestCaseData> GetQuotedSourceTestCases
        {
            get
            {
                yield return new TestCaseData(CspConstants.Sources.Self, false);
                yield return new TestCaseData(CspConstants.Sources.UnsafeEval, false);
                yield return new TestCaseData(CspConstants.Sources.UnsafeHashes, false);
                yield return new TestCaseData(CspConstants.Sources.UnsafeInline, false);
                yield return new TestCaseData(CspConstants.Sources.None, false);
                yield return new TestCaseData("'someotherquote'", true);
                yield return new TestCaseData(CspConstants.Sources.Self.Replace("'", string.Empty), true);
                yield return new TestCaseData(CspConstants.Sources.UnsafeEval.Replace("'", string.Empty), true);
                yield return new TestCaseData(CspConstants.Sources.UnsafeHashes.Replace("'", string.Empty), true);
                yield return new TestCaseData(CspConstants.Sources.UnsafeInline.Replace("'", string.Empty), true);
                yield return new TestCaseData(CspConstants.Sources.None.Replace("'", string.Empty), true);
            }
        }

        public static IEnumerable<TestCaseData> GetValidUrlTestCases
        {
            get
            {
                yield return new TestCaseData("http://www.example.com");
                yield return new TestCaseData("http://www.example.com/");
                yield return new TestCaseData("http://www.example.com/something");
                yield return new TestCaseData("http://www.example.com/something");
                yield return new TestCaseData("https://www.example.com");
                yield return new TestCaseData("https://www.example.com/");
                yield return new TestCaseData("https://www.example.com/something");
                yield return new TestCaseData("https://www.example.com/something");
                yield return new TestCaseData("*.mailsite.com");
                yield return new TestCaseData("https://onlinebanking.jumbobank.com");
                yield return new TestCaseData("media1.com");
                yield return new TestCaseData("*.trusted.com");
            }
        }

        public static IEnumerable<TestCaseData> GetInValidUrlTestCases
        {
            get
            {
                yield return new TestCaseData("https://www.example.com?q=1");
                yield return new TestCaseData("https://www.£$.com");
                yield return new TestCaseData("example-com");
            }
        }

        public static IEnumerable<TestCaseData> GetNullOrEmptyDirectivesTestCases
        {
            get
            {
                yield return new TestCaseData((List<string>)null);
                yield return new TestCaseData(new List<string>(0));
            }
        }

        public static IEnumerable<TestCaseData> GetInvalidDirectivesTestCases
        {
            get
            {
                yield return new TestCaseData(new List<string> { $"not-a-src" });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.DefaultSource, "not-a-src" });
            }
        }

        public static IEnumerable<TestCaseData> GetValidDirectivesTestCases
        {
            get
            {
                yield return new TestCaseData(new List<string> { CspConstants.Directives.BaseUri });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ChildSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ConnectSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.DefaultSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.FontSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.FormAction });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.FrameAncestors });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.FrameSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ImageSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ManifestSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.MediaSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.NavigateTo });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ObjectSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.PreFetchSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.RequireTrustedTypes });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.Sandbox });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ScriptSourceAttribute });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ScriptSourceElement });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ScriptSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.StyleSourceAttribute });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.StyleSourceElement });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.StyleSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.TrustedTypes });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.UpgradeInsecureRequests });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.WorkerSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.BaseUri, CspConstants.Directives.ObjectSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ChildSource, CspConstants.Directives.PreFetchSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ConnectSource, CspConstants.Directives.RequireTrustedTypes });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.DefaultSource, CspConstants.Directives.Sandbox });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.FontSource, CspConstants.Directives.ScriptSourceAttribute });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.FontSource, CspConstants.Directives.ScriptSourceElement });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.FrameAncestors, CspConstants.Directives.ScriptSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.FrameSource, CspConstants.Directives.StyleSourceAttribute });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ImageSource, CspConstants.Directives.StyleSourceElement });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.ManifestSource, CspConstants.Directives.StyleSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.MediaSource, CspConstants.Directives.TrustedTypes });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.NavigateTo, CspConstants.Directives.UpgradeInsecureRequests });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.BaseUri, CspConstants.Directives.ChildSource, CspConstants.Directives.ConnectSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.DefaultSource, CspConstants.Directives.FontSource, CspConstants.Directives.FontSource });
                yield return new TestCaseData(new List<string> { CspConstants.Directives.DefaultSource, CspConstants.Directives.DefaultSource, CspConstants.Directives.DefaultSource });
            }
        }
    }
}
