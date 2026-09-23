using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy.Models;

[TestFixture]
public sealed class PermissionPolicyDirectiveModelTests
{
    public static IEnumerable<PermissionPolicyDirective> AllDirectives =>
        PermissionPolicyConstants.AllDirectives.Select(x => PermissionPolicyConstants.Find(x)!);

    [Test]
    [TestCaseSource(nameof(AllDirectives))]
    public void Constructor_GivenADirective_ThenMetaDataIsCopiedFromTheDirective(PermissionPolicyDirective directive)
    {
        var model = new PermissionPolicyDirectiveModel(directive);

        Assert.Multiple(() =>
        {
            Assert.That(model.Name, Is.EqualTo(directive.Name));
            Assert.That(model.Title, Is.EqualTo(directive.Title).And.Not.Empty.And.Not.EqualTo(directive.Name));
            Assert.That(model.Description, Is.EqualTo(directive.Description).And.Not.Empty);
            Assert.That(model.IsDeprecated, Is.EqualTo(directive.IsDeprecated));
        });
    }

    [Test]
    [TestCaseSource(nameof(AllDirectives))]
    public void Constructor_GivenADirective_ThenTheModelIsUnconfigured(PermissionPolicyDirective directive)
    {
        var model = new PermissionPolicyDirectiveModel(directive);

        Assert.Multiple(() =>
        {
            Assert.That(model.EnabledState, Is.EqualTo(PermissionPolicyEnabledState.Disabled));
            Assert.That(model.Sources, Is.Empty);
        });
    }

    [Test]
    [TestCase(PermissionPolicyConstants.AttributionReporting)]
    [TestCase(PermissionPolicyConstants.BrowsingTopics)]
    [TestCase(PermissionPolicyConstants.DocumentDomain)]
    public void Constructor_GivenADeprecatedDirective_ThenTheModelIsFlaggedAsDeprecated(string name)
    {
        var model = new PermissionPolicyDirectiveModel(PermissionPolicyConstants.Find(name)!);

        Assert.That(model.IsDeprecated, Is.True);
    }

    [Test]
    [TestCaseSource(nameof(CurrentDirectives))]
    public void Constructor_GivenACurrentDirective_ThenTheModelIsNotFlaggedAsDeprecated(PermissionPolicyDirective directive)
    {
        var model = new PermissionPolicyDirectiveModel(directive);

        Assert.That(model.IsDeprecated, Is.False);
    }

    [Test]
    public void Constructor_GivenAnEntityAndADirective_ThenTheConfigurationAndMetaDataAreMapped()
    {
        // Arrange
        var entity = new Stott.Security.Optimizely.Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.Camera,
            EnabledState = nameof(PermissionPolicyEnabledState.SpecificSites),
            Origins = "https://www.example.com, https://www.example.org"
        };
        var directive = PermissionPolicyConstants.Find(PermissionPolicyConstants.Camera)!;

        // Act
        var model = new PermissionPolicyDirectiveModel(entity, directive);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(model.Name, Is.EqualTo(PermissionPolicyConstants.Camera));
            Assert.That(model.Title, Is.EqualTo(directive.Title));
            Assert.That(model.Description, Is.EqualTo(directive.Description));
            Assert.That(model.IsDeprecated, Is.False);
            Assert.That(model.EnabledState, Is.EqualTo(PermissionPolicyEnabledState.SpecificSites));
            Assert.That(model.Sources.Select(x => x.Url), Is.EqualTo(new[] { "https://www.example.com", "https://www.example.org" }));
        });
    }

    [Test]
    public void Constructor_GivenAnEntityAndADeprecatedDirective_ThenTheModelIsFlaggedAsDeprecated()
    {
        // Arrange
        var entity = new Stott.Security.Optimizely.Entities.PermissionPolicy
        {
            Directive = PermissionPolicyConstants.DocumentDomain,
            EnabledState = nameof(PermissionPolicyEnabledState.None)
        };
        var directive = PermissionPolicyConstants.Find(PermissionPolicyConstants.DocumentDomain)!;

        // Act
        var model = new PermissionPolicyDirectiveModel(entity, directive);

        // Assert
        Assert.That(model.IsDeprecated, Is.True);
    }

    [Test]
    public void Constructor_GivenAnEntityWithAnUnrecognisedDirective_ThenTheNameIsUsedAsTheTitle()
    {
        // Arrange
        var entity = new Stott.Security.Optimizely.Entities.PermissionPolicy
        {
            Directive = "not-a-directive",
            EnabledState = "not-a-state",
            Origins = null
        };

        // Act
        var model = new PermissionPolicyDirectiveModel(entity, null);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(model.Title, Is.EqualTo("not-a-directive"));
            Assert.That(model.Description, Is.Empty);
            Assert.That(model.IsDeprecated, Is.False);
            Assert.That(model.EnabledState, Is.EqualTo(PermissionPolicyEnabledState.None));
            Assert.That(model.Sources, Is.Empty);
        });
    }

    [Test]
    public void Deserialization_GivenAConfiguration_ThenTheConfigurationIsBoundWithoutMetaData()
    {
        // Arrange
        const string json = """{"name":"geolocation","enabledState":"ThisSite","sources":[]}""";
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        // Act
        var model = JsonSerializer.Deserialize<PermissionPolicyDirectiveModel>(json, options);

        // Assert
        Assert.That(model, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(model!.Name, Is.EqualTo(PermissionPolicyConstants.Geolocation));
            Assert.That(model.EnabledState, Is.EqualTo(PermissionPolicyEnabledState.ThisSite));

            // Meta data is only applied when serving directives, not when accepting them.
            Assert.That(model.Title, Is.Null);
            Assert.That(model.Description, Is.Null);
            Assert.That(model.IsDeprecated, Is.False);
        });
    }

    public static IEnumerable<PermissionPolicyDirective> CurrentDirectives => PermissionPolicyConstants.DefaultDirectiveDefinitions;
}
