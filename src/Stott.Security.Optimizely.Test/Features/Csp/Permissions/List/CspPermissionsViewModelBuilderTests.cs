﻿namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.List;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Csp.Permissions.List;
using Stott.Security.Optimizely.Features.Csp.Permissions.Service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestFixture]
public class CspPermissionsViewModelBuilderTests
{
    private Mock<ICspPermissionService> _mockService;

    private CspPermissionsListModelBuilder _viewModelBuilder;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ICspPermissionService>();

        _viewModelBuilder = new CspPermissionsListModelBuilder(_mockService.Object);
    }

    [Test]
    public async Task Build_GivenAnNullListOfCspSources_ThenOnlyDefaultPermissionsShouldBeReturned()
    {
        // Arrange
        _mockService.Setup(x => x.GetAsync()).ReturnsAsync((IList<CspSource>)null);

        // Act
        var model = await _viewModelBuilder.BuildAsync();

        // Assert
        Assert.That(model.Permissions, Is.Not.Empty);
        Assert.That(model.Permissions.All(x => Guid.Empty.Equals(x.Id)), Is.True);
    }

    [Test]
    public async Task Build_GivenAnEmptyListOfCspSources_ThenOnlyDefaultPermissionsShouldBeReturned()
    {
        // Arrange
        _mockService.Setup(x => x.GetAsync()).ReturnsAsync(new List<CspSource>(0));

        // Act
        var model = await _viewModelBuilder.BuildAsync();

        // Assert
        Assert.That(model.Permissions, Is.Not.Empty);
        Assert.That(model.Permissions.All(x => Guid.Empty.Equals(x.Id)), Is.True);
    }

    [Test]
    public async Task Build_GivenAListOfCspSourcesTheExcludesDefaultSources_ThenTheDefaultPermissionsShouldBeMergedIn()
    {
        // Arrange
        var sourceOne = new CspSource { Id = Guid.NewGuid(), Source = "https://*.example.com/", Directives = CspConstants.Directives.DefaultSource };
        var sourceTwo = new CspSource { Id = Guid.NewGuid(), Source = "https://*.example.co.uk/", Directives = CspConstants.Directives.DefaultSource };
        var savedSources = new List<CspSource> { sourceOne, sourceTwo };
        _mockService.Setup(x => x.GetAsync()).ReturnsAsync(savedSources);

        // Act
        var model = await _viewModelBuilder.BuildAsync();

        // Assert
        Assert.That(model.Permissions.Count, Is.EqualTo(3));
        Assert.That(model.Permissions.Count(x => x.Source.Equals(sourceOne.Source)), Is.EqualTo(1));
        Assert.That(model.Permissions.Count(x => x.Source.Equals(sourceTwo.Source)), Is.EqualTo(1));
        Assert.That(model.Permissions.Count(x => x.Source.Equals(CspConstants.Sources.Self)), Is.EqualTo(1));
    }

    [Test]
    public async Task Build_GivenAListOfCspSourcesTheIncludesDefaultSources_ThenOnlySavedPermissionsShouldBeReturned()
    {
        // Arrange
        var sourceOne = new CspSource { Id = Guid.NewGuid(), Source = "https://*.example.com/", Directives = CspConstants.Directives.DefaultSource };
        var sourceTwo = new CspSource { Id = Guid.NewGuid(), Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.DefaultSource };
        var savedSources = new List<CspSource> { sourceOne, sourceTwo };
        _mockService.Setup(x => x.GetAsync()).ReturnsAsync(savedSources);

        // Act
        var model = await _viewModelBuilder.BuildAsync();

        // Assert
        Assert.That(model.Permissions.Count, Is.EqualTo(2));
        Assert.That(model.Permissions.Count(x => x.Source.Equals(sourceOne.Source)), Is.EqualTo(1));
        Assert.That(model.Permissions.Count(x => x.Source.Equals(CspConstants.Sources.Self) && !Guid.Empty.Equals(x.Id)), Is.EqualTo(1));
    }

    [Test]
    public async Task Build_GivenAListOfCspSources_ThenCorrectlyMapsTheSourcesOntoTheViewModel()
    {
        // Arrange
        var sourceOne = new CspSource { Id = Guid.NewGuid(), Source = "https://*.example.com/", Directives = CspConstants.Directives.DefaultSource };
        var sourceTwo = new CspSource { Id = Guid.NewGuid(), Source = CspConstants.Sources.Self, Directives = CspConstants.Directives.DefaultSource };
        var savedSources = new List<CspSource> { sourceOne, sourceTwo };
        _mockService.Setup(x => x.GetAsync()).ReturnsAsync(savedSources);

        // Act
        var model = await _viewModelBuilder.BuildAsync();

        // Assert
        Assert.That(model.Permissions[1].Id, Is.EqualTo(sourceOne.Id));
        Assert.That(model.Permissions[1].Source, Is.EqualTo(sourceOne.Source));
        Assert.That(model.Permissions[1].Directives, Is.EqualTo(sourceOne.Directives));
        Assert.That(model.Permissions[0].Id, Is.EqualTo(sourceTwo.Id));
        Assert.That(model.Permissions[0].Source, Is.EqualTo(sourceTwo.Source));
        Assert.That(model.Permissions[0].Directives, Is.EqualTo(sourceTwo.Directives));
    }
}