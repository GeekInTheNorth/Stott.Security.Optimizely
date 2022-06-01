using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Moq;

using NUnit.Framework;

using Stott.Security.Core.Common;
using Stott.Security.Core.Entities.Exceptions;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.Permissions;
using Stott.Security.Core.Features.Permissions.List;
using Stott.Security.Core.Features.Permissions.Repository;
using Stott.Security.Core.Features.Permissions.Save;

namespace Stott.Optimizely.Csp.Test.Features.Permissions
{
    [TestFixture]
    public class CspPermissionsControllerTests
    {
        private Mock<ICspPermissionsListModelBuilder> _mockViewModelBuilder;

        private Mock<ICspPermissionRepository> _mockRepository;

        private Mock<ILoggingProviderFactory> _mockLoggingProviderFactory;

        private Mock<ILoggingProvider> _mockLoggingProvider;

        private CspPermissionsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockViewModelBuilder = new Mock<ICspPermissionsListModelBuilder>();

            _mockRepository = new Mock<ICspPermissionRepository>();

            _mockLoggingProvider = new Mock<ILoggingProvider>();
            _mockLoggingProviderFactory = new Mock<ILoggingProviderFactory>();
            _mockLoggingProviderFactory.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(_mockLoggingProvider.Object);

            _controller = new CspPermissionsController(
                _mockViewModelBuilder.Object, 
                _mockRepository.Object, 
                _mockLoggingProviderFactory.Object);
        }

        [Test]
        public async Task Save_GivenAnInvalidModelState_ThenAnInvalidRequestResponseIsReturned()
        {
            // Arrange
            var saveModel = new SavePermissionModel
            {
                Id = Guid.NewGuid(),
                Source = CspConstants.Sources.Self,
                Directives = CspConstants.AllDirectives
            };

            _controller.ModelState.AddModelError(nameof(SavePermissionModel.Source), "An Error.");

            // Act
            var response = await _controller.Save(saveModel) as ContentResult;

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task Save_WhenTheCommandThrowsAEntityExistsException_ThenAnInvalidRequestResponseIsReturned()
        {
            // Arrange
            var saveModel = new SavePermissionModel
            {
                Id = Guid.NewGuid(),
                Source = CspConstants.Sources.Self,
                Directives = CspConstants.AllDirectives
            };

            _mockRepository.Setup(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                           .Throws(new EntityExistsException(string.Empty));

            // Act
            var response = await _controller.Save(saveModel) as ContentResult;

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void Save_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
        {
            // Arrange
            var saveModel = new SavePermissionModel
            {
                Id = Guid.NewGuid(),
                Source = CspConstants.Sources.Self,
                Directives = CspConstants.AllDirectives
            };

            _mockRepository.Setup(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                           .ThrowsAsync(new Exception(string.Empty));

            // Assert
            Assert.ThrowsAsync<Exception>(() => _controller.Save(saveModel));
        }

        [Test]
        public async Task Save_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Arrange
            var saveModel = new SavePermissionModel
            {
                Id = Guid.NewGuid(),
                Source = CspConstants.Sources.Self,
                Directives = CspConstants.AllDirectives
            };

            // Act
            var response = await _controller.Save(saveModel);

            // Assert
            Assert.That(response, Is.AssignableFrom<OkResult>());
        }

        [Test]
        public async Task Append_GivenAnInvalidModelState_ThenAnInvalidRequestResponseIsReturned()
        {
            // Arrange
            var saveModel = new AppendPermissionModel();
            _controller.ModelState.AddModelError(nameof(SavePermissionModel.Source), "An Error.");

            // Act
            var response = await _controller.Append(saveModel) as ContentResult;

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void Append_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
        {
            // Arrange
            var saveModel = new AppendPermissionModel
            {
                Source = CspConstants.Sources.Self,
                Directive = CspConstants.Directives.DefaultSource
            };

            _mockRepository.Setup(x => x.AppendDirectiveAsync(It.IsAny<string>(), It.IsAny<string>()))
                           .ThrowsAsync(new Exception(string.Empty));

            // Assert
            Assert.ThrowsAsync<Exception>(() => _controller.Append(saveModel));
        }

        [Test]
        public async Task Append_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Arrange
            var saveModel = new AppendPermissionModel
            {
                Source = CspConstants.Sources.Self,
                Directive = CspConstants.Directives.DefaultSource
            };

            // Act
            var response = await _controller.Append(saveModel);

            // Assert
            Assert.That(response, Is.AssignableFrom<OkResult>());
        }

        [Test]
        public async Task Delete_GivenAnEmptyGuid_ThenABadRequestIsReturned()
        {
            // Act
            var response = await _controller.Delete(Guid.Empty) as ContentResult;

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void Delete_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
        {
            // Arrange
            _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<Guid>()))
                           .ThrowsAsync(new Exception(string.Empty));

            // Assert
            Assert.ThrowsAsync<Exception>(() => _controller.Delete(Guid.NewGuid()));
        }

        [Test]
        public async Task Delete_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Act
            var response = await _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.That(response, Is.AssignableFrom<OkResult>());
        }
    }
}
