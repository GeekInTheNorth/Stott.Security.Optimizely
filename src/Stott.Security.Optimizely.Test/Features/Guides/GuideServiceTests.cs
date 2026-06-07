namespace Stott.Security.Optimizely.Test.Features.Guides;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Guides.Models;
using Stott.Security.Optimizely.Features.Guides.Service;

[TestFixture]
public sealed class GuideServiceTests
{
    private Mock<IHttpClientFactory> _mockClientFactory;

    private Mock<ICacheWrapper> _mockCache;

    private Mock<ILogger<IGuideService>> _mockLogger;

    private StubHttpMessageHandler _stubHandler;

    private GuideService _service;

    [SetUp]
    public void SetUp()
    {
        _stubHandler = new StubHttpMessageHandler();
        _mockClientFactory = new Mock<IHttpClientFactory>();
        _mockClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient(_stubHandler));

        _mockCache = new Mock<ICacheWrapper>();
        _mockLogger = new Mock<ILogger<IGuideService>>();

        _service = new GuideService(_mockClientFactory.Object, _mockCache.Object, _mockLogger.Object);
    }

    [Test]
    public async Task GetGuidesAsync_GivenASuccessfulResponse_ThenGuidesAreReturnedNewestFirst()
    {
        // Arrange
        const string json = """
        [
            { "title": "Older", "url": "https://www.stott.pro/article/older", "date": "2026-01-01T00:00:00+00:00", "description": "Older guide" },
            { "title": "Newer", "url": "https://www.stott.pro/article/newer", "date": "2026-04-30T00:00:00+00:00", "description": "Newer guide" }
        ]
        """;

        _stubHandler.SetResponse(HttpStatusCode.OK, json);

        // Act
        var guides = await _service.GetGuidesAsync();

        // Assert
        Assert.That(guides, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(guides[0].Title, Is.EqualTo("Newer"));
            Assert.That(guides[1].Title, Is.EqualTo("Older"));
        });
    }

    [Test]
    public async Task GetGuidesAsync_GivenACacheHit_ThenNoHttpCallIsMade()
    {
        // Arrange
        var cachedGuides = new List<GuideModel> { new() { Title = "Cached" } };
        _mockCache.Setup(x => x.Get<List<GuideModel>>(CspConstants.CacheKeys.Guides)).Returns(cachedGuides);

        // Act
        var guides = await _service.GetGuidesAsync();

        // Assert
        Assert.That(guides, Has.Count.EqualTo(1));
        Assert.That(guides[0].Title, Is.EqualTo("Cached"));
        Assert.That(_stubHandler.CallCount, Is.EqualTo(0));
    }

    [Test]
    public async Task GetGuidesAsync_GivenACacheMiss_ThenTheResultIsCached()
    {
        // Arrange
        _stubHandler.SetResponse(HttpStatusCode.OK, "[]");

        // Act
        await _service.GetGuidesAsync();

        // Assert
        _mockCache.Verify(x => x.Add(CspConstants.CacheKeys.Guides, It.IsAny<List<GuideModel>>()), Times.Once);
    }

    [Test]
    public async Task GetGuidesAsync_GivenANonSuccessStatusCode_ThenAnEmptyListIsReturned()
    {
        // Arrange
        _stubHandler.SetResponse(HttpStatusCode.InternalServerError, string.Empty);

        // Act
        var guides = await _service.GetGuidesAsync();

        // Assert
        Assert.That(guides, Is.Empty);
    }

    [Test]
    public async Task GetGuidesAsync_GivenTheRequestThrows_ThenAnEmptyListIsReturned()
    {
        // Arrange
        _stubHandler.SetException(new HttpRequestException("Boom"));

        // Act
        var guides = await _service.GetGuidesAsync();

        // Assert
        Assert.That(guides, Is.Empty);
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private HttpStatusCode _statusCode = HttpStatusCode.OK;

        private string _content = "[]";

        private Exception? _exception;

        public int CallCount { get; private set; }

        public void SetResponse(HttpStatusCode statusCode, string content)
        {
            _statusCode = statusCode;
            _content = content;
            _exception = null;
        }

        public void SetException(Exception exception)
        {
            _exception = exception;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;

            if (_exception is not null)
            {
                throw _exception;
            }

            return Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content)
            });
        }
    }
}
