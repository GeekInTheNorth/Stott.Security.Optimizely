using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.PermissionPolicy;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy;

public static class SavePermissionPolicyModelTestCases
{
    public static IEnumerable<TestCaseData> DirectiveNameTestCases
    {
        get
        {
            yield return new TestCaseData(null, true);
            yield return new TestCaseData(string.Empty, true);
            yield return new TestCaseData(" ", true);
            yield return new TestCaseData("not-a-directive", true);
            foreach (var name in PermissionPolicyConstants.AllDirectives)
            {
                yield return new TestCaseData(name, false);
            }
        }
    }

    public static IEnumerable<TestCaseData> EnabledStateSourceTestCases
    {
        get
        {
            var nullSourceList = (List<string>)null;
            var emptySources = new List<string>();
            var singleSource = new List<string> { "https://www.example.com" };
            var multipleSources = new List<string> { "https://www.example.com", "https://*.example.com" };
            var nullSources = new List<string> { null };

            yield return new TestCaseData(PermissionPolicyEnabledState.None, nullSourceList, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.None, emptySources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.None, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.None, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.None, nullSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, nullSourceList, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, emptySources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.All, nullSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, nullSourceList, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, emptySources, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.SpecificSites, nullSources, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, nullSourceList, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, emptySources, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisAndSpecificSites, nullSources, true);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, nullSourceList, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, emptySources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, singleSource, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, multipleSources, false);
            yield return new TestCaseData(PermissionPolicyEnabledState.ThisSite, nullSources, false);
        }
    }

    public static IEnumerable<TestCaseData> SourceTestCases
    {
        get
        {
            yield return new TestCaseData("ftp://www.example.com", true);
            yield return new TestCaseData("www.example.com", true);
            yield return new TestCaseData("http://www.example.com", false);
            yield return new TestCaseData("http://www.example.com/", false);
            yield return new TestCaseData("http://*.example.com", false);
            yield return new TestCaseData("http://*.example.com/", false);
            yield return new TestCaseData("https://www.example.com", false);
            yield return new TestCaseData("https://www.example.com/", false);
            yield return new TestCaseData("https://*.example.com", false);
            yield return new TestCaseData("https://*.example.com/", false);
            yield return new TestCaseData("ws://www.example.com", false);
            yield return new TestCaseData("ws://www.example.com/", false);
            yield return new TestCaseData("ws://*.example.com", false);
            yield return new TestCaseData("ws://*.example.com/", false);
            yield return new TestCaseData("wss://www.example.com", false);
            yield return new TestCaseData("wss://www.example.com/", false);
            yield return new TestCaseData("wss://*.example.com", false);
            yield return new TestCaseData("wss://*.example.com/", false);
            yield return new TestCaseData("http://www.*.com", true);
            yield return new TestCaseData("http://*.com", true);
            yield return new TestCaseData("https://abc1d23456de7f890g12-h34ijklm567nop890qr12stu3v4567wx.ssl.cf5.rackcdn.com", false);
            yield return new TestCaseData("https://*.ssl.cf5.rackcdn.com", false);
            yield return new TestCaseData("https://abc1d23456de7f890g12-h34ijklm567nop890qr12stu3v4567wx.ssl.cf5.rackcdn.com/1234/v4.3.2iframeResizer.min.js", true);
            yield return new TestCaseData("https://www.example.com:443", false);
            yield return new TestCaseData("https://www.example.com:443/", false);
            yield return new TestCaseData("https://www.example.com:", true);
            yield return new TestCaseData("https://www.example.com:/", true);
        }
    }
}