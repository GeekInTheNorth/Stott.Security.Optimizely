using System.Collections.Generic;

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
    public void GetNonce_ReturnsNullWhenCspOrNonceIsDisabled(NonceSettings nonceSettings)
    {
        // Assert
        _mockNonceService.Setup(x => x.GetNonceSettingsAsync()).ReturnsAsync(nonceSettings);
        _mockSecurityRouteHelper.Setup(x => x.GetRouteType()).Returns(SecurityRouteType.Default);

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockNonceService.Object, _mockSecurityRouteHelper.Object);
        var nonce = nonceProvider.GetNonce();

        // Assert
        Assert.That(nonce, Is.Null);
    }

    [Test]
    [TestCase(SecurityRouteType.Unknown, false)]
    [TestCase(SecurityRouteType.Default, false)]
    [TestCase(SecurityRouteType.ContentSpecific, false)]
    [TestCase(SecurityRouteType.NoNonceOrHash, true)]
    public void GetNonce_ReturnsNullOnDisabledNonceAndHashRoutes(SecurityRouteType routeType, bool shouldBeNull)
    {
        // Assert
        var nonceSettings = new NonceSettings
        {
            IsEnabled = true,
            Directives = new List<string> { CspConstants.Directives.ScriptSource }
        };

        _mockNonceService.Setup(x => x.GetNonceSettingsAsync()).ReturnsAsync(nonceSettings);
        _mockSecurityRouteHelper.Setup(x => x.GetRouteType()).Returns(routeType);

        // Act
        var nonceProvider = new DefaultNonceProvider(_mockNonceService.Object, _mockSecurityRouteHelper.Object);
        var nonce = nonceProvider.GetNonce();
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