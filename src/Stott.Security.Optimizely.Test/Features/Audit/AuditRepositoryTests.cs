namespace Stott.Security.Optimizely.Test.Features.Audit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Audit;

[TestFixture]
public class AuditRepositoryTests
{
    private TestDataContext _inMemoryDatabase;

    private Lazy<ICspDataContext> _lazyInMemoryDatabase;

    private AuditRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _lazyInMemoryDatabase = new Lazy<ICspDataContext>(() => _inMemoryDatabase);

        _repository = new AuditRepository(_lazyInMemoryDatabase);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _inMemoryDatabase.Reset();
    }

    [Test]
    public async Task GetAsync_ReturnsAllRecordsWhenNoFiltersAreApplied()
    {
        // Arrange
        var header1 = new AuditHeader { Id = Guid.NewGuid(), Actioned = DateTime.Today, ActionedBy = "UserA", RecordType = "TypeA", OperationType = "OpA", Identifier = "IdA" };
        var header2 = new AuditHeader { Id = Guid.NewGuid(), Actioned = DateTime.Today, ActionedBy = "UserB", RecordType = "TypeB", OperationType = "OpB", Identifier = "IdB" };
        _inMemoryDatabase.AuditHeaders.AddRange(header1, header2);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), null, null, null, 0, 10, null);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    [TestCase("Search", 1)]
    [TestCase("SEARCH", 1)]
    [TestCase("  Search  ", 1)]
    [TestCase("None", 0)]
    public async Task GetAsync_FiltersByIdentifier(string searchTerm, int expectedCount)
    {
        // Arrange
        var header1 = new AuditHeader { Id = Guid.NewGuid(), Actioned = DateTime.Today, ActionedBy = "UserA", RecordType = "TypeA", OperationType = "OpA", Identifier = "SearchableId" };
        var header2 = new AuditHeader { Id = Guid.NewGuid(), Actioned = DateTime.Today, ActionedBy = "UserB", RecordType = "TypeB", OperationType = "OpB", Identifier = "OtherId" };
        _inMemoryDatabase.AuditHeaders.AddRange(header1, header2);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), null, null, null, 0, 10, searchTerm);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(expectedCount));
    }

    [Test]
    [TestCase("Old", 1)]
    [TestCase("New", 1)]
    [TestCase("Value", 2)]
    public async Task GetAsync_FiltersByAuditProperties(string searchTerm, int expectedCount)
    {
        // Arrange
        var header1 = new AuditHeader { Id = Guid.NewGuid(), Actioned = DateTime.Today, ActionedBy = "UserA", RecordType = "TypeA", OperationType = "OpA", Identifier = "IdA" };
        var prop1 = new AuditProperty { Id = Guid.NewGuid(), Header = header1, Field = "Field1", OldValue = "OldValue", NewValue = "SomethingElse" };
        
        var header2 = new AuditHeader { Id = Guid.NewGuid(), Actioned = DateTime.Today, ActionedBy = "UserB", RecordType = "TypeB", OperationType = "OpB", Identifier = "IdB" };
        var prop2 = new AuditProperty { Id = Guid.NewGuid(), Header = header2, Field = "Field2", OldValue = "SomethingElse", NewValue = "NewValue" };

        _inMemoryDatabase.AuditHeaders.AddRange(header1, header2);
        _inMemoryDatabase.AuditProperties.AddRange(prop1, prop2);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), null, null, null, 0, 10, searchTerm);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task GetAsync_HandlesNullValuesInDatabaseWithoutThrowingExceptions()
    {
        // Arrange
        var header1 = new AuditHeader { Id = Guid.NewGuid(), Actioned = DateTime.Today, ActionedBy = "UserA", RecordType = "TypeA", OperationType = "OpA", Identifier = null };
        var prop1 = new AuditProperty { Id = Guid.NewGuid(), Header = header1, Field = "Field1", OldValue = null, NewValue = null };
        
        _inMemoryDatabase.AuditHeaders.Add(header1);
        _inMemoryDatabase.AuditProperties.Add(prop1);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act & Assert
        Assert.DoesNotThrowAsync(async () => await _repository.GetAsync(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), null, null, null, 0, 10, "Search"));
    }

    [Test]
    public async Task GetAsync_FiltersCorrectlyWhenCombinedWithOtherFilters()
    {
        // Arrange
        var header1 = new AuditHeader { Id = Guid.NewGuid(), Actioned = DateTime.Today, ActionedBy = "TargetUser", RecordType = "TypeA", OperationType = "OpA", Identifier = "Match" };
        var header2 = new AuditHeader { Id = Guid.NewGuid(), Actioned = DateTime.Today, ActionedBy = "OtherUser", RecordType = "TypeA", OperationType = "OpA", Identifier = "Match" };
        
        _inMemoryDatabase.AuditHeaders.AddRange(header1, header2);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1), "TargetUser", null, null, 0, 10, "Match");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().ActionedBy, Is.EqualTo("TargetUser"));
    }
}
