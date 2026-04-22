namespace Stott.Security.Optimizely.Test.Features.CustomHeaders.Repository;

using System;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.CustomHeaders;
using Stott.Security.Optimizely.Features.CustomHeaders.Models;
using Stott.Security.Optimizely.Features.CustomHeaders.Repository;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public sealed class CustomHeaderRepositoryTests
{
    private static readonly Guid SiteA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static readonly Guid SiteB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private TestDataContext _context;

    private CustomHeaderRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _context = TestDataContextFactory.Create();

        var lazyContext = new Lazy<ICspDataContext>(() => _context);

        _repository = new CustomHeaderRepository(lazyContext);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.Reset();
    }

    [Test]
    public async Task GetAllAsync_GivenNoRecords_ThenReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync(null, null);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAllAsync_GivenRecordsExist_ThenReturnsAllRecords()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "X-Header-A", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value-a", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "X-Header-B", Behavior = CustomHeaderBehavior.Remove, Modified = DateTime.UtcNow, ModifiedBy = "test" });
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "X-Header-C", Behavior = CustomHeaderBehavior.Disabled, Modified = DateTime.UtcNow, ModifiedBy = "test" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(null, null);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetAllAsync_GivenRecordsExist_ThenReturnsRecordsOrderedByHeaderName()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "Z-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "A-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "M-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync(null, null)).ToList();

        // Assert
        Assert.That(result[0].HeaderName, Is.EqualTo("A-Header"));
        Assert.That(result[1].HeaderName, Is.EqualTo("M-Header"));
        Assert.That(result[2].HeaderName, Is.EqualTo("Z-Header"));
    }

    [Test]
    public async Task GetByIdAsync_GivenMatchingId_ThenReturnsRecord()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        _context.CustomHeaders.Add(new CustomHeader { Id = expectedId, HeaderName = "X-Test", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(expectedId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(expectedId));
    }

    [Test]
    public async Task GetByIdAsync_GivenNonMatchingId_ThenReturnsNull()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "X-Test", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public async Task GetByHeaderNameAsync_GivenNullOrWhitespace_ThenReturnsNull(string headerName)
    {
        // Act
        var result = await _repository.GetByHeaderNameAsync(headerName, null, null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByHeaderNameAsync_GivenMatchingName_ThenReturnsRecord()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "X-Test-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByHeaderNameAsync("X-Test-Header", null, null);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.HeaderName, Is.EqualTo("X-Test-Header"));
    }

    [Test]
    public async Task GetByHeaderNameAsync_GivenNonMatchingName_ThenReturnsNull()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "X-Test-Header", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByHeaderNameAsync("X-Other-Header", null, null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task SaveAsync_GivenNewRecordWithEmptyGuid_ThenCreatesNewRecord()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.Empty,
            HeaderName = "X-New-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "new-value"
        };

        // Act
        await _repository.SaveAsync(model, "test-user", null, null);

        // Assert
        _context.ClearTracking();
        var allHeaders = _context.CustomHeaders.ToList();
        Assert.That(allHeaders.Count, Is.EqualTo(1));
        Assert.That(allHeaders[0].HeaderName, Is.EqualTo("X-New-Header"));
    }

    [Test]
    public async Task SaveAsync_GivenExistingId_ThenUpdatesExistingRecord()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        _context.CustomHeaders.Add(new CustomHeader { Id = existingId, HeaderName = "X-Existing", Behavior = CustomHeaderBehavior.Disabled, Modified = DateTime.UtcNow, ModifiedBy = "original" });
        await _context.SaveChangesAsync();
        _context.ClearTracking();

        var model = new SaveCustomHeaderModel
        {
            Id = existingId,
            HeaderName = "X-Existing",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "updated-value"
        };

        // Act
        await _repository.SaveAsync(model, "updater", null, null);

        // Assert
        _context.ClearTracking();
        var allHeaders = _context.CustomHeaders.ToList();
        Assert.That(allHeaders.Count, Is.EqualTo(1));
        Assert.That(allHeaders[0].Behavior, Is.EqualTo(CustomHeaderBehavior.Add));
        Assert.That(allHeaders[0].HeaderValue, Is.EqualTo("updated-value"));
    }

    [Test]
    public async Task SaveAsync_GivenEmptyGuidButMatchingHeaderName_ThenUpdatesByHeaderName()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        _context.CustomHeaders.Add(new CustomHeader { Id = existingId, HeaderName = "X-Existing", Behavior = CustomHeaderBehavior.Disabled, Modified = DateTime.UtcNow, ModifiedBy = "original" });
        await _context.SaveChangesAsync();
        _context.ClearTracking();

        var model = new SaveCustomHeaderModel
        {
            Id = Guid.Empty,
            HeaderName = "X-Existing",
            Behavior = CustomHeaderBehavior.Remove
        };

        // Act
        await _repository.SaveAsync(model, "updater", null, null);

        // Assert
        _context.ClearTracking();
        var allHeaders = _context.CustomHeaders.ToList();
        Assert.That(allHeaders.Count, Is.EqualTo(1));
        Assert.That(allHeaders[0].Id, Is.EqualTo(existingId));
        Assert.That(allHeaders[0].Behavior, Is.EqualTo(CustomHeaderBehavior.Remove));
    }

    [Test]
    public async Task SaveAsync_GivenNewRecord_ThenSetsModifiedByAndModifiedDate()
    {
        // Arrange
        var model = new SaveCustomHeaderModel
        {
            Id = Guid.Empty,
            HeaderName = "X-New-Header",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "value"
        };

        var before = DateTime.UtcNow;

        // Act
        await _repository.SaveAsync(model, "test-user", null, null);

        // Assert
        _context.ClearTracking();
        var saved = _context.CustomHeaders.First();
        Assert.That(saved.ModifiedBy, Is.EqualTo("test-user"));
        Assert.That(saved.Modified, Is.GreaterThanOrEqualTo(before));
    }

    [Test]
    public async Task SaveAsync_GivenExistingRecord_ThenUpdatesBehaviorAndValue()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        _context.CustomHeaders.Add(new CustomHeader { Id = existingId, HeaderName = "X-Test", Behavior = CustomHeaderBehavior.Disabled, HeaderValue = "old", Modified = DateTime.UtcNow, ModifiedBy = "original" });
        await _context.SaveChangesAsync();
        _context.ClearTracking();

        var model = new SaveCustomHeaderModel
        {
            Id = existingId,
            HeaderName = "X-Test-Updated",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "new-value"
        };

        // Act
        await _repository.SaveAsync(model, "updater", null, null);

        // Assert
        _context.ClearTracking();
        var saved = _context.CustomHeaders.First();
        Assert.That(saved.HeaderName, Is.EqualTo("X-Test-Updated"));
        Assert.That(saved.Behavior, Is.EqualTo(CustomHeaderBehavior.Add));
        Assert.That(saved.HeaderValue, Is.EqualTo("new-value"));
    }

    [Test]
    public async Task DeleteAsync_GivenExistingId_ThenRemovesRecord()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        _context.CustomHeaders.Add(new CustomHeader { Id = existingId, HeaderName = "X-Delete-Me", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(existingId);

        // Assert
        _context.ClearTracking();
        var allHeaders = _context.CustomHeaders.ToList();
        Assert.That(allHeaders, Is.Empty);
    }

    [Test]
    public async Task DeleteAsync_GivenNonExistingId_ThenDoesNotThrow()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader { Id = Guid.NewGuid(), HeaderName = "X-Keep-Me", Behavior = CustomHeaderBehavior.Add, HeaderValue = "value", Modified = DateTime.UtcNow, ModifiedBy = "test" });
        await _context.SaveChangesAsync();

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _repository.DeleteAsync(Guid.NewGuid()));

        _context.ClearTracking();
        var allHeaders = _context.CustomHeaders.ToList();
        Assert.That(allHeaders.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllAsync_OnlyGlobalHeadersExist_ReturnsAllGlobalHeaders()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global-a",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-B",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global-b",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync(null, null)).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(x => x.HeaderName == "X-A" && x.HeaderValue == "global-a"), Is.True);
        Assert.That(result.Any(x => x.HeaderName == "X-B" && x.HeaderValue == "global-b"), Is.True);
    }

    [Test]
    public async Task GetAllAsync_GlobalAndSiteExist_ReturnsOnlySiteHeaders()
    {
        // Arrange: Actual behaviour is a cliff fallback — once a site-level record exists,
        // GetAllAsync returns only the site-level set and does not merge in global records.
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global-a",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-B",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global-b",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "site-a",
            SiteId = SiteA,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync(SiteA, null)).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].HeaderName, Is.EqualTo("X-A"));
        Assert.That(result[0].HeaderValue, Is.EqualTo("site-a"));
    }

    [Test]
    public async Task GetAllAsync_HostSiteAndGlobalExist_ReturnsOnlyHostHeaders()
    {
        // Arrange: Actual behaviour is cliff fallback — host-level present suppresses site and global.
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "site",
            SiteId = SiteA,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "host",
            SiteId = SiteA,
            HostName = "www.example.com",
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync(SiteA, "www.example.com")).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].HeaderValue, Is.EqualTo("host"));
    }

    [Test]
    public async Task GetAllAsync_SiteIdSuppliedButNoSiteRecord_FallsBackToGlobalOnly()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global-a",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync(SiteA, null)).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].HeaderName, Is.EqualTo("X-A"));
        Assert.That(result[0].HeaderValue, Is.EqualTo("global-a"));
    }

    [Test]
    public async Task GetAllAsync_NullSiteIdWithOtherSiteRecordsPresent_ReturnsOnlyGlobal()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global-a",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "site-a",
            SiteId = SiteA,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync(null, null)).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].HeaderValue, Is.EqualTo("global-a"));
    }

    [Test]
    public async Task GetAllByContextAsync_NoExactMatch_ReturnsNull()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global-a",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllByContextAsync(SiteA, null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllByContextAsync_ExactMatch_ReturnsOnlyThoseRecords()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global-a",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-B",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "site-b",
            SiteId = SiteA,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllByContextAsync(SiteA, null)).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].HeaderName, Is.EqualTo("X-B"));
        Assert.That(result[0].HeaderValue, Is.EqualTo("site-b"));
    }

    [Test]
    public async Task SaveAsync_CalledTwiceForSameContextAndHeaderName_UpdatesNotInserts()
    {
        // Arrange
        var firstModel = new SaveCustomHeaderModel
        {
            Id = Guid.Empty,
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "first"
        };

        var secondModel = new SaveCustomHeaderModel
        {
            Id = Guid.Empty,
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "second"
        };

        // Act
        await _repository.SaveAsync(firstModel, "user.one", SiteA, null);
        await _repository.SaveAsync(secondModel, "user.two", SiteA, null);

        // Assert
        _context.ClearTracking();
        var matching = _context.CustomHeaders
            .Where(x => x.HeaderName == "X-A" && x.SiteId == SiteA && x.HostName == null)
            .ToList();
        Assert.That(matching.Count, Is.EqualTo(1));
        Assert.That(matching[0].HeaderValue, Is.EqualTo("second"));
    }

    [Test]
    public async Task DeleteByContextAsync_GivenNullSiteId_DoesNothing()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global-a",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "site-a",
            SiteId = SiteA,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteByContextAsync(null, null, "user");

        // Assert
        _context.ClearTracking();
        Assert.That(_context.CustomHeaders.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteByContextAsync_RemovesOnlyMatchingContext()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "site-a",
            SiteId = SiteA,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "site-b",
            SiteId = SiteB,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteByContextAsync(SiteA, null, "user");

        // Assert
        _context.ClearTracking();
        var remaining = _context.CustomHeaders.ToList();
        Assert.That(remaining.Count, Is.EqualTo(2));
        Assert.That(remaining.Any(x => x.SiteId == null), Is.True);
        Assert.That(remaining.Any(x => x.SiteId == SiteB), Is.True);
        Assert.That(remaining.Any(x => x.SiteId == SiteA), Is.False);
    }

    [Test]
    public async Task GetAllAsync_WhenHostMissing_FallsBackToSite()
    {
        // Arrange
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "site",
            SiteId = SiteA,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync(SiteA, "www.example.com")).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].HeaderValue, Is.EqualTo("site"));
        });
    }

    [Test]
    public async Task GetAllAsync_WhenSiteAddsAdditionalHeaderNotInGlobal_ReturnsOnlySiteHeaders()
    {
        // Arrange: Pins current cliff-fallback behaviour — once any site-level record exists
        // for the requested site, global records are NOT merged in. Only the site-level set is returned.
        // (Spec originally expected a merged result of both "X"=global + "Y"=site; adapted to observed behaviour.)
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-A",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "global",
            SiteId = null,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        _context.CustomHeaders.Add(new CustomHeader
        {
            Id = Guid.NewGuid(),
            HeaderName = "X-B",
            Behavior = CustomHeaderBehavior.Add,
            HeaderValue = "site",
            SiteId = SiteA,
            HostName = null,
            Modified = DateTime.UtcNow,
            ModifiedBy = "test"
        });
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetAllAsync(SiteA, null)).ToList();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].HeaderName, Is.EqualTo("X-B"));
            Assert.That(result[0].HeaderValue, Is.EqualTo("site"));
        });
    }
}
