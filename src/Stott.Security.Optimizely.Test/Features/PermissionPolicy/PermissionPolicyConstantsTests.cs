using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.PermissionPolicy;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy;

[TestFixture]
public sealed class PermissionPolicyConstantsTests
{
    private const int DirectiveColumnLength = 50;

    private static readonly string[] DeprecatedDirectives =
    [
        PermissionPolicyConstants.AttributionReporting,
        PermissionPolicyConstants.BrowsingTopics,
        PermissionPolicyConstants.DocumentDomain
    ];

    [Test]
    [TestCaseSource(nameof(DeprecatedDirectives))]
    public void AllDirectives_ContainsDeprecatedDirectivesSoThatExistingConfigurationRemainsEditable(string name)
    {
        Assert.That(PermissionPolicyConstants.AllDirectives, Contains.Item(name));
    }

    [Test]
    [TestCaseSource(nameof(DeprecatedDirectives))]
    public void DefaultDirectives_ExcludesDeprecatedDirectives(string name)
    {
        Assert.That(PermissionPolicyConstants.DefaultDirectives, Does.Not.Contain(name));
    }

    [Test]
    public void DefaultDirectives_ContainsEveryDirectiveWhichHasNotBeenDeprecated()
    {
        var expected = PermissionPolicyConstants.AllDirectives.Except(DeprecatedDirectives).ToList();

        Assert.That(PermissionPolicyConstants.DefaultDirectives, Is.EqualTo(expected));
    }

    [Test]
    public void AllDirectives_DoesNotContainDuplicates()
    {
        var duplicates = PermissionPolicyConstants.AllDirectives
                                                  .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                                                  .Where(x => x.Count() > 1)
                                                  .Select(x => x.Key)
                                                  .ToList();

        Assert.That(duplicates, Is.Empty);
    }

    [Test]
    public void AllDirectives_IsOrderedByDirectiveName()
    {
        var expected = PermissionPolicyConstants.AllDirectives.OrderBy(x => x, StringComparer.Ordinal).ToList();

        Assert.That(PermissionPolicyConstants.AllDirectives, Is.EqualTo(expected));
    }

    [Test]
    public void AllDirectives_AreShortEnoughToBePersisted()
    {
        var tooLong = PermissionPolicyConstants.AllDirectives.Where(x => x.Length > DirectiveColumnLength).ToList();

        Assert.That(tooLong, Is.Empty);
    }

    [Test]
    [TestCaseSource(nameof(AllDirectiveNames))]
    public void Find_GivenAKnownDirective_ThenTheDirectiveIsReturned(string name)
    {
        var directive = PermissionPolicyConstants.Find(name);

        Assert.That(directive, Is.Not.Null);
        Assert.That(directive!.Name, Is.EqualTo(name));
    }

    [Test]
    public void Find_IsNotCaseSensitive()
    {
        var directive = PermissionPolicyConstants.Find(PermissionPolicyConstants.Geolocation.ToUpperInvariant());

        Assert.That(directive?.Name, Is.EqualTo(PermissionPolicyConstants.Geolocation));
    }

    [Test]
    [TestCase("not-a-directive")]
    [TestCase(" ")]
    [TestCase("")]
    [TestCase(null)]
    public void Find_GivenAnUnknownDirective_ThenNullIsReturned(string name)
    {
        Assert.That(PermissionPolicyConstants.Find(name), Is.Null);
    }

    [Test]
    [TestCase("identity-credentials", PermissionPolicyConstants.IdentityCredentialsGet)]
    [TestCase("IDENTITY-CREDENTIALS", PermissionPolicyConstants.IdentityCredentialsGet)]
    [TestCase("opt-credentials", PermissionPolicyConstants.OtpCredentials)]
    [TestCase("OPT-CREDENTIALS", PermissionPolicyConstants.OtpCredentials)]
    public void ResolveLegacyName_GivenALegacyDirectiveName_ThenTheCurrentNameIsReturned(string legacyName, string expectedName)
    {
        Assert.That(PermissionPolicyConstants.ResolveLegacyName(legacyName), Is.EqualTo(expectedName));
    }

    [Test]
    [TestCase(PermissionPolicyConstants.Geolocation)]
    [TestCase(PermissionPolicyConstants.OtpCredentials)]
    [TestCase("not-a-directive")]
    [TestCase(" ")]
    [TestCase("")]
    [TestCase(null)]
    public void ResolveLegacyName_GivenAnyOtherName_ThenTheNameIsReturnedUnaltered(string name)
    {
        Assert.That(PermissionPolicyConstants.ResolveLegacyName(name), Is.EqualTo(name));
    }

    [Test]
    [TestCase("identity-credentials")]
    [TestCase("opt-credentials")]
    public void AllDirectives_DoesNotContainLegacyDirectiveNames(string legacyName)
    {
        Assert.That(PermissionPolicyConstants.AllDirectives, Does.Not.Contain(legacyName));
    }

    public static IEnumerable<string> AllDirectiveNames => PermissionPolicyConstants.AllDirectives;
}
