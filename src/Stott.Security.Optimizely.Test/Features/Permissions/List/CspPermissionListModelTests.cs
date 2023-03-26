namespace Stott.Security.Optimizely.Test.Features.Permissions.List;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Permissions.List;

[TestFixture]
public sealed class CspPermissionListModelTests
{
    [Test]
    [TestCaseSource(typeof(CspPermissionListModelTestCases), nameof(CspPermissionListModelTestCases.SortSourceTestCases))]
    public void CorrectlyAssemblesSortString(string originalSource, string sortSource)
    {
        // Arrange
        var model = new CspPermissionListModel(originalSource, CspConstants.Directives.DefaultSource);

        // Act
        Assert.That(model.SortSource, Is.EqualTo(sortSource));
    }
}