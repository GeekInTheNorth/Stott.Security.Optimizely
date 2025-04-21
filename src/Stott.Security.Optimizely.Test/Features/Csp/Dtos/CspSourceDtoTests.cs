using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Dtos;

namespace Stott.Security.Optimizely.Test.Features.Csp.Dtos;

[TestFixture]
public sealed class CspSourceDtoTests
{
    [Test]
    public void Constructor_ValidSourceAndDirectives_SetsProperties()
    {
        // Arrange
        var source = "https://example.com";
        var directives = "script-src, img-src";
        
        // Act
        var dto = new CspSourceDto(source, directives);
        
        // Assert
        Assert.That(dto.Source, Is.EqualTo(source));
        Assert.That(dto.Directives, Has.Count.EqualTo(2));
        Assert.That(dto.Directives.Contains(CspConstants.Directives.ScriptSource), Is.True);
        Assert.That(dto.Directives.Contains(CspConstants.Directives.ImageSource), Is.True);
    }

    [Test]
    public void Constructor_GivenAnInvalidDirective_TheOnlyTheValidDirectivesAreSet()
    {
        // Arrange
        var source = "https://example.com";
        var directives = "script-src, invalid-directive";

        // Act
        var dto = new CspSourceDto(source, directives);

        // Assert
        Assert.That(dto.Source, Is.EqualTo(source));
        Assert.That(dto.Directives, Has.Count.EqualTo(1));
        Assert.That(dto.Directives.Contains(CspConstants.Directives.ScriptSource), Is.True);
    }

    [Test]
    public void Constructor_GivenDirectivesContainInCorrectCasing_ThenCasingWillBeCorrectedWhenSet()
    {
        // Arrange
        var source = "https://example.com";
        var directives = "Script-Src, IMG-SRC";
        
        // Act
        var dto = new CspSourceDto(source, directives);
        
        // Assert
        Assert.That(dto.Source, Is.EqualTo(source));
        Assert.That(dto.Directives, Has.Count.EqualTo(2));
        Assert.That(dto.Directives.Contains(CspConstants.Directives.ScriptSource), Is.True);
        Assert.That(dto.Directives.Contains(CspConstants.Directives.ImageSource), Is.True);
    }
}
