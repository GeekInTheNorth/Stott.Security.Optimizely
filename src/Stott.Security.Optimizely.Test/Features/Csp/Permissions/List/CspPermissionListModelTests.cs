namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.List;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Permissions.List;

[TestFixture]
public sealed class CspPermissionListModelTests
{
    [Test]
    [TestCaseSource(typeof(CspPermissionListModelTestCases), nameof(CspPermissionListModelTestCases.SortSourceTestCases))]
    public void CorrectlyAssemblesSortString(string originalSource, string sortSource)
    {
        // Arrange
        var source = new CspSource { Source = originalSource, Directives = CspConstants.Directives.DefaultSource };

        // Act
        var model = new CspPermissionListModel(source, null, null);

        // Assert
        Assert.That(model.SortSource, Is.EqualTo(sortSource));
    }
}