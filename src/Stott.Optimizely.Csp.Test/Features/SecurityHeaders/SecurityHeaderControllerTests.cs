using System;
using System.Threading.Tasks;

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
        public async Task Get_CallsGetFromTheRepository()
        {
            // Arrange
            _mockRepository.Setup(x => x.GetAsync())
                           .ReturnsAsync(new SecurityHeaderSettings());

            // Act
            await _controller.Get();

            // Assert
            _mockRepository.Verify(x => x.GetAsync(), Times.Once());
        }

        [Test]
        public void Get_ReturnsErrorWhenRespositoryThrowsAnException()
        {
            // Arrange
            _mockRepository.Setup(x => x.GetAsync())
                           .ThrowsAsync(new Exception(string.Empty));

            // Assert
            Assert.ThrowsAsync<Exception>(() => _controller.Get());
        }

        [Test]
        public async Task Get_ReturnsSuccessResponseWhenRespositoryReturnsData()
        {
            // Arrange
            _mockRepository.Setup(x => x.GetAsync())
                           .ReturnsAsync(new SecurityHeaderSettings());

            // Act
            var response = await _controller.Get();

            // Assert
            Assert.That(response, Is.AssignableFrom<ContentResult>());
            Assert.That((response as ContentResult).StatusCode, Is.EqualTo(200));
        }
    }
}
