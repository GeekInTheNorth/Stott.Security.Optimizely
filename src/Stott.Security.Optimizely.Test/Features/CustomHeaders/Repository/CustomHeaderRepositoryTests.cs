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
        var result = await _repository.GetAllAsync();

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
        var result = await _repository.GetAllAsync();

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
        var result = (await _repository.GetAllAsync()).ToList();

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
        var result = await _repository.GetByHeaderNameAsync(headerName);

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
        var result = await _repository.GetByHeaderNameAsync("X-Test-Header");

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
        var result = await _repository.GetByHeaderNameAsync("X-Other-Header");

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
        await _repository.SaveAsync(model, "test-user");

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
        await _repository.SaveAsync(model, "updater");

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
        await _repository.SaveAsync(model, "updater");

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
        await _repository.SaveAsync(model, "test-user");

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
        await _repository.SaveAsync(model, "updater");

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
}
