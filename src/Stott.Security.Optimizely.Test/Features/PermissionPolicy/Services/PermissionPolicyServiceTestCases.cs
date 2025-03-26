using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.PermissionPolicy;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy.Services;

public static class PermissionPolicyServiceTestCases
{
    public static IEnumerable<TestCaseData> FilterDirectiveTests
    {
        get
        {
            var allDirectives = string.Join(",", PermissionPolicyConstants.AllDirectives);

            yield return new TestCaseData(null, PermissionPolicyEnabledFilter.All, allDirectives);
            yield return new TestCaseData("example", PermissionPolicyEnabledFilter.All, $"{PermissionPolicyConstants.Bluetooth},{PermissionPolicyConstants.BrowsingTopics}");
            yield return new TestCaseData("exampleone", PermissionPolicyEnabledFilter.All, PermissionPolicyConstants.Bluetooth);
            yield return new TestCaseData("exampletwo", PermissionPolicyEnabledFilter.All, PermissionPolicyConstants.BrowsingTopics);
            yield return new TestCaseData(null, PermissionPolicyEnabledFilter.AllEnabled, $"{PermissionPolicyConstants.AmbientLightSensor},{PermissionPolicyConstants.AttributionReporting},{PermissionPolicyConstants.Autoplay},{PermissionPolicyConstants.Bluetooth},{PermissionPolicyConstants.BrowsingTopics}");
            yield return new TestCaseData(null, PermissionPolicyEnabledFilter.AllSites, PermissionPolicyConstants.AttributionReporting);
            yield return new TestCaseData(null, PermissionPolicyEnabledFilter.ThisSite, $"{PermissionPolicyConstants.Autoplay},{PermissionPolicyConstants.BrowsingTopics}");
            yield return new TestCaseData(null, PermissionPolicyEnabledFilter.SpecificSites, $"{PermissionPolicyConstants.Bluetooth},{PermissionPolicyConstants.BrowsingTopics}");
            yield return new TestCaseData("example", PermissionPolicyEnabledFilter.AllEnabled, $"{PermissionPolicyConstants.Bluetooth},{PermissionPolicyConstants.BrowsingTopics}");
            yield return new TestCaseData("example", PermissionPolicyEnabledFilter.AllSites, string.Empty);
            yield return new TestCaseData("example", PermissionPolicyEnabledFilter.ThisSite, PermissionPolicyConstants.BrowsingTopics);
            yield return new TestCaseData("example", PermissionPolicyEnabledFilter.SpecificSites, $"{PermissionPolicyConstants.Bluetooth},{PermissionPolicyConstants.BrowsingTopics}");
            yield return new TestCaseData("exampleone", PermissionPolicyEnabledFilter.AllEnabled, PermissionPolicyConstants.Bluetooth);
            yield return new TestCaseData("exampleone", PermissionPolicyEnabledFilter.AllSites, string.Empty);
            yield return new TestCaseData("exampleone", PermissionPolicyEnabledFilter.ThisSite, string.Empty);
            yield return new TestCaseData("exampleone", PermissionPolicyEnabledFilter.SpecificSites, PermissionPolicyConstants.Bluetooth);
            yield return new TestCaseData("exampletwo", PermissionPolicyEnabledFilter.AllEnabled, PermissionPolicyConstants.BrowsingTopics);
            yield return new TestCaseData("exampletwo", PermissionPolicyEnabledFilter.AllSites, string.Empty);
            yield return new TestCaseData("exampletwo", PermissionPolicyEnabledFilter.ThisSite, PermissionPolicyConstants.BrowsingTopics);
            yield return new TestCaseData("exampletwo", PermissionPolicyEnabledFilter.SpecificSites, PermissionPolicyConstants.BrowsingTopics);
        }
    }
}