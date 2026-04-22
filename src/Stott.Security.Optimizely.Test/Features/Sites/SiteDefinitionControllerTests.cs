namespace Stott.Security.Optimizely.Test.Features.Sites;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

using EPiServer.Web;

using Microsoft.AspNetCore.Mvc;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Sites;

[TestFixture]
public sealed class SiteDefinitionControllerTests
{
    private static readonly Guid SiteOneId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid SiteTwoId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private Mock<ISiteDefinitionRepository> _mockSiteRepository;

    private SiteDefinitionController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockSiteRepository = new Mock<ISiteDefinitionRepository>();
        _controller = new SiteDefinitionController(_mockSiteRepository.Object);
    }

    [Test]
    public void Sites_WhenRepositoryReturnsTwoSites_ThenResponseContainsGlobalSentinelPlusBothSites()
    {
        // Arrange
        _mockSiteRepository.Setup(x => x.List()).Returns(
        [
            CreateSite(SiteOneId, "Site One", CreateHost("localhost:44344", HostDefinitionType.Primary)),
            CreateSite(SiteTwoId, "Site Two", CreateHost("localhost:44346", HostDefinitionType.Primary)),
        ]);

        // Act
        var result = DeserializeSitesResponse(_controller.Sites());

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        Assert.That(result.Select(x => x.SiteId), Is.EquivalentTo([Guid.Empty, SiteOneId, SiteTwoId]));
    }

    [Test]
    public void Sites_WhenRepositoryReturnsSites_ThenTheGlobalAllSitesSentinelIsTheFirstItem()
    {
        // Arrange
        _mockSiteRepository.Setup(x => x.List()).Returns(
        [
            CreateSite(SiteOneId, "Site One", CreateHost("localhost:44344", HostDefinitionType.Primary)),
        ]);

        // Act
        var result = DeserializeSitesResponse(_controller.Sites());

        // Assert
        Assert.That(result[0].SiteId, Is.EqualTo(Guid.Empty));
        Assert.That(result[0].SiteName, Is.EqualTo("All Sites"));
        Assert.That(result[0].HasMultipleHosts, Is.False);
    }

    [Test]
    public void Sites_ComputesHasMultipleHostsFromCountOfHostsWithNonNullUrl()
    {
        // Arrange — Site One has a single URL-bound host; Site Two has two.
        _mockSiteRepository.Setup(x => x.List()).Returns(
        [
            CreateSite(SiteOneId, "Site One", CreateHost("localhost:44344", HostDefinitionType.Primary)),
            CreateSite(SiteTwoId, "Site Two", CreateHost("localhost:44346", HostDefinitionType.Primary), CreateHost("localhost:44347", HostDefinitionType.Edit))
        ]);

        // Act
        var result = DeserializeSitesResponse(_controller.Sites());

        // Assert
        Assert.That(result.Single(x => x.SiteId == SiteOneId).HasMultipleHosts, Is.False);
        Assert.That(result.Single(x => x.SiteId == SiteTwoId).HasMultipleHosts, Is.True);
    }

    [Test]
    public void Sites_IncludesHostSummariesWithTypeAndLanguage()
    {
        // Arrange
        var host = CreateHost("example.com", HostDefinitionType.Primary, new CultureInfo("en"));
        _mockSiteRepository.Setup(x => x.List()).Returns(
        [
            CreateSite(SiteOneId, "Site One", host),
        ]);

        // Act
        var result = DeserializeSitesResponse(_controller.Sites());

        // Assert
        var siteOne = result.Single(x => x.SiteId == SiteOneId);
        var exampleHost = siteOne.AvailableHosts.Single(x => x.HostName == "example.com");
        Assert.That(exampleHost.HostType, Is.EqualTo("Primary"));
        Assert.That(exampleHost.HostLanguage, Is.Not.Null);
        Assert.That(exampleHost.HostLanguage, Is.Not.Empty);
    }

    [Test]
    public void Sites_AlwaysIncludesADefaultSentinelHostEvenWhenNoHostsAreConfigured()
    {
        // Arrange — a site with no hosts at all.
        _mockSiteRepository.Setup(x => x.List()).Returns(
        [
            CreateSite(SiteOneId, "Site One"),
        ]);

        // Act
        var result = DeserializeSitesResponse(_controller.Sites());

        // Assert
        var siteOne = result.Single(x => x.SiteId == SiteOneId);
        Assert.That(siteOne.AvailableHosts, Has.Count.EqualTo(1));
        Assert.That(siteOne.AvailableHosts[0].DisplayName, Is.EqualTo("Default"));
        Assert.That(siteOne.AvailableHosts[0].HostName, Is.EqualTo(string.Empty));
    }

    private static SiteDefinition CreateSite(Guid id, string name, params HostDefinition[] hosts)
    {
        var site = new SiteDefinition
        {
            Id = id,
            Name = name,
        };

        foreach (var host in hosts)
        {
            site.Hosts.Add(host);
        }

        return site;
    }

    private static HostDefinition CreateHost(string name, HostDefinitionType type, CultureInfo language = null)
    {
        // HostDefinition.Url is a computed property derived from Name + UseSecureConnection;
        // supplying a concrete Name and UseSecureConnection = true produces a non-null Url,
        // which is what the filter in SiteDefinitionExtensions.ToHostSummaries relies on.
        return new HostDefinition
        {
            Name = name,
            Type = type,
            UseSecureConnection = true,
            Language = language,
        };
    }

    private static List<DeserializedSite> DeserializeSitesResponse(IActionResult actionResult)
    {
        var content = actionResult as ContentResult;
        Assert.That(content, Is.Not.Null, "Expected a ContentResult from the controller action.");
        Assert.That(content!.StatusCode, Is.EqualTo(200));

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        return JsonSerializer.Deserialize<List<DeserializedSite>>(content.Content!, options)!;
    }

    // Local DTOs used only to deserialise the controller response for assertion purposes.
    private sealed class DeserializedSite
    {
        public Guid SiteId { get; set; }

        public string SiteName { get; set; }

        public List<DeserializedHost> AvailableHosts { get; set; } = [];

        public bool HasMultipleHosts { get; set; }
    }

    private sealed class DeserializedHost
    {
        public string DisplayName { get; set; }

        public string HostName { get; set; }

        public string HostType { get; set; }

        public string HostLanguage { get; set; }
    }
}
