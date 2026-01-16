namespace Stott.Security.Optimizely.Test.Features.Csp.Permissions.Repository;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Entities.Exceptions;
using Stott.Security.Optimizely.Features.Csp.Permissions.Repository;
using Stott.Security.Optimizely.Test.TestCases;

[TestFixture]
public sealed class CspPermissionRepositoryTests
{
    private TestDataContext _inMemoryDatabase;

    private Lazy<ICspDataContext> _lazyInMemoryDatabase;

    private CspPermissionRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _lazyInMemoryDatabase = new Lazy<ICspDataContext>(() => _inMemoryDatabase);

        _repository = new CspPermissionRepository(_lazyInMemoryDatabase);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _inMemoryDatabase.Reset();
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void SaveAsync_GivenAnEmptyOrNullSource_ThenAnArgumentNullExceptionShouldBeThrown(string source)
    {
        // Arrange
        var user = "test.user";
        var directives = new List<string> { CspConstants.Directives.DefaultSource };

        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveAsync(Guid.Empty, source, directives, user));
    }

    [Test]
    [TestCaseSource(typeof(CspPermissionRepositoryTestCases), nameof(CspPermissionRepositoryTestCases.InvalidDirectivesTestCases))]
    public void SaveAsync_GivenAnEmptyOrNullDirectives_ThenAnArgumentExceptionShouldBeThrown(List<string> directives)
    {
        // Arrange
        var user = "test.user";
        var source = CspConstants.Sources.Self;

        // Assert
        Assert.ThrowsAsync<ArgumentException>(() => _repository.SaveAsync(Guid.Empty, source, directives, user));
    }

    [Test]
    public void SaveAsync_GivenSourceExistsAgainstAnotherEntry_ThenAnEntityExistsExceptionShouldBeThrown()
    {
        // Arrange
        var user = "test.user";
        var source = CspConstants.Sources.Self;
        var directives = new List<string> { CspConstants.Directives.DefaultSource };

        var existingCspSource = new CspSource { Id = Guid.NewGuid(), Source = source };
        _inMemoryDatabase.CspSources.Add(existingCspSource);
        _inMemoryDatabase.SaveChanges();

        // Assert
        Assert.ThrowsAsync<EntityExistsException>(() => _repository.SaveAsync(Guid.Empty, source, directives, user));
    }

    [Test]
    public void SaveAsync_GivenSourceDoesNotExistForAnotherEntry_ThenAnEntityExistsExceptionShouldNotBeThrown()
    {
        // Arrange
        var user = "test.user";
        var source = CspConstants.Sources.Self;
        var directives = new List<string> { CspConstants.Directives.DefaultSource };

        // Assert
        Assert.DoesNotThrowAsync(() => _repository.SaveAsync(Guid.Empty, source, directives, user));
    }

    [Test]
    public async Task SaveAsync_GivenAValidCspSourceForANewSource_ThenANewRecordShouldBeSaved()
    {
        // Arrange
        var id = Guid.Empty;
        var user = "test.user";
        var source = CspConstants.Sources.Self;
        var directives = new List<string> { CspConstants.Directives.DefaultSource };

        // Arrange
        await _repository.SaveAsync(id, source, directives, user);

        // Assert
        var lastEntry = await _inMemoryDatabase.CspSources.AsQueryable().LastOrDefaultAsync();
        Assert.That(lastEntry, Is.Not.Null);
        Assert.That(lastEntry.Source, Is.EqualTo(source));
        Assert.That(lastEntry.Directives, Is.EqualTo(CspConstants.Directives.DefaultSource));
    }

    [Test]
    public async Task SaveAsync_GivenAValidCspSourceForAnExistingSource_ThenTheExistingRecordShouldBeUpdated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = "test.user";
        var source = CspConstants.Sources.Self;
        var directives = new List<string> { CspConstants.Directives.DefaultSource };

        var existingSource = new CspSource { Id = id, Source = source, Directives = CspConstants.Directives.FrameSource };
        _inMemoryDatabase.CspSources.Add(existingSource);
        _inMemoryDatabase.SaveChanges();

        // Arrange
        await _repository.SaveAsync(id, source, directives, user);

        // Assert
        var updatedRecord = await _inMemoryDatabase.CspSources.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
        Assert.That(updatedRecord, Is.Not.Null);
        Assert.That(updatedRecord.Directives.Contains(CspConstants.Directives.DefaultSource), Is.True);
        Assert.That(updatedRecord.Directives.Contains(CspConstants.Directives.FrameSource), Is.False);
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void AppendDirectiveAsync_GivenAnEmptyOrNullSource_ThrowsAnArgumentNullException(string source)
    {
        // Arrange
        const string user = "test.user";
        const string directive = CspConstants.Directives.DefaultSource;

        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AppendDirectiveAsync(source, directive, user));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void AppendDirectiveAsync_GivenAnEmptyOrNullDirective_ThrowsAnArgumentNullException(string directive)
    {
        // Arrange
        const string user = "test.user";
        const string source = "https://www.example.com";

        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AppendDirectiveAsync(source, directive, user));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void AppendDirectiveAsync_GivenAnEmptyOrNullModifiedBy_ThrowsAnArgumentNullException(string user)
    {
        // Arrange
        const string source = "https://www.example.com";
        const string directive = CspConstants.Directives.DefaultSource;

        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AppendDirectiveAsync(source, directive, user));
    }

    [Test]
    public async Task AppendDirectiveAsync_GivenASourceThatIsNotMapped_ThenANewSourceRecordShouldBeCreated()
    {
        // Arrange
        const string user = "test.user";
        const string source = "https://www.example.com";
        const string directive = CspConstants.Directives.DefaultSource;

        // Act
        var originalCount = await _inMemoryDatabase.CspSources.AsQueryable().CountAsync();

        await _repository.AppendDirectiveAsync(source, directive, user);

        var updatedCount = await _inMemoryDatabase.CspSources.AsQueryable().CountAsync();
        var createdRecord = await _inMemoryDatabase.CspSources.AsQueryable().FirstOrDefaultAsync();

        // Assert
        Assert.That(originalCount, Is.EqualTo(0));
        Assert.That(updatedCount, Is.EqualTo(1));
        Assert.That(createdRecord.Source, Is.EqualTo(source));
        Assert.That(createdRecord.Directives, Is.EqualTo(directive));
    }

    [Test]
    public async Task AppendDirectiveAsync_GivenASourceThatIsAlreadyMappedWithoutAMatchingDirective_ThenTheDirectiveShouldBeAppended()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = "test.user";
        var source = "https://www.example.com";
        var directive = CspConstants.Directives.StyleSource;

        var existingSource = new CspSource
        {
            Id = id,
            Source = source,
            Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource}"
        };
        _inMemoryDatabase.CspSources.Add(existingSource);
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.AppendDirectiveAsync(source, directive, user);

        // Assert
        var updatedRecord = await _inMemoryDatabase.CspSources.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
        Assert.That(updatedRecord, Is.Not.Null);
        Assert.That(updatedRecord.Directives.Contains(CspConstants.Directives.DefaultSource), Is.True);
        Assert.That(updatedRecord.Directives.Contains(CspConstants.Directives.ScriptSource), Is.True);
        Assert.That(updatedRecord.Directives.Contains(CspConstants.Directives.StyleSource), Is.True);
    }

    [Test]
    public async Task AppendDirectiveAsync_GivenASourceThatIsAlreadyMappedWithAMatchingDirective_ThenTheDirectiveShouldNotBeAppended()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = "test.user";
        var source = "https://www.example.com";
        var directive = CspConstants.Directives.DefaultSource;
        var originalDirectives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource}";

        var existingSource = new CspSource
        {
            Id = id,
            Source = source,
            Directives = originalDirectives
        };
        _inMemoryDatabase.CspSources.Add(existingSource);
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.AppendDirectiveAsync(source, directive, user);

        // Assert
        var updatedRecord = await _inMemoryDatabase.CspSources.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
        Assert.That(updatedRecord, Is.Not.Null);
        Assert.That(updatedRecord.Source, Is.EqualTo(existingSource.Source));
        Assert.That(updatedRecord.Directives, Is.EqualTo(originalDirectives));
    }

    [Test]
    [TestCaseSource(typeof(CspPermissionRepositoryTestCases), nameof(CspPermissionRepositoryTestCases.AppendHandlesSimilarDirectivesTestCases))]
    public async Task AppendDirectiveAsync_GivenASourceThatIsAlreadyMappedWithAMoreFineTunedDirective_ThenTheDirectiveShouldBeAppended(
        string existingDirectives,
        string directiveToAdd,
        string updatedDirectives)
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = "test.user";
        var source = "https://www.example.com";

        var existingSource = new CspSource
        {
            Id = id,
            Source = source,
            Directives = existingDirectives
        };
        _inMemoryDatabase.CspSources.Add(existingSource);
        _inMemoryDatabase.SaveChanges();

        // Act
        await _repository.AppendDirectiveAsync(source, directiveToAdd, user);

        // Assert
        var updatedRecord = await _inMemoryDatabase.CspSources.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
        Assert.That(updatedRecord, Is.Not.Null);
        Assert.That(updatedRecord.Source, Is.EqualTo(existingSource.Source));
        Assert.That(updatedRecord.Directives, Is.EqualTo(updatedDirectives));
    }

    [Test]
    public void DeleteAsync_GivenAnEmptyId_ThrowsAnArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() => _repository.DeleteAsync(Guid.Empty, "test.user"));
    }

    [Test]
    [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
    public void DeleteAsync_GivenAnEmptyOrNullDeletedBy_ThrowsAnArgumentNullException(string user)
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _repository.DeleteAsync(Guid.NewGuid(), user));
    }
}