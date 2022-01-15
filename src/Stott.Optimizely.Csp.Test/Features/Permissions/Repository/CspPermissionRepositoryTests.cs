using System;
using System.Collections.Generic;

using EPiServer.Data;
using EPiServer.Data.Dynamic;

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
        private Mock<DynamicDataStoreFactory> _mockDynamicDataStoreFactory;

        private Mock<DynamicDataStore> _mockDynamicDataStore;

        private Mock<StoreDefinition> _mockStoreDefinition;

        private CspPermissionRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockStoreDefinition = new Mock<StoreDefinition>(
                MockBehavior.Loose,
                string.Empty,
                new List<PropertyMap>(0),
                null);

            _mockDynamicDataStore = new Mock<DynamicDataStore>(
                MockBehavior.Loose,
                _mockStoreDefinition.Object);

            _mockDynamicDataStoreFactory = new Mock<DynamicDataStoreFactory>();
            _mockDynamicDataStoreFactory.Setup(x => x.CreateStore(typeof(CspSource))).Returns(_mockDynamicDataStore.Object);

            _repository = new CspPermissionRepository(_mockDynamicDataStoreFactory.Object);
        }

        [Test]
        [TestCaseSource(typeof(CommonTestCases), nameof(CommonTestCases.EmptyNullOrWhitespaceStrings))]
        public void Save_GivenAnEmptyOrNullSource_ThenAnArgumentNullExceptionShouldBeThrown(string source)
        {
            // Arrange
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            // Assert
            Assert.Throws<ArgumentNullException>(() => _repository.Save(Guid.Empty, source, directives));
        }

        [Test]
        [TestCaseSource(typeof(CspPermissionRepositoryTestCases), nameof(CspPermissionRepositoryTestCases.InvalidDirectivesTestCases))]
        public void Save_GivenAnEmptyOrNullDirectives_ThenAnArgumentExceptionShouldBeThrown(List<string> directives)
        {
            // Arrange
            var source = CspConstants.Sources.Self;

            // Assert
            Assert.Throws<ArgumentException>(() => _repository.Save(Guid.Empty, source, directives));
        }

        [Test]
        public void Save_GivenSourceExistsAgainstAnotherEntry_ThenAnEntityExistsExceptionShouldBeThrown()
        {
            // Arrange
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            var existingCspSource = new CspSource { Id = Guid.NewGuid(), Source = source };

            _mockDynamicDataStore.Setup(x => x.Find<CspSource>(It.IsAny<string>(), It.IsAny<object>()))
                                 .Returns(new List<CspSource> { existingCspSource });

            // Assert
            Assert.Throws<EntityExistsException>(() => _repository.Save(Guid.Empty, source, directives));
        }

        [Test]
        public void Save_GivenSourceDoesNotExistForAnotherEntry_ThenAnEntityExistsExceptionShouldNotBeThrown()
        {
            // Arrange
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            _mockDynamicDataStore.Setup(x => x.Find<CspSource>(It.IsAny<string>(), It.IsAny<object>()))
                                 .Returns(new List<CspSource>(0));

            // Assert
            Assert.DoesNotThrow(() => _repository.Save(Guid.Empty, source, directives));
        }

        [Test]
        public void Save_GivenAValidCspSourceForANewSource_ThenANewRecordShouldBeSaved()
        {
            // Arrange
            var id = Guid.Empty;
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            CspSource savedSource = null;
            _mockDynamicDataStore.Setup(x => x.Save(It.IsAny<CspSource>()))
                                 .Callback<object>(x => savedSource = x as CspSource);

            // Arrange
            _repository.Save(id, source, directives);

            // Assert
            _mockDynamicDataStore.Verify(x => x.Save(It.IsAny<CspSource>()), Times.Once);
            Assert.That(savedSource, Is.Not.Null);
            Assert.That(savedSource.Source, Is.EqualTo(source));
            Assert.That(savedSource.Directives, Is.EqualTo(CspConstants.Directives.DefaultSource));
        }

        [Test]
        public void Save_GivenAValidCspSourceForAnExistingSource_ThenTheExistingRecordShouldBeLoaded()
        {
            // Arrange
            var id = Guid.NewGuid();
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            var existingSource = new CspSource { Source = source, Directives = CspConstants.Directives.FrameSource };
            _mockDynamicDataStore.Setup(x => x.Load<CspSource>(It.IsAny<Identity>()))
                                 .Returns(existingSource);

            // Arrange
            _repository.Save(id, source, directives);

            // Assert
            _mockDynamicDataStore.Verify(x => x.Load<CspSource>(It.IsAny<Identity>()), Times.Once);
        }

        [Test]
        public void Save_GivenAValidCspSourceForAnExistingSource_ThenTheExistingRecordShouldBeSaved()
        {
            // Arrange
            var id = Guid.NewGuid();
            var source = CspConstants.Sources.Self;
            var directives = new List<string> { CspConstants.Directives.DefaultSource };

            var existingSource = new CspSource { Source = source, Directives = CspConstants.Directives.FrameSource };
            _mockDynamicDataStore.Setup(x => x.Load<CspSource>(It.IsAny<Identity>()))
                                 .Returns(existingSource);

            // Arrange
            _repository.Save(id, source, directives);

            // Assert
            _mockDynamicDataStore.Verify(x => x.Save(existingSource), Times.Once);
        }
    }
}
