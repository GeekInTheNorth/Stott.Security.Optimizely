using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using EPiServer.Data;

using Microsoft.EntityFrameworkCore;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Entities.Exceptions;
using Stott.Optimizely.Csp.Features.Permissions.Repository;
using Stott.Optimizely.Csp.Test.TestCases;

namespace Stott.Optimizely.Csp.Test.Features.Permissions.Repository
{
    [TestFixture]
    public class CspPermissionRepositoryTests
    {
        private Mock<ICspDataContext> _mockContext;

        private Mock<DbSet<CspSource>> _mockDbSet;

        private CspPermissionRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<ICspDataContext>();
            _mockDbSet = DbSetMocker.GetQueryableMockDbSet<CspSource>();
            _mockContext.Setup(x => x.CspSources).Returns(_mockDbSet.Object);

            _repository = new CspPermissionRepository(_mockContext.Object);
        }

        [Test]
        [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
        public void SaveAsync_GivenAnEmptyOrNullSource_ThenAnArgumentNullExceptionShouldBeThrown(string source)
        {
            // Arrange
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveAsync(Guid.Empty, source, directives));
        }

        [Test]
        [TestCaseSource(typeof(CspPermissionRepositoryTestCases), nameof(CspPermissionRepositoryTestCases.InvalidDirectivesTestCases))]
        public void SaveAsync_GivenAnEmptyOrNullDirectives_ThenAnArgumentExceptionShouldBeThrown(List<string> directives)
        {
            // Arrange
            var source = CspConstants.Sources.Self;

            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => _repository.SaveAsync(Guid.Empty, source, directives));
        }

        [Test]
        public void SaveAsync_GivenSourceExistsAgainstAnotherEntry_ThenAnEntityExistsExceptionShouldBeThrown()
        {
            // Arrange
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            var existingCspSource = new CspSource { Id = Guid.NewGuid(), Source = source };
            _mockDbSet = DbSetMocker.GetQueryableMockDbSet(existingCspSource);
            _mockContext.Setup(x => x.CspSources).Returns(_mockDbSet.Object);

            // Assert
            Assert.ThrowsAsync<EntityExistsException>(() => _repository.SaveAsync(Guid.Empty, source, directives));
        }

        [Test]
        public void SaveAsync_GivenSourceDoesNotExistForAnotherEntry_ThenAnEntityExistsExceptionShouldNotBeThrown()
        {
            // Arrange
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            // Assert
            Assert.DoesNotThrowAsync(() => _repository.SaveAsync(Guid.Empty, source, directives));
        }

        [Test]
        public async Task SaveAsync_GivenAValidCspSourceForANewSource_ThenANewRecordShouldBeSaved()
        {
            // Arrange
            var id = Guid.Empty;
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            CspSource savedSource = null;
            _mockDbSet.Setup(x => x.Add(It.IsAny<CspSource>())).Callback<CspSource>(x => savedSource = x);

            // Arrange
            await _repository.SaveAsync(id, source, directives);

            // Assert
            _mockDbSet.Verify(x => x.Add(It.IsAny<CspSource>()), Times.Once);
            Assert.That(savedSource, Is.Not.Null);
            Assert.That(savedSource.Source, Is.EqualTo(source));
            Assert.That(savedSource.Directives, Is.EqualTo(CspConstants.Directives.DefaultSource));
        }

        [Test]
        public async Task SaveAsync_GivenAValidCspSourceForAnExistingSource_ThenTheExistingRecordShouldBeUpdates()
        {
            // Arrange
            var id = Guid.NewGuid();
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };
        
            var existingSource = new CspSource { Id = id, Source = source, Directives = CspConstants.Directives.FrameSource };
            _mockDbSet = DbSetMocker.GetQueryableMockDbSet(existingSource);
            _mockContext.Setup(x => x.CspSources).Returns(_mockDbSet.Object);

            // Arrange
            await _repository.SaveAsync(id, source, directives);

            // Assert
            _mockDbSet.Verify(x => x.Add(It.IsAny<CspSource>()), Times.Never);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(existingSource.Directives.Contains(CspConstants.Directives.DefaultSource), Is.True);
            Assert.That(existingSource.Directives.Contains(CspConstants.Directives.FrameSource), Is.False);
        }
        
        [Test]
        public async Task AppendDirectiveAsync_GivenASourceThatIsNotMapped_ThenANewSourceRecordShouldBeCreated()
        {
            // Arrange
            const string source = "https://www.example.com";
            const string directive = CspConstants.Directives.DefaultSource;

            CspSource savedSource = null;
            _mockDbSet.Setup(x => x.Add(It.IsAny<CspSource>())).Callback<CspSource>(x => savedSource = x);

            // Act
            await _repository.AppendDirectiveAsync(source, directive);

            // Assert
            _mockDbSet.Verify(x => x.Add(It.IsAny<CspSource>()), Times.Once);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(savedSource, Is.Not.Null);
            Assert.That(savedSource.Source, Is.EqualTo(source));
            Assert.That(savedSource.Directives, Is.EqualTo(directive));
        }
        
        [Test]
        public async Task AppendDirectiveAsync_GivenASourceThatIsAlreadyMappedWithoutAMatchingDirective_ThenTheDirectiveShouldBeAppended()
        {
            // Arrange
            var id = Guid.NewGuid();
            var source = "https://www.example.com";
            var directive = CspConstants.Directives.StyleSource;

            var existingSource = new CspSource 
            { 
                Id = id, 
                Source = source,
                Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource}"
            };
            _mockDbSet = DbSetMocker.GetQueryableMockDbSet(existingSource);
            _mockContext.Setup(x => x.CspSources).Returns(_mockDbSet.Object);

            // Act
            await _repository.AppendDirectiveAsync(source, directive);

            // Assert
            _mockDbSet.Verify(x => x.Add(It.IsAny<CspSource>()), Times.Never);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(existingSource.Directives.Contains(CspConstants.Directives.DefaultSource), Is.True);
            Assert.That(existingSource.Directives.Contains(CspConstants.Directives.ScriptSource), Is.True);
            Assert.That(existingSource.Directives.Contains(CspConstants.Directives.StyleSource), Is.True);
        }
        
        [Test]
        public async Task AppendDirectiveAsync_GivenASourceThatIsAlreadyMappedWithAMatchingDirective_ThenTheDirectiveShouldNotBeAppended()
        {
            // Arrange
            var id = Guid.NewGuid();
            var source = "https://www.example.com";
            var directive = CspConstants.Directives.DefaultSource;
            var originalDirectives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource}";

            var existingSource = new CspSource
            {
                Id = id,
                Source = source,
                Directives = originalDirectives
            };
            _mockDbSet = DbSetMocker.GetQueryableMockDbSet(existingSource);
            _mockContext.Setup(x => x.CspSources).Returns(_mockDbSet.Object);

            // Act
            await _repository.AppendDirectiveAsync(source, directive);

            // Assert
            _mockDbSet.Verify(x => x.Add(It.IsAny<CspSource>()), Times.Never);
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.That(existingSource.Source, Is.EqualTo(existingSource.Source));
            Assert.That(existingSource.Directives, Is.EqualTo(originalDirectives));
        }
    }
}
