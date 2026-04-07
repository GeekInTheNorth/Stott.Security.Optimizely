using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp.Nonce;
using Stott.Security.Optimizely.Features.Route;

namespace Stott.Security.Optimizely.Test.Features.Csp.Nonce;

[TestFixture]
public sealed class DefaultNonceProviderTests
{
    private Mock<INonceService> _mockNonceService;

    private Mock<ISecurityRouteHelper> _mockSecurityRouteHelper;

    [SetUp]
    public void SetUp()
    {
        _mockNonceService = new Mock<INonceService>();
        _mockSecurityRouteHelper = new Mock<ISecurityRouteHelper>();
    }

    [Test]
    [TestCaseSource(typeof(DefaultNonceProviderTestCases), nameof(DefaultNonceProviderTestCases.InvalidSettingsTestCases))]
    public async Task GetCspValueAsync_ReturnsNullWhenCspOrNonceIsDisabled(NonceSettings nonceSettings)
    {
        // Arrange
        _mockNonceService.Setup(x => x.GetNonceSettingsAsync(It.IsAny<SecurityRouteData>())).ReturnsAsync(nonceSettings);
        _mockSecurityRouteHelper.Setup(x => x.GetRouteDataAsync()).ReturnsAsync(new SecurityRouteData { RouteType = SecurityRouteType.Default });

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockNonceService.Object, _mockSecurityRouteHelper.Object);
        var nonce = await nonceProvider.GetCspValueAsync();

        // Assert
        Assert.That(nonce, Is.Null);
    }

    [Test]
    [TestCase(SecurityRouteType.Unknown, false)]
    [TestCase(SecurityRouteType.Default, false)]
    [TestCase(SecurityRouteType.ContentSpecific, false)]
    [TestCase(SecurityRouteType.NoNonceOrHash, true)]
    public async Task GetNonceAsync_ReturnsNullOnDisabledNonceAndHashRoutes(SecurityRouteType routeType, bool shouldBeNull)
    {
        // Arrange
        var nonceSettings = new NonceSettings
        {
            IsEnabled = true,
            Directives = [CspConstants.Directives.ScriptSource]
        };

        _mockNonceService.Setup(x => x.GetNonceSettingsAsync(It.IsAny<SecurityRouteData>())).ReturnsAsync(nonceSettings);
        _mockSecurityRouteHelper.Setup(x => x.GetRouteDataAsync()).ReturnsAsync(new SecurityRouteData { RouteType = routeType });

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockNonceService.Object, _mockSecurityRouteHelper.Object);
        var nonce = await nonceProvider.GetCspValueAsync();
        var nonceIsNull = nonce is null;

        // Assert
        Assert.That(nonceIsNull, Is.EqualTo(shouldBeNull));
    }
}

public static class DefaultNonceProviderTestCases
{
    public static IEnumerable<TestCaseData> InvalidSettingsTestCases()
    {
        yield return new TestCaseData(new NonceSettings());
        yield return new TestCaseData(new NonceSettings { IsEnabled = true });
        yield return new TestCaseData(new NonceSettings { IsEnabled = true, Directives = [] });
    }
}
