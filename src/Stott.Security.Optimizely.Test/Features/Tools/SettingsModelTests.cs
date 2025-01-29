using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
using Stott.Security.Optimizely.Features.SecurityHeaders;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;
using Stott.Security.Optimizely.Features.Tools;

namespace Stott.Security.Optimizely.Test.Features.Tools;

public sealed class SettingsModelTests
{
    [Test]
    public void Validate_DoesNotReturnErrorsForAViableModel()
    {
        // Arrange
        var model = GetMinimalViableModel();

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public void Validate_ReturnsAnErrorWhenCspIsNull()
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Csp = null;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Is.Not.Empty);
    }

    [Test]
    public void Validate_ReturnsAnErrorWhenCspSandboxIsNull()
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Csp.Sandbox = null;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Is.Not.Empty);
    }

    [Test]
    public void Validate_ReturnsAnErrorWhenCspSourcesIsNull()
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Csp.Sources = null;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Is.Not.Empty);
    }

    [Test]
    public void Validate_ReturnsAnErrorWhenCorsIsNull()
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Cors = null;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Is.Not.Empty);
    }

    [Test]
    public void Validate_ReturnsAnErrorWhenHeadersIsNull()
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Headers = null;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Is.Not.Empty);
    }

    [Test]
    [TestCaseSource(typeof(SettingsModelTestCases), nameof(SettingsModelTestCases.XContentTypeOptionsTestCases))]
    public void Validate_ReturnsAnErrorWhenHeadersContainsAnInvalidXContentTypeOptions(string testValue, int expectedErrors)
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Headers.XContentTypeOptions = testValue;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Has.Count.EqualTo(expectedErrors));
    }

    [Test]
    [TestCaseSource(typeof(SettingsModelTestCases), nameof(SettingsModelTestCases.XXssProtectionTestCases))]
    public void Validate_ReturnsAnErrorWhenHeadersContainsAnInvalidXXssProtection(string testValue, int expectedErrors)
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Headers.XXssProtection = testValue;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Has.Count.EqualTo(expectedErrors));
    }

    [Test]
    [TestCaseSource(typeof(SettingsModelTestCases), nameof(SettingsModelTestCases.ReferrerPolicyTestCases))]
    public void Validate_ReturnsAnErrorWhenHeadersContainsAnInvalidReferrerPolicy(string testValue, int expectedErrors)
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Headers.ReferrerPolicy = testValue;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Has.Count.EqualTo(expectedErrors));
    }

    [Test]
    [TestCaseSource(typeof(SettingsModelTestCases), nameof(SettingsModelTestCases.XFrameOptionsTestCases))]
    public void Validate_ReturnsAnErrorWhenHeadersContainsAnInvalidXFrameOptions(string testValue, int expectedErrors)
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Headers.XFrameOptions = testValue;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Has.Count.EqualTo(expectedErrors));
    }

    [Test]
    [TestCaseSource(typeof(SettingsModelTestCases), nameof(SettingsModelTestCases.CrossOriginEmbedderPolicyTestCases))]
    public void Validate_ReturnsAnErrorWhenHeadersContainsAnInvalidCrossOriginEmbedderPolicy(string testValue, int expectedErrors)
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Headers.CrossOriginEmbedderPolicy = testValue;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Has.Count.EqualTo(expectedErrors));
    }

    [Test]
    [TestCaseSource(typeof(SettingsModelTestCases), nameof(SettingsModelTestCases.CrossOriginOpenerPolicyTestCases))]
    public void Validate_ReturnsAnErrorWhenHeadersContainsAnInvalidCrossOriginOpenerPolicy(string testValue, int expectedErrors)
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Headers.CrossOriginOpenerPolicy = testValue;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Has.Count.EqualTo(expectedErrors));
    }

    [Test]
    [TestCaseSource(typeof(SettingsModelTestCases), nameof(SettingsModelTestCases.CrossOriginResourcePolicyTestCases))]
    public void Validate_ReturnsAnErrorWhenHeadersContainsAnInvalidCrossOriginResourcePolicy(string testValue, int expectedErrors)
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Headers.CrossOriginResourcePolicy = testValue;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Has.Count.EqualTo(expectedErrors));
    }

    [Test]
    [TestCaseSource(typeof(SettingsModelTestCases), nameof(SettingsModelTestCases.MaxAgeTestCases))]
    public void Validate_ReturnsAnErrorWhenHeadersContainsAnInvalidStrictTransportSecurityMaxAge(int testValue, int expectedErrors)
    {
        // Arrange
        var model = GetMinimalViableModel();
        model.Headers.StrictTransportSecurityMaxAge = testValue;

        // Act
        var errors = model.Validate(null).ToList();

        // Assert
        Assert.That(errors, Has.Count.EqualTo(expectedErrors));
    }

    private static SettingsModel GetMinimalViableModel()
    {
        return new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                Sandbox = new SandboxModel(),
                Sources = new List<CspSourceModel>(0)
            },
            Cors = new CorsConfiguration(),
            Headers = new SecurityHeaderModel
            {
                XContentTypeOptions = XContentTypeOptions.None.ToString(),
                XXssProtection = XssProtection.None.ToString(),
                ReferrerPolicy = ReferrerPolicy.None.ToString(),
                XFrameOptions = XFrameOptions.None.ToString(),
                CrossOriginEmbedderPolicy = CrossOriginEmbedderPolicy.None.ToString(),
                CrossOriginOpenerPolicy = CrossOriginOpenerPolicy.None.ToString(),
                CrossOriginResourcePolicy = CrossOriginResourcePolicy.None.ToString(),
                StrictTransportSecurityMaxAge = 1
            }
        };
    }
}