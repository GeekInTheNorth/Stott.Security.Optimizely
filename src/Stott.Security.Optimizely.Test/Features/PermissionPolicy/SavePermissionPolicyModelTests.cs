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

    [Test]
    [TestCaseSource(typeof(SavePermissionPolicyModelTestCases), nameof(SavePermissionPolicyModelTestCases.SourceTestCases))]
    public void Validate_WhenSourceIsInvalid_ReturnsValidationError(string source, bool expectedValue)
    {
        // Arrange
        var model = new SavePermissionPolicyModel
        {
            Name = PermissionPolicyConstants.Accelerometer,
            EnabledState = PermissionPolicyEnabledState.SpecificSites,
            Sources = [source]
        };

        // Act
        var validationResults = model.Validate(new ValidationContext(model));
        var hasError = validationResults.Any(x => x.MemberNames.Contains(nameof(SavePermissionPolicyModel.Sources)));

        // Assert
        Assert.That(hasError, Is.EqualTo(expectedValue));
    }
}
