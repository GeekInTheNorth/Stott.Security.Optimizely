using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Moq;

using NUnit.Framework;

using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.Logging;
using Stott.Security.Core.Features.SecurityHeaders;
using Stott.Security.Core.Features.SecurityHeaders.Repository;

namespace Stott.Optimizely.Csp.Test.Features.SecurityHeaders
{
    [TestFixture]
    public class SecurityHeaderControllerTests
    {
        private Mock<ISecurityHeaderRepository> _mockRepository;

        private Mock<ILoggingProviderFactory> _mockLoggingProviderFactory;

        private Mock<ILoggingProvider> _mockLoggingProvider;

        private SecurityHeaderController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ISecurityHeaderRepository>();

            _mockLoggingProvider = new Mock<ILoggingProvider>();
            _mockLoggingProviderFactory = new Mock<ILoggingProviderFactory>();
            _mockLoggingProviderFactory.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(_mockLoggingProvider.Object);

            _controller = new SecurityHeaderController(_mockRepository.Object, _mockLoggingProviderFactory.Object);
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
