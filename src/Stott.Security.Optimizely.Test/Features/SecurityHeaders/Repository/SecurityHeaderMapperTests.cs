using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.SecurityHeaders;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;
using Stott.Security.Optimizely.Features.SecurityHeaders.Repository;

namespace Stott.Security.Optimizely.Test.Features.SecurityHeaders.Repository;

[TestFixture]
public sealed class SecurityHeaderMapperTests
{
    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.XContentTypeTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForXContentTypeOptions(
        XContentTypeOptions originalValue,
        string newValue,
        XContentTypeOptions expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { XContentTypeOptions = originalValue };
        var model = new SecurityHeaderModel { XContentTypeOptions = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.XContentTypeOptions, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.XssProtectionTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForXssProtection(
        XssProtection originalValue,
        string newValue,
        XssProtection expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { XssProtection = originalValue };
        var model = new SecurityHeaderModel { XXssProtection = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.XssProtection, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.ReferrerPolicyTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForReferrerPolicy(
        ReferrerPolicy originalValue,
        string newValue,
        ReferrerPolicy expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { ReferrerPolicy = originalValue };
        var model = new SecurityHeaderModel { ReferrerPolicy = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.ReferrerPolicy, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.XFrameOptionsTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForXFrameOptions(
        XFrameOptions originalValue,
        string newValue,
        XFrameOptions expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { FrameOptions = originalValue };
        var model = new SecurityHeaderModel { XFrameOptions = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.FrameOptions, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.CrossOriginEmbedderPolicyTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForCrossOriginEmbedderPolicy(
        CrossOriginEmbedderPolicy originalValue,
        string newValue,
        CrossOriginEmbedderPolicy expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { CrossOriginEmbedderPolicy = originalValue };
        var model = new SecurityHeaderModel { CrossOriginEmbedderPolicy = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.CrossOriginEmbedderPolicy, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.CrossOriginOpenerPolicyTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForCrossOriginOpenerPolicy(
        CrossOriginOpenerPolicy originalValue,
        string newValue,
        CrossOriginOpenerPolicy expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { CrossOriginOpenerPolicy = originalValue };
        var model = new SecurityHeaderModel { CrossOriginOpenerPolicy = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.CrossOriginOpenerPolicy, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.CrossOriginResourcePolicyTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForCrossOriginOpenerPolicy(
        CrossOriginResourcePolicy originalValue,
        string newValue,
        CrossOriginResourcePolicy expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { CrossOriginResourcePolicy = originalValue };
        var model = new SecurityHeaderModel { CrossOriginResourcePolicy = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.CrossOriginResourcePolicy, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.BooleanTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForIsStrictTransportSecurityEnabled(
        bool originalValue,
        bool newValue,
        bool expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { IsStrictTransportSecurityEnabled = originalValue };
        var model = new SecurityHeaderModel { IsStrictTransportSecurityEnabled = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.IsStrictTransportSecurityEnabled, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.BooleanTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForIsStrictTransportSecuritySubDomainsEnabled(
        bool originalValue,
        bool newValue,
        bool expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { IsStrictTransportSecuritySubDomainsEnabled = originalValue };
        var model = new SecurityHeaderModel { IsStrictTransportSecuritySubDomainsEnabled = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.IsStrictTransportSecuritySubDomainsEnabled, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.BooleanTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForForceHttpRedirect(
        bool originalValue,
        bool newValue,
        bool expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { ForceHttpRedirect = originalValue };
        var model = new SecurityHeaderModel { ForceHttpRedirect = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.ForceHttpRedirect, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCaseSource(typeof(SecurityHeaderMapperTestCases), nameof(SecurityHeaderMapperTestCases.MaxAgeTestCases))]
    public void ToEntity_OnlyAssignsValidValuesForStrictTransportSecurityMaxAge(
        int originalValue,
        int newValue,
        int expectedValue)
    {
        // Arrange
        var entity = new SecurityHeaderSettings { StrictTransportSecurityMaxAge = originalValue };
        var model = new SecurityHeaderModel { StrictTransportSecurityMaxAge = newValue };

        // Act
        SecurityHeaderMapper.ToEntity(entity, model);

        // Assert
        Assert.That(entity.StrictTransportSecurityMaxAge, Is.EqualTo(expectedValue));
    }
}