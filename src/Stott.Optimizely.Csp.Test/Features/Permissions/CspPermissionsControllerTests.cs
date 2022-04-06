using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities.Exceptions;
using Stott.Optimizely.Csp.Features.Permissions;
using Stott.Optimizely.Csp.Features.Permissions.List;
using Stott.Optimizely.Csp.Features.Permissions.Repository;
using Stott.Optimizely.Csp.Features.Permissions.Save;

namespace Stott.Optimizely.Csp.Test.Features.Permissions
{
    [TestFixture]
    public class CspPermissionsControllerTests
    {
        private Mock<ICspPermissionsListModelBuilder> _mockViewModelBuilder;

        private Mock<ICspPermissionRepository> _mockRepository;

        private CspPermissionsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockViewModelBuilder = new Mock<ICspPermissionsListModelBuilder>();

            _mockRepository = new Mock<ICspPermissionRepository>();

            _controller = new CspPermissionsController(_mockViewModelBuilder.Object, _mockRepository.Object);
        }

        [Test]
        public void Save_GivenAnInvalidModelState_ThenAnInvalidRequestResponseIsReturned()
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
            var response = _controller.Save(saveModel) as ContentResult;

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void Save_WhenTheCommandThrowsAEntityExistsException_ThenAnInvalidRequestResponseIsReturned()
        {
            // Arrange
            var saveModel = new SavePermissionModel
            {
                Id = Guid.NewGuid(),
                Source = CspConstants.Sources.Self,
                Directives = CspConstants.AllDirectives
            };

            _mockRepository.Setup(x => x.Save(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                           .Throws(new EntityExistsException(string.Empty));

            // Act
            var response = _controller.Save(saveModel) as ContentResult;

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

            _mockRepository.Setup(x => x.Save(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                           .Throws(new Exception(string.Empty));

            // Assert
            Assert.Throws<Exception>(() => _controller.Save(saveModel));
        }

        [Test]
        public void Save_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Arrange
            var saveModel = new SavePermissionModel
            {
                Id = Guid.NewGuid(),
                Source = CspConstants.Sources.Self,
                Directives = CspConstants.AllDirectives
            };

            // Act
            var response = _controller.Save(saveModel);


            // Assert
            Assert.That(response, Is.AssignableFrom<OkResult>());
        }

        [Test]
        public void Append_GivenAnInvalidModelState_ThenAnInvalidRequestResponseIsReturned()
        {
            // Arrange
            var saveModel = new AppendPermissionModel();
            _controller.ModelState.AddModelError(nameof(SavePermissionModel.Source), "An Error.");

            // Act
            var response = _controller.Append(saveModel) as ContentResult;

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

            _mockRepository.Setup(x => x.AppendDirective(It.IsAny<string>(), It.IsAny<string>()))
                           .Throws(new Exception(string.Empty));

            // Assert
            Assert.Throws<Exception>(() => _controller.Append(saveModel));
        }

        [Test]
        public void Append_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Arrange
            var saveModel = new AppendPermissionModel
            {
                Source = CspConstants.Sources.Self,
                Directive = CspConstants.Directives.DefaultSource
            };

            // Act
            var response = _controller.Append(saveModel);


            // Assert
            Assert.That(response, Is.AssignableFrom<OkResult>());
        }

        [Test]
        public void Delete_GivenAnEmptyGuid_ThenABadRequestIsReturned()
        {
            // Act
            var response = _controller.Delete(Guid.Empty) as ContentResult;

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public void Delete_WhenTheCommandThrowsAnException_ThenTheErrorIsReThrown()
        {
            // Arrange
            _mockRepository.Setup(x => x.Delete(It.IsAny<Guid>()))
                           .Throws(new Exception(string.Empty));

            // Assert
            Assert.Throws<Exception>(() => _controller.Delete(Guid.NewGuid()));
        }

        [Test]
        public void Delete_WhenTheCommandIsSuccessful_ThenAnOkResponseIsReturned()
        {
            // Act
            var response = _controller.Delete(Guid.NewGuid());

            // Assert
            Assert.That(response, Is.AssignableFrom<OkResult>());
        }
    }
}
