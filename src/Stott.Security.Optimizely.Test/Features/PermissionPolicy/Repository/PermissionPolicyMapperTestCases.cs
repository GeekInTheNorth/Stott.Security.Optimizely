using System.Collections.Generic;

using NUnit.Framework;

using Stott.Security.Optimizely.Features.PermissionPolicy;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy.Repository;

public static class PermissionPolicyMapperTestCases
{
    public static IEnumerable<TestCaseData> ToPolicyFragmentTestCases
    {
        get
        {
            yield return new TestCaseData(PermissionPolicyConstants.Accelerometer.ToString(), PermissionPolicyEnabledState.None.ToString(), string.Empty, "accelerometer=()");
            yield return new TestCaseData(PermissionPolicyConstants.AmbientLightSensor.ToString(), PermissionPolicyEnabledState.All.ToString(), string.Empty, "ambient-light-sensor=*");
            yield return new TestCaseData(PermissionPolicyConstants.Autoplay.ToString(), PermissionPolicyEnabledState.ThisSite.ToString(), string.Empty, "autoplay=(self)");
            yield return new TestCaseData(PermissionPolicyConstants.Bluetooth.ToString(), PermissionPolicyEnabledState.ThisAndSpecificSites.ToString(), string.Empty, "bluetooth=(self )");
            yield return new TestCaseData(PermissionPolicyConstants.Camera.ToString(), PermissionPolicyEnabledState.ThisAndSpecificSites.ToString(), "https://www.example.com", "camera=(self \"https://www.example.com\")");
            yield return new TestCaseData(PermissionPolicyConstants.Fullscreen.ToString(), PermissionPolicyEnabledState.ThisAndSpecificSites.ToString(), "https://www.example.com,https://www.test.com", "fullscreen=(self \"https://www.example.com\" \"https://www.test.com\")");
            yield return new TestCaseData(PermissionPolicyConstants.Gamepad.ToString(), PermissionPolicyEnabledState.SpecificSites.ToString(), string.Empty, "gamepad=()");
            yield return new TestCaseData(PermissionPolicyConstants.Geolocation.ToString(), PermissionPolicyEnabledState.SpecificSites.ToString(), "https://www.example.com", "geolocation=(\"https://www.example.com\")");
            yield return new TestCaseData(PermissionPolicyConstants.Gyroscope.ToString(), PermissionPolicyEnabledState.SpecificSites.ToString(), "https://www.example.com,https://www.test.com", "gyroscope=(\"https://www.example.com\" \"https://www.test.com\")");
        }
    }
}