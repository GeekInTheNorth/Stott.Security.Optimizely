using System.Collections.Generic;

using NUnit.Framework;

using Stott.Optimizely.Csp.Common;
using Stott.Optimizely.Csp.Entities;

namespace Stott.Optimizely.Csp.Test.Features.Header
{
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
    }
}
