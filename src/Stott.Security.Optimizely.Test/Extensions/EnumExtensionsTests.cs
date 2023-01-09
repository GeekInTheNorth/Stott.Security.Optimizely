namespace Stott.Security.Optimizely.Test.Extensions;

using NUnit.Framework;

using Stott.Security.Optimizely.Extensions;
using Stott.Security.Optimizely.Features.SecurityHeaders.Enums;

[TestFixture]
public sealed class EnumExtensionsTests
{
    [Test]
    [TestCase(CrossOriginEmbedderPolicy.None, null)]
    [TestCase(CrossOriginEmbedderPolicy.UnsafeNone, "unsafe-none")]
    [TestCase(CrossOriginEmbedderPolicy.RequireCorp, "require-corp")]
    public void GetSecurityHeaderValue_CorrectlyConvertsCrossOriginEmbedderPolicy(
        CrossOriginEmbedderPolicy enumValue, 
        string securityHeaderValue)
    {
        // Act
        var actualValue = enumValue.GetSecurityHeaderValue();

        // Assert
        Assert.That(actualValue, Is.EqualTo(securityHeaderValue));
    }
}