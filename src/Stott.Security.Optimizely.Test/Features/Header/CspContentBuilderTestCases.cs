namespace Stott.Security.Optimizely.Test.Features.Header;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Entities;

public static class CspContentBuilderTestCases
{
    public static IEnumerable<TestCaseData> MultipleMatchingSourceTestCases
    {
        get
        {
            var cspOne = new CspSource
            {
                Source = "https://www.example.com",
                Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource}"
            };

            var cspTwo = new CspSource
            {
                Source = "https://www.example.com",
                Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.FrameSource}"
            };

            var cspThree = new CspSource
            {
                Source = CspConstants.Sources.Self,
                Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource},{CspConstants.Directives.FrameSource}"
            };

            yield return new TestCaseData
            (
                new List<CspSource> { cspOne, cspTwo },
                "default-src https://www.example.com; script-src https://www.example.com; frame-src https://www.example.com;"
            );

            yield return new TestCaseData
            (
                new List<CspSource> { cspOne, cspTwo, cspThree },
                "default-src 'self' https://www.example.com; script-src 'self' https://www.example.com; frame-src 'self' https://www.example.com;"
            );

            yield return new TestCaseData
            (
                new List<CspSource> { cspOne, cspThree },
                "default-src 'self' https://www.example.com; script-src 'self' https://www.example.com; frame-src 'self';"
            );
        }
    }

    public static IEnumerable<TestCaseData> NonMatchingSourceTestCases
    {
        get
        {
            var cspOne = new CspSource
            {
                Source = "https://www.example-one.com",
                Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource}"
            };

            var cspTwo = new CspSource
            {
                Source = "https://www.example-two.com",
                Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.FrameSource}"
            };

            var cspThree = new CspSource
            {
                Source = CspConstants.Sources.Self,
                Directives = $"{CspConstants.Directives.DefaultSource},{CspConstants.Directives.ScriptSource},{CspConstants.Directives.FrameSource}"
            };

            yield return new TestCaseData
            (
                new List<CspSource> { cspOne, cspTwo },
                "default-src https://www.example-one.com https://www.example-two.com; script-src https://www.example-one.com; frame-src https://www.example-two.com;"
            );

            yield return new TestCaseData
            (
                new List<CspSource> { cspOne, cspTwo, cspThree },
                "default-src 'self' https://www.example-one.com https://www.example-two.com; script-src 'self' https://www.example-one.com; frame-src 'self' https://www.example-two.com;"
            );

            yield return new TestCaseData
            (
                new List<CspSource> { cspOne, cspThree },
                "default-src 'self' https://www.example-one.com; script-src 'self' https://www.example-one.com; frame-src 'self';"
            );
        }
    }

    public static IEnumerable<TestCaseData> GetSandboxContentTestCases
    {
        get
        {
            yield return new TestCaseData(false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, "sandbox;");
            yield return new TestCaseData(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, "sandbox allow-downloads;");
            yield return new TestCaseData(false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, "sandbox allow-downloads-without-user-activation;");
            yield return new TestCaseData(false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, "sandbox allow-forms;");
            yield return new TestCaseData(false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, "sandbox allow-modals;");
            yield return new TestCaseData(false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, "sandbox allow-orientation-lock;");
            yield return new TestCaseData(false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, "sandbox allow-pointer-lock;");
            yield return new TestCaseData(false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, "sandbox allow-popups;");
            yield return new TestCaseData(false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, "sandbox allow-popups-to-escape-sandbox;");
            yield return new TestCaseData(false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, "sandbox allow-presentation;");
            yield return new TestCaseData(false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, "sandbox allow-same-origin;");
            yield return new TestCaseData(false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, "sandbox allow-scripts;");
            yield return new TestCaseData(false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, "sandbox allow-storage-access-by-user-activation;");
            yield return new TestCaseData(false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, "sandbox allow-top-navigation;");
            yield return new TestCaseData(false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, "sandbox allow-top-navigation-by-user-activation;");
            yield return new TestCaseData(false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, "sandbox allow-top-navigation-to-custom-protocols;");
            yield return new TestCaseData(true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, "sandbox allow-downloads allow-downloads-without-user-activation allow-forms allow-modals allow-orientation-lock allow-pointer-lock allow-popups allow-popups-to-escape-sandbox allow-presentation allow-same-origin allow-scripts allow-storage-access-by-user-activation allow-top-navigation allow-top-navigation-by-user-activation allow-top-navigation-to-custom-protocols;");
        }
    }
}
