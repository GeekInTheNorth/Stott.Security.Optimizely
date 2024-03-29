﻿namespace Stott.Security.Optimizely.Test.Features.Cors;

using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Cors;

[TestFixture]
public sealed class CorsConfigurationTests
{
    [Test]
    [TestCaseSource(typeof(CorsConfigurationTestCases), nameof(CorsConfigurationTestCases.InvalidOriginTestCases))]
    public void Validate_GivenAnInvalidOrigin_ThenAnErrorWillBeReturned(string origin)
    {
        // Arrange
        var configuration = new CorsConfiguration
        {
            AllowOrigins = new List<CorsConfigurationItem>
            {
                new() { Id = Guid.NewGuid(), Value = origin }
            }
        };

        // Act
        var result = configuration.Validate(null).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    [TestCaseSource(typeof(CorsConfigurationTestCases), nameof(CorsConfigurationTestCases.ValidOriginTestCases))]
    public void Validate_GivenAnValidOrigin_ThenAnErrorWillBeReturned(string origin)
    {
        // Arrange
        var configuration = new CorsConfiguration
        {
            AllowOrigins = new List<CorsConfigurationItem>
            {
                new() { Id = Guid.NewGuid(), Value = origin }
            }
        };

        // Act
        var result = configuration.Validate(null);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    [TestCaseSource(typeof(CorsConfigurationTestCases), nameof(CorsConfigurationTestCases.InvalidHeaderTestCases))]
    public void Validate_GivenAnInvalidAllowHeader_ThenAnErrorWillBeReturned(string header)
    {
        // Arrange
        var configuration = new CorsConfiguration
        {
            AllowHeaders = new List<CorsConfigurationItem>
            {
                new() { Id = Guid.NewGuid(), Value = header }
            }
        };

        // Act
        var result = configuration.Validate(null).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    [TestCaseSource(typeof(CorsConfigurationTestCases), nameof(CorsConfigurationTestCases.ValidHeaderTestCases))]
    public void Validate_GivenAnValidAllowHeader_ThenAnErrorWillNotBeReturned(string header)
    {
        // Arrange
        var configuration = new CorsConfiguration
        {
            AllowHeaders = new List<CorsConfigurationItem>
            {
                new() { Id = Guid.NewGuid(), Value = header }
            }
        };

        // Act
        var result = configuration.Validate(null).ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    [TestCaseSource(typeof(CorsConfigurationTestCases), nameof(CorsConfigurationTestCases.InvalidHeaderTestCases))]
    public void Validate_GivenAnInvalidExposeHeader_ThenAnErrorWillBeReturned(string header)
    {
        // Arrange
        var configuration = new CorsConfiguration
        {
            ExposeHeaders = new List<CorsConfigurationItem>
            {
                new() { Id = Guid.NewGuid(), Value = header }
            }
        };

        // Act
        var result = configuration.Validate(null).ToList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    [TestCaseSource(typeof(CorsConfigurationTestCases), nameof(CorsConfigurationTestCases.ValidHeaderTestCases))]
    public void Validate_GivenAnValidExposeHeader_ThenAnErrorWillNotBeReturned(string header)
    {
        // Arrange
        var configuration = new CorsConfiguration
        {
            ExposeHeaders = new List<CorsConfigurationItem>
            {
                new() { Id = Guid.NewGuid(), Value = header }
            }
        };

        // Act
        var result = configuration.Validate(null).ToList();

        // Assert
        Assert.That(result, Is.Empty);
    }
}