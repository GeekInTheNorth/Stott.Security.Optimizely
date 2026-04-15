namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.List;

using System;

using NUnit.Framework;

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
        var cspSource = new CspSource { Id = Guid.NewGuid(), Source = originalSource, Directives = "default-src" };
        var model = new CspPermissionListModel(cspSource, null, null);

        // Act
        Assert.That(model.SortSource, Is.EqualTo(sortSource));
    }
}