using System;
using System.Collections.Generic;
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
        private Mock<CspDataContext> _mockContext;

        private Mock<DbSet<CspSource>> _mockDbSet;

        private CspPermissionRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<CspDataContext>();
            _mockDbSet = ContextMocker.GetQueryableMockDbSet<CspSource>();
            _mockContext.Setup(x => x.CspSources).Returns(_mockDbSet.Object);

            _repository = new CspPermissionRepository(_mockContext.Object);
        }

        [Test]
        [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
        public void Save_GivenAnEmptyOrNullSource_ThenAnArgumentNullExceptionShouldBeThrown(string source)
        {
            // Arrange
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _repository.SaveAsync(Guid.Empty, source, directives));
        }

        [Test]
        [TestCaseSource(typeof(CspPermissionRepositoryTestCases), nameof(CspPermissionRepositoryTestCases.InvalidDirectivesTestCases))]
        public void Save_GivenAnEmptyOrNullDirectives_ThenAnArgumentExceptionShouldBeThrown(List<string> directives)
        {
            // Arrange
            var source = CspConstants.Sources.Self;

            // Assert
            Assert.ThrowsAsync<ArgumentException>(() => _repository.SaveAsync(Guid.Empty, source, directives));
        }

        [Test]
        public void Save_GivenSourceExistsAgainstAnotherEntry_ThenAnEntityExistsExceptionShouldBeThrown()
        {
            // Arrange
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            var existingCspSource = new CspSource { Id = Guid.NewGuid(), Source = source };

            // Assert
            Assert.ThrowsAsync<EntityExistsException>(() => _repository.SaveAsync(Guid.Empty, source, directives));
        }

        [Test]
        public void Save_GivenSourceDoesNotExistForAnotherEntry_ThenAnEntityExistsExceptionShouldNotBeThrown()
        {
            // Arrange
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            // Assert
            Assert.DoesNotThrowAsync(() => _repository.SaveAsync(Guid.Empty, source, directives));
        }

        [Test]
        public async Task Save_GivenAValidCspSourceForANewSource_ThenANewRecordShouldBeSaved()
        {
            // Arrange
            var id = Guid.Empty;
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            CspSource savedSource = null;

            // Arrange
            await _repository.SaveAsync(id, source, directives);

            // Assert
            _mockDbSet.Verify(x => x.Add(It.IsAny<CspSource>()), Times.Once);
            Assert.That(savedSource, Is.Not.Null);
            Assert.That(savedSource.Source, Is.EqualTo(source));
            Assert.That(savedSource.Directives, Is.EqualTo(CspConstants.Directives.DefaultSource));
        }

        //[Test]
        //public async Task Save_GivenAValidCspSourceForAnExistingSource_ThenTheExistingRecordShouldBeLoaded()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    var source = CspConstants.Sources.Self;
        //    var directives = new List<string> { CspConstants.Directives.DefaultSource };

        //    var existingSource = new CspSource { Source = source, Directives = CspConstants.Directives.FrameSource };
        //    _mockContext.Setup(x => x.CspSources)
        //                .Returns(ContextMocker.GetQueryableMockDbSet(existingSource).Object);

        //    // Arrange
        //    await _repository.SaveAsync(id, source, directives);

        //    // Assert
        //    _mockDynamicDataStore.Verify(x => x.Load<CspSource>(It.IsAny<Identity>()), Times.Once);
        //}

        //[Test]
        //public async Task Save_GivenAValidCspSourceForAnExistingSource_ThenTheExistingRecordShouldBeSaved()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    var source = CspConstants.Sources.Self;
        //    var directives = new List<string> { CspConstants.Directives.DefaultSource };

        //    var existingSource = new CspSource { Source = source, Directives = CspConstants.Directives.FrameSource };
        //    _mockDynamicDataStore.Setup(x => x.Load<CspSource>(It.IsAny<Identity>()))
        //                         .Returns(existingSource);

        //    // Arrange
        //    await _repository.SaveAsync(id, source, directives);

        //    // Assert
        //    _mockDynamicDataStore.Verify(x => x.Save(existingSource), Times.Once);
        //}

        //[Test]
        //public async Task AppendDirective_GivenASourceThatIsNotMapped_ThenANewSourceRecordShouldBeCreated()
        //{
        //    // Arrange
        //    const string source = "https://www.example.com";
        //    const string directive = CspConstants.Directives.DefaultSource;
        //    _mockDynamicDataStore.Setup(x => x.Find<CspSource>(It.IsAny<string>(), It.IsAny<object>()))
        //                         .Returns(new List<CspSource>(0));
            
        //    CspSource savedSource = null;
        //    _mockDynamicDataStore.Setup(x => x.Save(It.IsAny<CspSource>()))
        //                         .Callback<object>(x => savedSource = x as CspSource);

        //    // Act
        //    await _repository.AppendDirectiveAsync(source, directive);

        //    // Assert
        //    Assert.That(savedSource, Is.Not.Null);
        //    Assert.That(savedSource.Source, Is.EqualTo(source));
        //    Assert.That(savedSource.Directives, Is.EqualTo(directive));
        //}

        //[Test]
        //public async Task AppendDirective_GivenASourceThatIsAlreadyMappedWithoutAMatchingDirective_ThenTheDirectiveShouldBeAppended()
        //{
        //    // Arrange
        //    const string source = "https://www.example.com";
        //    const string directive = CspConstants.Directives.DefaultSource;
        //    var existingSource = new CspSource 
        //    { 
        //        Id = Guid.NewGuid(), 
        //        Source = source, 
        //        Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource}"
        //    };

        //    _mockDynamicDataStore.Setup(x => x.Find<CspSource>(It.IsAny<string>(), It.IsAny<object>()))
        //                         .Returns(new List<CspSource> { existingSource });

        //    CspSource savedSource = null;
        //    _mockDynamicDataStore.Setup(x => x.Save(It.IsAny<CspSource>()))
        //                         .Callback<object>(x => savedSource = x as CspSource);

        //    // Act
        //    await _repository.AppendDirectiveAsync(source, directive);

        //    // Assert
        //    Assert.That(savedSource, Is.Not.Null);
        //    Assert.That(savedSource.Source, Is.EqualTo(existingSource.Source));
        //    Assert.That(savedSource.Directives, Is.EqualTo(existingSource.Directives));
        //}

        //[Test]
        //public async Task AppendDirective_GivenASourceThatIsAlreadyMappedWithAMatchingDirective_ThenTheDirectiveShouldNotBeAppended()
        //{
        //    // Arrange
        //    const string source = "https://www.example.com";
        //    const string directive = CspConstants.Directives.StyleSource;
        //    var existingSource = new CspSource
        //    {
        //        Id = Guid.NewGuid(),
        //        Source = source,
        //        Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource}"
        //    };

        //    _mockDynamicDataStore.Setup(x => x.Find<CspSource>(It.IsAny<string>(), It.IsAny<object>()))
        //                         .Returns(new List<CspSource> { existingSource });

        //    CspSource savedSource = null;
        //    _mockDynamicDataStore.Setup(x => x.Save(It.IsAny<CspSource>()))
        //                         .Callback<object>(x => savedSource = x as CspSource);

        //    // Act
        //    await _repository.AppendDirectiveAsync(source, directive);

        //    // Assert
        //    Assert.That(savedSource, Is.Not.Null);
        //    Assert.That(savedSource.Source, Is.EqualTo(existingSource.Source));
        //    Assert.That(savedSource.Directives.Contains(CspConstants.Directives.DefaultSource), Is.True);
        //    Assert.That(savedSource.Directives.Contains(CspConstants.Directives.ScriptSource), Is.True);
        //    Assert.That(savedSource.Directives.Contains(CspConstants.Directives.StyleSource), Is.True);
        //}
    }
}
