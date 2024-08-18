using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using NUnit.Framework;

namespace Stott.Security.Optimizely.Test.Standards;

[TestFixture]
public sealed class ControllerStandardsTests
{
    [Test]
    [TestCaseSource(typeof(ControllerStandardsTestCases), nameof(ControllerStandardsTestCases.PageControllerActionTestCases))]
    public void ControllersShouldHaveHttpMethodAttributes(string controllerName, string methodName, MethodInfo methodInfo)
    {
        // Act
        var hasHttpMethodAttribute = methodInfo.GetCustomAttributes(typeof(HttpMethodAttribute)).Any();

        // Assert
        // Controllers should only respond in intended Verbs and should respond with method not allowed on unintended verbs.
        // This will prevent posting of malicious payloads to methods not intended to retrieve of deal with these payloads.
        // First raised by a penetration test where an attempt to post files to a content end point returned a 200 instead of a 405
        Assert.That(hasHttpMethodAttribute, Is.True, $"{controllerName}.{methodName} should be decorated with a http method attribute.");
    }

    [Test]
    [TestCaseSource(typeof(ControllerStandardsTestCases), nameof(ControllerStandardsTestCases.PageControllerActionTestCases))]
    public void ControllerMethodsShouldEitherHaveAuthorizeOrAllowAnonymousAttributes(string controllerName, string methodName, MethodInfo methodInfo)
    {
        // Act
        var hasAuthorizeAttribute = methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute)).Any();
        var hasAllowAnonymousAttribute = methodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute)).Any();
        var controllerHasAuthorizeAttribute = methodInfo.DeclaringType?.GetCustomAttributes(typeof(AuthorizeAttribute)).Any() ?? false;

        var hasAttribute = hasAuthorizeAttribute || hasAllowAnonymousAttribute || controllerHasAuthorizeAttribute;

        // Assert
        // Controller actions should be protected with an Authorization attribute or an intentional AllowAnonymous attribute.
        // This will ensure your controllers are secure by default and that you have to explicitly allow anonymous access.
        Assert.That(hasAttribute, Is.True, $"{controllerName}.{methodName} should be decorated directly or indirectly with an Authorize or AllowAnonymous attribute.");
    }

    [Test]
    [TestCaseSource(typeof(ControllerStandardsTestCases), nameof(ControllerStandardsTestCases.PageControllerActionTestCases))]
    public void ControllerMethodsShouldHaveRouteAttributes(string controllerName, string methodName, MethodInfo methodInfo)
    {
        // Act
        var hasRouteAttribute = methodInfo.GetCustomAttributes(typeof(RouteAttribute)).Any();
        var controllerHasRouteAttribute = methodInfo.DeclaringType?.GetCustomAttributes(typeof(RouteAttribute)).Any() ?? false;

        var hasAttribute = hasRouteAttribute || controllerHasRouteAttribute;

        // Assert
        // Controller actions should have a fixed route attribute so as to not have clashes with routes declared by other modules.
        Assert.That(hasAttribute, Is.True, $"{controllerName}.{methodName} should be decorated directly with a Route attribute.");
    }
}
