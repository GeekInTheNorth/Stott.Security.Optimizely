namespace Stott.Security.Optimizely.Test.Features.Csp.AllowList;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.AllowList;

[TestFixture]
public class AllowListCollectionTests
{
    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.InvalidAllowListTests))]
    public void IsOnAllowList_ReturnsFalseWhenViolationSourceOrDirectiveIsNullOrEmpty(string violationSource, string directive)
    {
        // Arrange
        var allowListEntries = new List<AllowListEntry>
        {
            CreateAllowListEntry("https://www.example.com", CspConstants.Directives.DefaultSource),
            CreateAllowListEntry("https://www.exampleTwo.com", CspConstants.Directives.DefaultSource)
        };

        var allowListCollection = new AllowListCollection(allowListEntries);

        // Act
        var isOnAllowList = allowListCollection.IsOnAllowList(violationSource, directive);

        // Assert
        Assert.That(isOnAllowList, Is.False);
    }

    [Test]
    [TestCaseSource(typeof(AllowListServiceTestCases), nameof(AllowListServiceTestCases.AllowListTests))]
    public void IsOnAllowList_WhenDomainMatchesAAllowListEntry_ThenReturnsTrue(
        string violatedSource,
        string violatedDirective,
        string allowedDomain,
        string allowedDirective,
        bool expectedResult)
    {
        // Arrange
        var allowListEntries = new List<AllowListEntry>
        {
            CreateAllowListEntry("https://www.example.com", CspConstants.Directives.DefaultSource),
            CreateAllowListEntry(allowedDomain, allowedDirective)
        };

        var allowListCollection = new AllowListCollection(allowListEntries);

        // Act
        var isOnAllowList = allowListCollection.IsOnAllowList(violatedSource, violatedDirective);

        // Assert
        Assert.That(isOnAllowList, Is.EqualTo(expectedResult));
    }

    private static AllowListEntry CreateAllowListEntry(string sourceUrl, string directive)
    {
        return new AllowListEntry
        {
            SourceUrl = sourceUrl,
            Directives = new List<string>
            {
                directive
            }
        };
    }
}
