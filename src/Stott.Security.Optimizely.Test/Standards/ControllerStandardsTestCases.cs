using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.LandingPage;

namespace Stott.Security.Optimizely.Test.Standards;

public static class ControllerStandardsTestCases
{
    public static IEnumerable<TestCaseData> PageControllerActionTestCases
    {
        get
        {
            var assembly = Assembly.GetAssembly(typeof(SettingsLandingPageController));

            if (assembly == null)
            {
                yield break;
            }

            var controllers = assembly.GetTypes()
                                      .Where(t => (t.BaseType?.Name.StartsWith("Controller") ?? false)
                                               || (t.BaseType?.Name.StartsWith("BaseController") ?? false))
                                      .ToList();

            foreach (var controller in controllers)
            {
                var actions = controller.GetMethods()
                                        .Where(x => x.DeclaringType == controller && x.IsPublic)
                                        .ToList();

                foreach (var methodInfo in actions)
                {
                    if (methodInfo.ReturnType == typeof(IActionResult) || methodInfo.ReturnType == typeof(Task<IActionResult>))
                    {
                        yield return new TestCaseData(controller.Name, methodInfo.Name, methodInfo);
                    }
                }
            }
        }
    }
}
