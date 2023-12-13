namespace Stott.Security.Optimizely.Test.Features.Cors.Provider;

using System;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Cors;
using Stott.Security.Optimizely.Features.Cors.Provider;
using Stott.Security.Optimizely.Features.Cors.Service;

[TestFixture]
public sealed class CustomCorsPolicyProviderTests
{
    private Mock<ICacheWrapper> _mockCache;

    private Mock<ICorsSettingsService> _mockService;

    private Mock<IOptions<CorsOptions>> _mockOptions;

    private Mock<HttpContext> _mockHttpContext;

    private CustomCorsPolicyProvider _provider;

    [SetUp]
    public void SetUp()
    {
        _mockCache = new Mock<ICacheWrapper>();

        _mockService = new Mock<ICorsSettingsService>();
        _mockService.Setup(x => x.GetAsync()).ReturnsAsync(new CorsConfiguration());

        _mockHttpContext = new Mock<HttpContext>();

        var corsOptions = new CorsOptions();
        corsOptions.AddPolicy("Test-Policy", x =>
        {
            x.AllowAnyHeader();
            x.AllowAnyMethod();
            x.AllowAnyOrigin();
        });

        _mockOptions = new Mock<IOptions<CorsOptions>>();
        _mockOptions.Setup(x => x.Value).Returns(corsOptions);

        _provider = new CustomCorsPolicyProvider(_mockCache.Object, _mockService.Object, _mockOptions.Object);
    }

    [Test]
    public async Task GivenTheProvidedPolicyNameMatchesAConfigurationInCode_ThenPolicyWillNotBeLoadedFromCacheOrDatabase()
    {
        // Act
        _ = await _provider.GetPolicyAsync(_mockHttpContext.Object, "Test-Policy");

        // Assert
        _mockCache.Verify(x => x.Get<CorsPolicy>(It.IsAny<string>()), Times.Never());
        _mockService.Verify(x => x.GetAsync(), Times.Never());
    }

    [Test]
    public async Task GivenTheProvidedPolicyNameDoesNotMatchAConfigurationInCode_ThenPolicyWillBeLoadedFromCacheOrDatabase()
    {
        // Act
        _ = await _provider.GetPolicyAsync(_mockHttpContext.Object, Guid.NewGuid().ToString());

        // Assert
        _mockCache.Verify(x => x.Get<CorsPolicy>(It.IsAny<string>()), Times.Once());
        _mockService.Verify(x => x.GetAsync(), Times.Once());
    }

    [Test]
    [TestCaseSource(typeof(CustomCorsPolicyProviderTestCases), nameof(CustomCorsPolicyProviderTestCases.GetNullOrDefaultPolicyNameTestCases))]
    public async Task GivenTheProvidedPolicyNameIsEmptyOrMatchesTheModuleDefault_ThenPolicyWillBeLoadedFromCacheOrDatabase(
        [CanBeNull]string policyName)
    {
        // Act
        _ = await _provider.GetPolicyAsync(_mockHttpContext.Object, policyName);

        // Assert
        _mockCache.Verify(x => x.Get<CorsPolicy>(It.IsAny<string>()), Times.Once());
        _mockService.Verify(x => x.GetAsync(), Times.Once());
    }
}