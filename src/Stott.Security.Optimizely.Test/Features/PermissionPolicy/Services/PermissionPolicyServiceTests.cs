using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;
using Stott.Security.Optimizely.Features.PermissionPolicy.Repository;
using Stott.Security.Optimizely.Features.PermissionPolicy.Service;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy.Services
{
    public sealed class PermissionPolicyServiceTests
    {
        private Mock<ICacheWrapper> _mockCache;

        private Mock<IPermissionPolicyRepository> _mockRepository;

        private PermissionPolicyService _service;

        [SetUp]
        public void SetUp()
        {
            _mockCache = new Mock<ICacheWrapper>();
            _mockRepository = new Mock<IPermissionPolicyRepository>();
            _service = new PermissionPolicyService(_mockCache.Object, _mockRepository.Object);
        }

        [Test]
        public async Task GetPermissionPolicySettingsAsync_WhenSettingsAreInTheCache_ThenDataIsNotRetrievedFromTheRepository()
        {
            var settings = new PermissionPolicySettingsModel();
            _mockCache.Setup(x => x.Get<PermissionPolicySettingsModel>(It.IsAny<string>())).Returns(settings);

            var result = await _service.GetPermissionPolicySettingsAsync();

            Assert.That(result, Is.EqualTo(settings));
            _mockRepository.Verify(x => x.GetSettingsAsync(), Times.Never);
        }

        [Test]
        public async Task GetPermissionPolicySettingsAsync_WhenSettingsAreNotInTheCache_ThenDataIsRetrievedFromTheRepository()
        {
            var settings = new PermissionPolicySettingsModel();
            _mockRepository.Setup(x => x.GetSettingsAsync()).ReturnsAsync(settings);

            var result = await _service.GetPermissionPolicySettingsAsync();

            Assert.That(result, Is.EqualTo(settings));
            _mockRepository.Verify(x => x.GetSettingsAsync(), Times.Once);
            _mockCache.Verify(x => x.Add(It.IsAny<string>(), settings), Times.Once);
        }
    }
}
