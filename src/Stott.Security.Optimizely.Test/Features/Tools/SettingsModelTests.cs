using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Csp.Sandbox;
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

    private static SettingsModel GetMinimalViableModel()
    {
        return new SettingsModel
        {
            Csp = new CspSettingsModel
            {
                Sandbox = new SandboxModel(),
                Sources = []
            },
            Cors = new CorsConfiguration(),
            PermissionPolicy = new PermissionPolicyModel
            {
                Directives = []
            }
        };
    }
}