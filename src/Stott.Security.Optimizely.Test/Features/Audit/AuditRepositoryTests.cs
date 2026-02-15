namespace Stott.Security.Optimizely.Test.Features.Audit;

using System;
using System.Linq;
using System.Threading.Tasks;

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
    public async Task DeleteAsync_GivenNoRecordsOlderThanThreshold_ThenReturnsZero()
    {
        // Arrange
        var threshold = DateTime.Today.AddDays(-30);
        var recentDate = DateTime.Today.AddDays(-10);

        _inMemoryDatabase.AuditHeaders.Add(new AuditHeader
        {
            RecordType = "CspSettings",
            OperationType = "Update",
            Actioned = recentDate,
            ActionedBy = "testuser",
            Identifier = "1"
        });
        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(threshold, 1000);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteAsync_GivenRecordsOlderThanThreshold_ThenDeletesThoseRecords()
    {
        // Arrange
        var threshold = DateTime.Today.AddDays(-30);
        var oldDate1 = DateTime.Today.AddDays(-60);
        var oldDate2 = DateTime.Today.AddDays(-45);
        var recentDate = DateTime.Today.AddDays(-10);

        _inMemoryDatabase.AuditHeaders.Add(new AuditHeader
        {
            RecordType = "CspSettings",
            OperationType = "Update",
            Actioned = oldDate1,
            ActionedBy = "testuser1",
            Identifier = "1"
        });

        _inMemoryDatabase.AuditHeaders.Add(new AuditHeader
        {
            RecordType = "CspSettings",
            OperationType = "Update",
            Actioned = oldDate2,
            ActionedBy = "testuser2",
            Identifier = "2"
        });

        _inMemoryDatabase.AuditHeaders.Add(new AuditHeader
        {
            RecordType = "CspSettings",
            OperationType = "Update",
            Actioned = recentDate,
            ActionedBy = "testuser3",
            Identifier = "3"
        });

        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(threshold, 1000);

        var remainingRecords = await _inMemoryDatabase.AuditHeaders.AsQueryable().ToListAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(2));
            Assert.That(remainingRecords, Has.Count.EqualTo(1));
            Assert.That(remainingRecords[0].ActionedBy, Is.EqualTo("testuser3"));
        });
    }

    [Test]
    public async Task DeleteAsync_GivenRecordsWithProperties_ThenDeletesCascades()
    {
        // Arrange
        var threshold = DateTime.Today.AddDays(-30);
        var oldDate = DateTime.Today.AddDays(-60);

        var header = new AuditHeader
        {
            RecordType = "CspSettings",
            OperationType = "Update",
            Actioned = oldDate,
            ActionedBy = "testuser",
            Identifier = "1",
            AuditProperties = new List<AuditProperty>
            {
                new() {
                    Field = "IsEnabled",
                    OldValue = "false",
                    NewValue = "true"
                },
                new() {
                    Field = "ReportOnly",
                    OldValue = "true",
                    NewValue = "false"
                }
            }
        };

        _inMemoryDatabase.AuditHeaders.Add(header);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(threshold, 1000);

        var remainingHeaders = await _inMemoryDatabase.AuditHeaders.AsQueryable().ToListAsync();
        var remainingProperties = await _inMemoryDatabase.AuditProperties.AsQueryable().ToListAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(1));
            Assert.That(remainingHeaders, Is.Empty);
            Assert.That(remainingProperties, Is.Empty);
        });
    }

    [Test]
    public async Task DeleteAsync_GivenMultipleRecordsWithMixedDates_ThenOnlyDeletesOldRecords()
    {
        // Arrange
        var threshold = DateTime.Today.AddDays(-365);
        var veryOldDate = DateTime.Today.AddDays(-800);
        var oldDate = DateTime.Today.AddDays(-400);
        var recentDate = DateTime.Today.AddDays(-100);

        _inMemoryDatabase.AuditHeaders.AddRange(
            new AuditHeader { RecordType = "Type1", OperationType = "Create", Actioned = veryOldDate, ActionedBy = "user1", Identifier = "1" },
            new AuditHeader { RecordType = "Type2", OperationType = "Update", Actioned = oldDate, ActionedBy = "user2", Identifier = "2" },
            new AuditHeader { RecordType = "Type3", OperationType = "Delete", Actioned = recentDate, ActionedBy = "user3", Identifier = "3" },
            new AuditHeader { RecordType = "Type4", OperationType = "Create", Actioned = veryOldDate, ActionedBy = "user4", Identifier = "4" }
        );

        await _inMemoryDatabase.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(threshold, 1000);

        var remainingRecords = await _inMemoryDatabase.AuditHeaders.AsQueryable().ToListAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(3));
            Assert.That(remainingRecords, Has.Count.EqualTo(1));
            Assert.That(remainingRecords[0].ActionedBy, Is.EqualTo("user3"));
        });
    }

    [Test]
    public async Task DeleteAsync_GivenBatchSizeLimit_ThenOnlyDeletesUpToBatchSize()
    {
        // Arrange
        var threshold = DateTime.Today.AddDays(-30);
        var oldDate1 = DateTime.Today.AddDays(-60);
        var oldDate2 = DateTime.Today.AddDays(-50);
        var oldDate3 = DateTime.Today.AddDays(-40);

        _inMemoryDatabase.AuditHeaders.AddRange(
            new AuditHeader { RecordType = "Type1", OperationType = "Create", Actioned = oldDate1, ActionedBy = "user1", Identifier = "1" },
            new AuditHeader { RecordType = "Type2", OperationType = "Update", Actioned = oldDate2, ActionedBy = "user2", Identifier = "2" },
            new AuditHeader { RecordType = "Type3", OperationType = "Delete", Actioned = oldDate3, ActionedBy = "user3", Identifier = "3" }
        );

        await _inMemoryDatabase.SaveChangesAsync();

        // Act - Only delete 2 records even though 3 are eligible
        var result = await _repository.DeleteAsync(threshold, 2);

        var remainingRecords = await _inMemoryDatabase.AuditHeaders.AsQueryable().ToListAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(2));
            Assert.That(remainingRecords, Has.Count.EqualTo(1));
            // Should delete oldest records first (ordered by Actioned)
            Assert.That(remainingRecords[0].ActionedBy, Is.EqualTo("user3"));
        });
    }

    [Test]
    public async Task DeleteAsync_GivenRecordsNotDeleted_ThenTheirPropertiesRemainIntact()
    {
        // Arrange
        var threshold = DateTime.Today.AddDays(-30);
        var oldDate1 = DateTime.Today.AddDays(-60);
        var oldDate2 = DateTime.Today.AddDays(-50);
        var recentDate = DateTime.Today.AddDays(-10);

        // Create records with properties - some old, some recent
        var oldHeader1 = new AuditHeader
        {
            RecordType = "Type1",
            OperationType = "Update",
            Actioned = oldDate1,
            ActionedBy = "user1",
            Identifier = "1",
            AuditProperties = new List<AuditProperty>
            {
                new() { Field = "Field1", OldValue = "Old1", NewValue = "New1" },
                new() { Field = "Field2", OldValue = "Old2", NewValue = "New2" }
            }
        };

        var oldHeader2 = new AuditHeader
        {
            RecordType = "Type2",
            OperationType = "Update",
            Actioned = oldDate2,
            ActionedBy = "user2",
            Identifier = "2",
            AuditProperties = new List<AuditProperty>
            {
                new() { Field = "Field3", OldValue = "Old3", NewValue = "New3" }
            }
        };

        var recentHeader = new AuditHeader
        {
            RecordType = "Type3",
            OperationType = "Create",
            Actioned = recentDate,
            ActionedBy = "user3",
            Identifier = "3",
            AuditProperties = new List<AuditProperty>
            {
                new() { Field = "Field4", OldValue = "Old4", NewValue = "New4" },
                new() { Field = "Field5", OldValue = "Old5", NewValue = "New5" },
                new() { Field = "Field6", OldValue = "Old6", NewValue = "New6" }
            }
        };

        _inMemoryDatabase.AuditHeaders.AddRange(oldHeader1, oldHeader2, recentHeader);
        await _inMemoryDatabase.SaveChangesAsync();

        // Act - Delete only 1 old record, leaving oldHeader2 and recentHeader
        var result = await _repository.DeleteAsync(threshold, 1);

        var remainingHeaders = await _inMemoryDatabase.AuditHeaders.Include(x => x.AuditProperties).AsQueryable().ToListAsync();
        var remainingProperties = await _inMemoryDatabase.AuditProperties.AsQueryable().ToListAsync();

        // Assert
        Assert.Multiple(() =>
        {
            // Verify 1 record deleted
            Assert.That(result, Is.EqualTo(1));
            Assert.That(remainingHeaders, Has.Count.EqualTo(2));

            // Verify remaining headers still have their properties
            var remainingOldHeader = remainingHeaders.FirstOrDefault(x => x.ActionedBy == "user2");
            Assert.That(remainingOldHeader, Is.Not.Null);
            Assert.That(remainingOldHeader.AuditProperties, Has.Count.EqualTo(1));
            Assert.That(remainingOldHeader.AuditProperties.First().Field, Is.EqualTo("Field3"));

            var remainingRecentHeader = remainingHeaders.FirstOrDefault(x => x.ActionedBy == "user3");
            Assert.That(remainingRecentHeader, Is.Not.Null);
            Assert.That(remainingRecentHeader.AuditProperties, Has.Count.EqualTo(3));

            // Verify total properties: 1 from oldHeader2 + 3 from recentHeader = 4
            Assert.That(remainingProperties, Has.Count.EqualTo(4));

            // Verify no orphaned properties exist
            var orphanedProperties = remainingProperties.Where(p =>
                !remainingHeaders.Any(h => h.Id == p.AuditHeaderId)).ToList();
            Assert.That(orphanedProperties, Is.Empty);
        });
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
