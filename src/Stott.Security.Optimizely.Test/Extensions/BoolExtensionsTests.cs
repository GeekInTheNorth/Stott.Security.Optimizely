using NUnit.Framework;

using Stott.Security.Optimizely.Extensions;

namespace Stott.Security.Optimizely.Test.Extensions;

[TestFixture]
public sealed class BoolExtensionsTests
{
    [Test]
    [TestCase(true, "Yes")]
    [TestCase(false, "No")]
    public void ToYesNo_ReturnsAppropriateValue(bool valueToTest, string expectedOutcome)
    {
        // Act
        var result = valueToTest.ToYesNo();

        // Assert
        Assert.That(result, Is.EqualTo(expectedOutcome));
    }
}
