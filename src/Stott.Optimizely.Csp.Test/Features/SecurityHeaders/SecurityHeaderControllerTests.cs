using System;

using Microsoft.AspNetCore.Mvc;

using Moq;

using NUnit.Framework;

using Stott.Optimizely.Csp.Entities;
using Stott.Optimizely.Csp.Features.SecurityHeaders;
using Stott.Optimizely.Csp.Features.SecurityHeaders.Repository;

namespace Stott.Optimizely.Csp.Test.Features.SecurityHeaders
{
    [TestFixture]
    public class SecurityHeaderControllerTests
    {
        private Mock<ISecurityHeaderRepository> _mockRepository;

        private SecurityHeaderController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ISecurityHeaderRepository>();

            _controller = new SecurityHeaderController(_mockRepository.Object);
        }

        [Test]
        public void Get_CallsGetFromTheRepository()
        {
            // Arrange
            _mockRepository.Setup(x => x.Get())
                           .Returns(new SecurityHeaderSettings());

            // Act
            _controller.Get();

            // Assert
            _mockRepository.Verify(x => x.Get(), Times.Once());
        }

        [Test]
        public void Get_ReturnsErrorWhenRespositoryThrowsAnException()
        {
            // Arrange
            _mockRepository.Setup(x => x.Get())
                           .Throws(new Exception(string.Empty));

            // Assert
            Assert.Throws<Exception>(() => _controller.Get());
        }

        [Test]
        public void Get_ReturnsSuccessResponseWhenRespositoryReturnsData()
        {
            // Arrange
            _mockRepository.Setup(x => x.Get())
                           .Returns(new SecurityHeaderSettings());

            // Act
            var response = _controller.Get();

            // Assert
            Assert.That(response, Is.AssignableFrom<JsonResult>());
        }
    }
}
