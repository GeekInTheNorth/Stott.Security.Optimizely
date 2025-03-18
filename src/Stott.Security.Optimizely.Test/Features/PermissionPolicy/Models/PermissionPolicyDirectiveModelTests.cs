using NUnit.Framework;

using Stott.Security.Optimizely.Features.PermissionPolicy;
using Stott.Security.Optimizely.Features.PermissionPolicy.Models;

namespace Stott.Security.Optimizely.Test.Features.PermissionPolicy.Models;

[TestFixture]
public sealed class PermissionPolicyDirectiveModelTests
{
    [Test]
    [TestCase(PermissionPolicyConstants.Accelerometer, PermissionPolicyConstants.Titles.Accelerometer)]
    [TestCase(PermissionPolicyConstants.AmbientLightSensor, PermissionPolicyConstants.Titles.AmbientLightSensor)]
    [TestCase(PermissionPolicyConstants.AttributionReporting, PermissionPolicyConstants.Titles.AttributionReporting)]
    [TestCase(PermissionPolicyConstants.Autoplay, PermissionPolicyConstants.Titles.Autoplay)]
    [TestCase(PermissionPolicyConstants.Bluetooth, PermissionPolicyConstants.Titles.Bluetooth)]
    [TestCase(PermissionPolicyConstants.BrowsingTopics, PermissionPolicyConstants.Titles.BrowsingTopics)]
    [TestCase(PermissionPolicyConstants.Camera, PermissionPolicyConstants.Titles.Camera)]
    [TestCase(PermissionPolicyConstants.ComputePressure, PermissionPolicyConstants.Titles.ComputePressure)]
    [TestCase(PermissionPolicyConstants.DisplayCapture, PermissionPolicyConstants.Titles.DisplayCapture)]
    [TestCase(PermissionPolicyConstants.DocumentDomain, PermissionPolicyConstants.Titles.DocumentDomain)]
    [TestCase(PermissionPolicyConstants.EncryptedMedia, PermissionPolicyConstants.Titles.EncryptedMedia)]
    [TestCase(PermissionPolicyConstants.Fullscreen, PermissionPolicyConstants.Titles.Fullscreen)]
    [TestCase(PermissionPolicyConstants.Gamepad, PermissionPolicyConstants.Titles.Gamepad)]
    [TestCase(PermissionPolicyConstants.Geolocation, PermissionPolicyConstants.Titles.Geolocation)]
    [TestCase(PermissionPolicyConstants.Gyroscope, PermissionPolicyConstants.Titles.Gyroscope)]
    [TestCase(PermissionPolicyConstants.Hid, PermissionPolicyConstants.Titles.Hid)]
    [TestCase(PermissionPolicyConstants.IdentityCredentials, PermissionPolicyConstants.Titles.IdentityCredentials)]
    [TestCase(PermissionPolicyConstants.IdleDetection, PermissionPolicyConstants.Titles.IdleDetection)]
    [TestCase(PermissionPolicyConstants.LocalFonts, PermissionPolicyConstants.Titles.LocalFonts)]
    [TestCase(PermissionPolicyConstants.Magnetometer, PermissionPolicyConstants.Titles.Magnetometer)]
    [TestCase(PermissionPolicyConstants.Microphone, PermissionPolicyConstants.Titles.Microphone)]
    [TestCase(PermissionPolicyConstants.Midi, PermissionPolicyConstants.Titles.Midi)]
    [TestCase(PermissionPolicyConstants.OptCredentials, PermissionPolicyConstants.Titles.OptCredentials)]
    [TestCase(PermissionPolicyConstants.Payment, PermissionPolicyConstants.Titles.Payment)]
    [TestCase(PermissionPolicyConstants.PictureInPicture, PermissionPolicyConstants.Titles.PictureInPicture)]
    [TestCase(PermissionPolicyConstants.PublickeyCredentialsCreate, PermissionPolicyConstants.Titles.PublickeyCredentialsCreate)]
    [TestCase(PermissionPolicyConstants.PublickeyCredentialsGet, PermissionPolicyConstants.Titles.PublickeyCredentialsGet)]
    [TestCase(PermissionPolicyConstants.ScreenWakeLock, PermissionPolicyConstants.Titles.ScreenWakeLock)]
    [TestCase(PermissionPolicyConstants.Serial, PermissionPolicyConstants.Titles.Serial)]
    [TestCase(PermissionPolicyConstants.SpeakerSelection, PermissionPolicyConstants.Titles.SpeakerSelection)]
    [TestCase(PermissionPolicyConstants.StorageAccess, PermissionPolicyConstants.Titles.StorageAccess)]
    [TestCase(PermissionPolicyConstants.Usb, PermissionPolicyConstants.Titles.Usb)]
    [TestCase(PermissionPolicyConstants.WebShare, PermissionPolicyConstants.Titles.WebShare)]
    [TestCase(PermissionPolicyConstants.WindowManagement, PermissionPolicyConstants.Titles.WindowManagement)]
    [TestCase(PermissionPolicyConstants.XrSpatialTracking, PermissionPolicyConstants.Titles.XrSpatialTracking)]
    [TestCase("Invalid", "Invalid")]
    [TestCase(" ", " ")]
    [TestCase("", "")]
    [TestCase(null, null)]
    public void Title_ReturnsCorrectTitleBasedOnDirectiveName(string name, string expectedValue)
    {
        var model = new PermissionPolicyDirectiveModel
        {
            Name = name
        };

        Assert.That(model.Title, Is.EqualTo(expectedValue));
    }

    [Test]
    [TestCase(PermissionPolicyConstants.Accelerometer, PermissionPolicyConstants.Descriptions.Accelerometer)]
    [TestCase(PermissionPolicyConstants.AmbientLightSensor, PermissionPolicyConstants.Descriptions.AmbientLightSensor)]
    [TestCase(PermissionPolicyConstants.AttributionReporting, PermissionPolicyConstants.Descriptions.AttributionReporting)]
    [TestCase(PermissionPolicyConstants.Autoplay, PermissionPolicyConstants.Descriptions.Autoplay)]
    [TestCase(PermissionPolicyConstants.Bluetooth, PermissionPolicyConstants.Descriptions.Bluetooth)]
    [TestCase(PermissionPolicyConstants.BrowsingTopics, PermissionPolicyConstants.Descriptions.BrowsingTopics)]
    [TestCase(PermissionPolicyConstants.Camera, PermissionPolicyConstants.Descriptions.Camera)]
    [TestCase(PermissionPolicyConstants.ComputePressure, PermissionPolicyConstants.Descriptions.ComputePressure)]
    [TestCase(PermissionPolicyConstants.DisplayCapture, PermissionPolicyConstants.Descriptions.DisplayCapture)]
    [TestCase(PermissionPolicyConstants.DocumentDomain, PermissionPolicyConstants.Descriptions.DocumentDomain)]
    [TestCase(PermissionPolicyConstants.EncryptedMedia, PermissionPolicyConstants.Descriptions.EncryptedMedia)]
    [TestCase(PermissionPolicyConstants.Fullscreen, PermissionPolicyConstants.Descriptions.Fullscreen)]
    [TestCase(PermissionPolicyConstants.Gamepad, PermissionPolicyConstants.Descriptions.Gamepad)]
    [TestCase(PermissionPolicyConstants.Geolocation, PermissionPolicyConstants.Descriptions.Geolocation)]
    [TestCase(PermissionPolicyConstants.Gyroscope, PermissionPolicyConstants.Descriptions.Gyroscope)]
    [TestCase(PermissionPolicyConstants.Hid, PermissionPolicyConstants.Descriptions.Hid)]
    [TestCase(PermissionPolicyConstants.IdentityCredentials, PermissionPolicyConstants.Descriptions.IdentityCredentials)]
    [TestCase(PermissionPolicyConstants.IdleDetection, PermissionPolicyConstants.Descriptions.IdleDetection)]
    [TestCase(PermissionPolicyConstants.LocalFonts, PermissionPolicyConstants.Descriptions.LocalFonts)]
    [TestCase(PermissionPolicyConstants.Magnetometer, PermissionPolicyConstants.Descriptions.Magnetometer)]
    [TestCase(PermissionPolicyConstants.Microphone, PermissionPolicyConstants.Descriptions.Microphone)]
    [TestCase(PermissionPolicyConstants.Midi, PermissionPolicyConstants.Descriptions.Midi)]
    [TestCase(PermissionPolicyConstants.OptCredentials, PermissionPolicyConstants.Descriptions.OptCredentials)]
    [TestCase(PermissionPolicyConstants.Payment, PermissionPolicyConstants.Descriptions.Payment)]
    [TestCase(PermissionPolicyConstants.PictureInPicture, PermissionPolicyConstants.Descriptions.PictureInPicture)]
    [TestCase(PermissionPolicyConstants.PublickeyCredentialsCreate, PermissionPolicyConstants.Descriptions.PublickeyCredentialsCreate)]
    [TestCase(PermissionPolicyConstants.PublickeyCredentialsGet, PermissionPolicyConstants.Descriptions.PublickeyCredentialsGet)]
    [TestCase(PermissionPolicyConstants.ScreenWakeLock, PermissionPolicyConstants.Descriptions.ScreenWakeLock)]
    [TestCase(PermissionPolicyConstants.Serial, PermissionPolicyConstants.Descriptions.Serial)]
    [TestCase(PermissionPolicyConstants.SpeakerSelection, PermissionPolicyConstants.Descriptions.SpeakerSelection)]
    [TestCase(PermissionPolicyConstants.StorageAccess, PermissionPolicyConstants.Descriptions.StorageAccess)]
    [TestCase(PermissionPolicyConstants.Usb, PermissionPolicyConstants.Descriptions.Usb)]
    [TestCase(PermissionPolicyConstants.WebShare, PermissionPolicyConstants.Descriptions.WebShare)]
    [TestCase(PermissionPolicyConstants.WindowManagement, PermissionPolicyConstants.Descriptions.WindowManagement)]
    [TestCase(PermissionPolicyConstants.XrSpatialTracking, PermissionPolicyConstants.Descriptions.XrSpatialTracking)]
    [TestCase("Invalid", "")]
    [TestCase(" ", "")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void Description_ReturnsCorrectDescriptionBasedOnDirectiveName(string name, string expectedValue)
    {
        var model = new PermissionPolicyDirectiveModel
        {
            Name = name
        };

        Assert.That(model.Description, Is.EqualTo(expectedValue));
    }
}
