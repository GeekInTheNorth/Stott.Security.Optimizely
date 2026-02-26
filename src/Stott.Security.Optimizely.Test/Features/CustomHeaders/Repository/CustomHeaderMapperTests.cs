namespace Stott.Security.Optimizely.Test.Features.CustomHeaders.Repository;

using System;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.CustomHeaders.Repository;

[TestFixture]
public sealed class CustomHeaderMapperTests
{
    [Test]
    public void ToModel_GivenEntity_ThenAllPropertiesAreMapped()
    {
        // Arrange
        var entity = new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Custom-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "custom-value"
        };

        // Act
        var result = CustomHeaderMapper.ToModel(entity);

        // Assert
        Assert.That(result.Id, Is.EqualTo(entity.Id));
        Assert.That(result.HeaderName, Is.EqualTo(entity.HeaderName));
        Assert.That(result.Behavior, Is.EqualTo(entity.Behavior));
        Assert.That(result.HeaderValue, Is.EqualTo(entity.HeaderValue));
    }

    [Test]
    public void ToModel_GivenEntity_ThenCanDeleteIsTrue()
    {
        // Arrange
        var entity = new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Custom-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "custom-value"
        };

        // Act
        var result = CustomHeaderMapper.ToModel(entity);

        // Assert
        Assert.That(result.CanDelete, Is.True);
    }

    [Test]
    [TestCaseSource(typeof(CustomHeaderMapperTestCases), nameof(CustomHeaderMapperTestCases.FixedHeaderNames))]
    public void ToModel_GivenEntityWithFixedHeaderName_ThenIsHeaderNameEditableIsFalse(string headerName)
    {
        // Arrange
        var entity = new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "some-value"
        };

        // Act
        var result = CustomHeaderMapper.ToModel(entity);

        // Assert
        Assert.That(result.IsHeaderNameEditable, Is.False);
    }

    [Test]
    public void ToModel_GivenEntityWithCustomHeaderName_ThenIsHeaderNameEditableIsTrue()
    {
        // Arrange
        var entity = new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Custom-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "custom-value"
        };

        // Act
        var result = CustomHeaderMapper.ToModel(entity);

        // Assert
        Assert.That(result.IsHeaderNameEditable, Is.True);
    }

    [Test]
    [TestCaseSource(typeof(CustomHeaderMapperTestCases), nameof(CustomHeaderMapperTestCases.FixedHeaderNames))]
    public void ToModel_GivenEntityWithFixedHeaderName_ThenDescriptionIsNotNull(string headerName)
    {
        // Arrange
        var entity = new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Disabled
        };

        // Act
        var result = CustomHeaderMapper.ToModel(entity);

        // Assert
        Assert.That(result.Description, Is.Not.Null);
    }

    [Test]
    [TestCaseSource(typeof(CustomHeaderMapperTestCases), nameof(CustomHeaderMapperTestCases.FixedSelectHeaders))]
    public void ToModel_GivenEntityWithFixedSelectHeader_ThenAllowedValuesIsNotNull(string headerName)
    {
        // Arrange
        var entity = new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Disabled
        };

        // Act
        var result = CustomHeaderMapper.ToModel(entity);

        // Assert
        Assert.That(result.AllowedValues, Is.Not.Null);
    }

    [Test]
    [TestCaseSource(typeof(CustomHeaderMapperTestCases), nameof(CustomHeaderMapperTestCases.NonHstsFixedHeaders))]
    public void ToModel_GivenEntityWithNonHstsFixedHeader_ThenPropertyTypeIsSelect(string headerName)
    {
        // Arrange
        var entity = new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Disabled
        };

        // Act
        var result = CustomHeaderMapper.ToModel(entity);

        // Assert
        Assert.That(result.PropertyType, Is.EqualTo("select"));
    }

    [Test]
    public void ToModel_GivenString_ThenBehaviorIsDisabled()
    {
        // Act
        var result = CustomHeaderMapper.ToModel("X-Custom-Header");

        // Assert
        Assert.That(result.Behavior, Is.EqualTo(CustomHeaderBehavior.Disabled));
    }

    [Test]
    public void ToModel_GivenString_ThenCanDeleteIsFalse()
    {
        // Act
        var result = CustomHeaderMapper.ToModel("X-Custom-Header");

        // Assert
        Assert.That(result.CanDelete, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(CustomHeaderMapperTestCases), nameof(CustomHeaderMapperTestCases.FixedHeaderNames))]
    public void ToModel_GivenStringWithFixedHeaderName_ThenIsHeaderNameEditableIsFalse(string headerName)
    {
        // Act
        var result = CustomHeaderMapper.ToModel(headerName);

        // Assert
        Assert.That(result.IsHeaderNameEditable, Is.False);
    }

    [Test]
    public void ToModel_GivenStringWithCustomHeaderName_ThenIsHeaderNameEditableIsTrue()
    {
        // Act
        var result = CustomHeaderMapper.ToModel("X-Custom-Header");

        // Assert
        Assert.That(result.IsHeaderNameEditable, Is.True);
    }

    [Test]
    public void GetPropertyType_GivenStrictTransportSecurity_ThenReturnsHsts()
    {
        // Act
        var result = CustomHeaderMapper.GetPropertyType(CspConstants.HeaderNames.StrictTransportSecurity);

        // Assert
        Assert.That(result, Is.EqualTo("hsts"));
    }

    [Test]
    public void GetPropertyType_GivenStrictTransportSecurityInDifferentCase_ThenReturnsHsts()
    {
        // Act
        var result = CustomHeaderMapper.GetPropertyType("strict-transport-security");

        // Assert
        Assert.That(result, Is.EqualTo("hsts"));
    }

    [Test]
    [TestCaseSource(typeof(CustomHeaderMapperTestCases), nameof(CustomHeaderMapperTestCases.NonHstsFixedHeaders))]
    public void GetPropertyType_GivenNonHstsFixedHeader_ThenReturnsSelect(string headerName)
    {
        // Act
        var result = CustomHeaderMapper.GetPropertyType(headerName);

        // Assert
        Assert.That(result, Is.EqualTo("select"));
    }

    [Test]
    public void GetPropertyType_GivenCustomHeaderName_ThenReturnsString()
    {
        // Act
        var result = CustomHeaderMapper.GetPropertyType("X-Custom-Header");

        // Assert
        Assert.That(result, Is.EqualTo("string"));
    }

    [Test]
    public void GetPropertyType_GivenNull_ThenReturnsString()
    {
        // Act
        var result = CustomHeaderMapper.GetPropertyType(null);

        // Assert
        Assert.That(result, Is.EqualTo("string"));
    }

    [Test]
    [TestCaseSource(typeof(CustomHeaderMapperTestCases), nameof(CustomHeaderMapperTestCases.FixedHeaderNames))]
    public void GetDescriptionForHeaderName_GivenFixedHeader_ThenReturnsNonNullDescription(string headerName)
    {
        // Act
        var result = CustomHeaderMapper.GetDescriptionForHeaderName(headerName);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetDescriptionForHeaderName_GivenCustomHeaderName_ThenReturnsNull()
    {
        // Act
        var result = CustomHeaderMapper.GetDescriptionForHeaderName("X-Custom-Header");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetDescriptionForHeaderName_GivenNull_ThenReturnsNull()
    {
        // Act
        var result = CustomHeaderMapper.GetDescriptionForHeaderName(null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetDescriptionForHeaderName_IsCaseInsensitive()
    {
        // Act
        var result = CustomHeaderMapper.GetDescriptionForHeaderName("x-frame-options");

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    [TestCaseSource(typeof(CustomHeaderMapperTestCases), nameof(CustomHeaderMapperTestCases.FixedSelectHeaders))]
    public void GetAllowedValues_GivenFixedSelectHeader_ThenReturnsNonNullList(string headerName)
    {
        // Act
        var result = CustomHeaderMapper.GetAllowedValues(headerName);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetAllowedValues_GivenStrictTransportSecurity_ThenReturnsNull()
    {
        // Act
        var result = CustomHeaderMapper.GetAllowedValues(CspConstants.HeaderNames.StrictTransportSecurity);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetAllowedValues_GivenCustomHeaderName_ThenReturnsNull()
    {
        // Act
        var result = CustomHeaderMapper.GetAllowedValues("X-Custom-Header");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetAllowedValues_GivenNull_ThenReturnsNull()
    {
        // Act
        var result = CustomHeaderMapper.GetAllowedValues(null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    [TestCaseSource(typeof(CustomHeaderMapperTestCases), nameof(CustomHeaderMapperTestCases.FixedSelectHeaders))]
    public void GetAllowedValues_GivenFixedSelectHeader_ThenFirstItemHasEmptyValue(string headerName)
    {
        // Act
        var result = CustomHeaderMapper.GetAllowedValues(headerName);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.First().Value, Is.EqualTo(string.Empty));
    }

    [Test]
    public void ToEntity_MapsHeaderNameBehaviorAndValue()
    {
        // Arrange
        var model = new Stott.Security.Optimizely.Features.CustomHeaders.Models.SaveCustomHeaderModel
        {
            HeaderName = "X-Test-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "test-value"
        };

        var entity = new CustomHeader
        {
            Id = Guid.NewGuid(),
            Modified = DateTime.UtcNow,
            ModifiedBy = "test-user"
        };

        // Act
        CustomHeaderMapper.ToEntity(model, entity);

        // Assert
        Assert.That(entity.HeaderName, Is.EqualTo("X-Test-Header"));
        Assert.That(entity.Behavior, Is.EqualTo(CustomHeaderBehavior.Add));
        Assert.That(entity.HeaderValue, Is.EqualTo("test-value"));
    }

    [Test]
    public void ToEntity_DoesNotOverwriteIdOrAuditFields()
    {
        // Arrange
        var originalId = Guid.NewGuid();
        var originalModified = new DateTime(2025, 1, 1);
        var originalModifiedBy = "original-user";

        var model = new Stott.Security.Optimizely.Features.CustomHeaders.Models.SaveCustomHeaderModel
        {
            HeaderName = "X-Test-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "test-value"
        };

        var entity = new CustomHeader
        {
            Id = originalId,
            Modified = originalModified,
            ModifiedBy = originalModifiedBy
        };

        // Act
        CustomHeaderMapper.ToEntity(model, entity);

        // Assert
        Assert.That(entity.Id, Is.EqualTo(originalId));
        Assert.That(entity.Modified, Is.EqualTo(originalModified));
        Assert.That(entity.ModifiedBy, Is.EqualTo(originalModifiedBy));
    }
}
