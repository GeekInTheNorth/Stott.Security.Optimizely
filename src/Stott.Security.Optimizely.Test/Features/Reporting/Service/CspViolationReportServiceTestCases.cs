namespace Stott.Security.Optimizely.Test.Features.Reporting.Service;

using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;

public static class CspViolationReportServiceTestCases
{
    public static IEnumerable<TestCaseData> BlockedUriTransformTestCases
    {
        get
        {
            yield return new TestCaseData("blob", CspConstants.Sources.SchemeBlob);
            yield return new TestCaseData("data", CspConstants.Sources.SchemeData);
            yield return new TestCaseData("filesystem", CspConstants.Sources.SchemeFileSystem);
            yield return new TestCaseData("http", CspConstants.Sources.SchemeHttp);
            yield return new TestCaseData("https", CspConstants.Sources.SchemeHttps);
            yield return new TestCaseData("mediastream", CspConstants.Sources.SchemeMediaStream);
            yield return new TestCaseData("self", CspConstants.Sources.Self);
            yield return new TestCaseData("unsafe-eval", CspConstants.Sources.UnsafeEval);
            yield return new TestCaseData("eval", CspConstants.Sources.UnsafeEval);
            yield return new TestCaseData("wasm-unsafe-eval", CspConstants.Sources.WebAssemblyUnsafeEval);
            yield return new TestCaseData("unsafe-hashes", CspConstants.Sources.UnsafeHashes);
            yield return new TestCaseData("hashes", CspConstants.Sources.UnsafeHashes);
            yield return new TestCaseData("unsafe-inline", CspConstants.Sources.UnsafeInline);
            yield return new TestCaseData("inline", CspConstants.Sources.UnsafeInline);
            yield return new TestCaseData("none", CspConstants.Sources.None);
            yield return new TestCaseData("https://www.example.com/some-part/?someQuery=true", "https://www.example.com/some-part/");
            yield return new TestCaseData("https://www.example.com/segment-one/?query=one", "https://www.example.com/segment-one/");
        }
    }

    public static IEnumerable<TestCaseData> RepositorySaveAttemptsTestCases
    {
        get
        {
            yield return new TestCaseData("blob", CspConstants.Directives.DefaultSource, 1);
            yield return new TestCaseData("https://www.example.com/some-part/?someQuery=true", CspConstants.Directives.ConnectSource, 1);
            yield return new TestCaseData("https://www.example.com/some-part/?someQuery=true", " ", 0);
            yield return new TestCaseData("https://www.example.com/some-part/?someQuery=true", string.Empty, 0);
            yield return new TestCaseData("https://www.example.com/some-part/?someQuery=true", null, 0);
            yield return new TestCaseData("not-a-valid-source", CspConstants.Directives.ConnectSource, 0);
            yield return new TestCaseData(" ", CspConstants.Directives.ConnectSource, 0);
            yield return new TestCaseData(string.Empty, CspConstants.Directives.ConnectSource, 0);
            yield return new TestCaseData(null, CspConstants.Directives.ConnectSource, 0);
        }
    }
}