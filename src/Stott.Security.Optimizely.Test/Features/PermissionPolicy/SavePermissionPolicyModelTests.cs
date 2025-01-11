using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy;

[TestFixture]
public sealed class SavePermissionPolicyModelTests
{
    [Test]
    [TestCaseSource(typeof(SavePermissionPolicyModelTestCases), nameof(SavePermissionPolicyModelTestCases.DirectiveNameTestCases))]
    public void Validate_WhenNameIsNull_ReturnsValidationError(string directiveName, bool isValid)
    {
        // Arrange
        var model = new SavePermissionPolicyModel
        {
            Name = directiveName,
            EnabledState = PermissionPolicyEnabledState.SpecificSites,
            Sources = ["source1", "source2"]
        };

        // Act
        var validationResults = model.Validate(new ValidationContext(model));
        var hasError = validationResults.Any(x => x.MemberNames.Contains(nameof(SavePermissionPolicyModel.Name)));

        // Assert
        Assert.That(hasError, Is.EqualTo(isValid));
    }

    [Test]
    [TestCaseSource(typeof(SavePermissionPolicyModelTestCases), nameof(SavePermissionPolicyModelTestCases.EnabledStateSourceTestCases))]
    public void Validate_SourceMustContainAtLeastOneValueWhenEnableStateRequiresSpecificSites(PermissionPolicyEnabledState enabledState, List<string> sources, bool isValid)
    {
        // Arrange
        var model = new SavePermissionPolicyModel
        {
            Name = PermissionPolicyConstants.Accelerometer,
            EnabledState = enabledState,
            Sources = sources
        };

        // Act
        var validationResults = model.Validate(new ValidationContext(model));
        var hasError = validationResults.Any(x => x.MemberNames.Contains(nameof(SavePermissionPolicyModel.Sources)));

        // Assert
        Assert.That(hasError, Is.EqualTo(isValid));
    }
}

public static class SavePermissionPolicyModelTestCases
{
    public static IEnumerable<TestCaseData> DirectiveNameTestCases
    {
        get
        {
            yield return new TestCaseData(null, true);
            yield return new TestCaseData(string.Empty, true);
            yield return new TestCaseData(" ", true);
            yield return new TestCaseData("not-a-directive", true);
            foreach (var name in PermissionPolicyConstants.AllDirectives)
            {
                yield return new TestCaseData(name, false);
            }
        }
    }

    public static IEnumerable<TestCaseData> EnabledStateSourceTestCases
    {
        get
        {
            var nullSourceList = (List<string>)null;
            var emptySources = new List<string>();
            var singleSource = new List<string> { "source1" };
            var multipleSources = new List<string> { "source1", "source2" };
            var nullSources = new List<string> { null };

            yield return new TestCaseData(PermissionPolicyEnabledState.None, nullSourceList, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.None, emptySources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.None, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.None, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.None, nullSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, nullSourceList, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, emptySources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, nullSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, nullSourceList, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, emptySources, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, nullSources, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, nullSourceList, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, emptySources, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, nullSources, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, nullSourceList, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, emptySources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, nullSources, false);
        }
    }
}