namespace Stott.Security.Optimizely.Test.Features.CustomHeaders.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;
using Stott.Security.Optimizely.Features.CustomHeaders.Repository;
using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public sealed class SaveCustomHeaderModelTests
{
    private Mock<ICustomHeaderRepository> _mockRepository;

    private Mock<IServiceProvider> _mockServiceProvider;

    private ValidationContext _validationContext;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ICustomHeaderRepository>();

        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockServiceProvider.Setup(x => x.GetService(typeof(ICustomHeaderRepository))).Returns(_mockRepository.Object);

        _validationContext = new ValidationContext(new object(), _mockServiceProvider.Object, null);
    }

    [Test]
    public void Validate_GivenValidModel_ThenReturnsNoErrors()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Custom-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "custom-value"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results, Is.Empty);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void Validate_GivenEmptyHeaderName_ThenReturnsError(string headerName)
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "value"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].MemberNames, Does.Contain(nameof(SaveCustomHeaderModel.HeaderName)));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void Validate_GivenEmptyHeaderName_ThenStopsValidation(string headerName)
    {
        // Arrange - use Add behavior with empty value to trigger multiple errors if validation continues
        var model = new SaveCustomHeaderModel
        {
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = null
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert - should only get the header name error due to yield break
        Assert.That(results, Has.Count.EqualTo(1));
    }

    [Test]
    [TestCaseSource(typeof(SaveCustomHeaderModelTestCases), nameof(SaveCustomHeaderModelTestCases.InvalidHeaderNameCharacters))]
    public void Validate_GivenInvalidHeaderNameChars_ThenReturnsError(string headerName)
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Remove
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.MemberNames.Contains(nameof(SaveCustomHeaderModel.HeaderName))), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(SaveCustomHeaderModelTestCases), nameof(SaveCustomHeaderModelTestCases.ValidHeaderNameCharacters))]
    public void Validate_GivenValidHeaderNameChars_ThenReturnsNoFormatError(string headerName)
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Remove
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("invalid characters", StringComparison.OrdinalIgnoreCase)), Is.False);
    }

    [Test]
    public void Validate_GivenAddBehaviorAndEmptyValue_ThenReturnsError()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Custom-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = null
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.MemberNames.Contains(nameof(SaveCustomHeaderModel.HeaderValue))), Is.True);
    }

    [Test]
    public void Validate_GivenAddBehaviorAndNonEmptyValue_ThenReturnsNoValueError()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Custom-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "some-value"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.MemberNames.Contains(nameof(SaveCustomHeaderModel.HeaderValue))), Is.False);
    }

    [Test]
    public void Validate_GivenRemoveBehaviorAndEmptyValue_ThenReturnsNoValueError()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Custom-Header",
            Behavior = CustomHeaderBehavior.Remove,
            HeaderValue = null
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.MemberNames.Contains(nameof(SaveCustomHeaderModel.HeaderValue))), Is.False);
    }

    [Test]
    public void Validate_GivenDisabledBehaviorAndEmptyValue_ThenReturnsNoValueError()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Custom-Header",
            Behavior = CustomHeaderBehavior.Disabled,
            HeaderValue = null
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.MemberNames.Contains(nameof(SaveCustomHeaderModel.HeaderValue))), Is.False);
    }

    [Test]
    public void Validate_GivenDuplicateHeaderName_ThenReturnsError()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        _mockRepository.Setup(x => x.GetByHeaderNameAsync("X-Duplicate")).ReturnsAsync(new CustomHeader { Id = existingId, HeaderName = "X-Duplicate" });

        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Duplicate",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "value"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("already exists", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_GivenSameHeaderNameSameId_ThenReturnsNoDuplicateError()
    {
        // Arrange
        var sameId = Guid.NewGuid();
        _mockRepository.Setup(x => x.GetByHeaderNameAsync("X-Same")).ReturnsAsync(new CustomHeader { Id = sameId, HeaderName = "X-Same" });

        var model = new SaveCustomHeaderModel
        {
            Id = sameId,
            HeaderName = "X-Same",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "value"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("already exists", StringComparison.OrdinalIgnoreCase)), Is.False);
    }

    [Test]
    public void Validate_GivenRepositoryNotAvailable_ThenSkipsDuplicateCheck()
    {
        // Arrange
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(ICustomHeaderRepository))).Returns(null);
        var context = new ValidationContext(new object(), serviceProvider.Object, null);

        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-Custom-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "value"
        };

        // Act
        var results = model.Validate(context).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("already exists", StringComparison.OrdinalIgnoreCase)), Is.False);
    }

    [Test]
    public void Validate_GivenContentSecurityPolicyHeader_ThenReturnsError()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = CspConstants.HeaderNames.ContentSecurityPolicy,
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "default-src 'self'"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("managed by other features", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_GivenContentSecurityPolicyReportOnlyHeader_ThenReturnsError()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = CspConstants.HeaderNames.ReportOnlyContentSecurityPolicy,
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "default-src 'self'"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("managed by other features", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_GivenReportingEndpointsHeader_ThenReturnsError()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = CspConstants.HeaderNames.ReportingEndpoints,
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "endpoint"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("managed by other features", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_GivenPermissionsPolicyHeader_ThenReturnsError()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = PermissionPolicyConstants.PermissionPolicyHeader,
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "geolocation=()"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("managed by other features", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    [TestCaseSource(typeof(SaveCustomHeaderModelTestCases), nameof(SaveCustomHeaderModelTestCases.BuiltInCorsHeaders))]
    public void Validate_GivenBuiltInCorsHeader_ThenReturnsError(string headerName)
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = headerName,
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "*"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("managed by other features", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    [Test]
    public void Validate_GivenBuiltInHeaderInDifferentCase_ThenReturnsError()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.NewGuid(),
            HeaderName = "content-security-policy",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "default-src 'self'"
        };

        // Act
        var results = model.Validate(_validationContext).ToList();

        // Assert
        Assert.That(results.Any(x => x.ErrorMessage.Contains("managed by other features", StringComparison.OrdinalIgnoreCase)), Is.True);
    }
}
