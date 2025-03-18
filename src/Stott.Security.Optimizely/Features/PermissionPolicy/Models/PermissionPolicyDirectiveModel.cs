using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Stott.Security.Optimizely.Features.PermissionPolicy.Models;

public sealed class PermissionPolicyDirectiveModel
{
    public string? Name { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PermissionPolicyEnabledState EnabledState { get; set; }

    public List<PermissionPolicyUrl> Sources { get; set; } = new List<PermissionPolicyUrl>();

    public string? Title
    {
        get
        {
            return Name switch
            {
                PermissionPolicyConstants.Accelerometer => PermissionPolicyConstants.Titles.Accelerometer,
                PermissionPolicyConstants.AmbientLightSensor => PermissionPolicyConstants.Titles.AmbientLightSensor,
                PermissionPolicyConstants.AttributionReporting => PermissionPolicyConstants.Titles.AttributionReporting,
                PermissionPolicyConstants.Autoplay => PermissionPolicyConstants.Titles.Autoplay,
                PermissionPolicyConstants.Bluetooth => PermissionPolicyConstants.Titles.Bluetooth,
                PermissionPolicyConstants.BrowsingTopics => PermissionPolicyConstants.Titles.BrowsingTopics,
                PermissionPolicyConstants.Camera => PermissionPolicyConstants.Titles.Camera,
                PermissionPolicyConstants.ComputePressure => PermissionPolicyConstants.Titles.ComputePressure,
                PermissionPolicyConstants.DisplayCapture => PermissionPolicyConstants.Titles.DisplayCapture,
                PermissionPolicyConstants.DocumentDomain => PermissionPolicyConstants.Titles.DocumentDomain,
                PermissionPolicyConstants.EncryptedMedia => PermissionPolicyConstants.Titles.EncryptedMedia,
                PermissionPolicyConstants.Fullscreen => PermissionPolicyConstants.Titles.Fullscreen,
                PermissionPolicyConstants.Gamepad => PermissionPolicyConstants.Titles.Gamepad,
                PermissionPolicyConstants.Geolocation => PermissionPolicyConstants.Titles.Geolocation,
                PermissionPolicyConstants.Gyroscope => PermissionPolicyConstants.Titles.Gyroscope,
                PermissionPolicyConstants.Hid => PermissionPolicyConstants.Titles.Hid,
                PermissionPolicyConstants.IdentityCredentials => PermissionPolicyConstants.Titles.IdentityCredentials,
                PermissionPolicyConstants.IdleDetection => PermissionPolicyConstants.Titles.IdleDetection,
                PermissionPolicyConstants.LocalFonts => PermissionPolicyConstants.Titles.LocalFonts,
                PermissionPolicyConstants.Magnetometer => PermissionPolicyConstants.Titles.Magnetometer,
                PermissionPolicyConstants.Microphone => PermissionPolicyConstants.Titles.Microphone,
                PermissionPolicyConstants.Midi => PermissionPolicyConstants.Titles.Midi,
                PermissionPolicyConstants.OptCredentials => PermissionPolicyConstants.Titles.OptCredentials,
                PermissionPolicyConstants.Payment => PermissionPolicyConstants.Titles.Payment,
                PermissionPolicyConstants.PictureInPicture => PermissionPolicyConstants.Titles.PictureInPicture,
                PermissionPolicyConstants.PublickeyCredentialsCreate => PermissionPolicyConstants.Titles.PublickeyCredentialsCreate,
                PermissionPolicyConstants.PublickeyCredentialsGet => PermissionPolicyConstants.Titles.PublickeyCredentialsGet,
                PermissionPolicyConstants.ScreenWakeLock => PermissionPolicyConstants.Titles.ScreenWakeLock,
                PermissionPolicyConstants.Serial => PermissionPolicyConstants.Titles.Serial,
                PermissionPolicyConstants.SpeakerSelection => PermissionPolicyConstants.Titles.SpeakerSelection,
                PermissionPolicyConstants.StorageAccess => PermissionPolicyConstants.Titles.StorageAccess,
                PermissionPolicyConstants.Usb => PermissionPolicyConstants.Titles.Usb,
                PermissionPolicyConstants.WebShare => PermissionPolicyConstants.Titles.WebShare,
                PermissionPolicyConstants.WindowManagement => PermissionPolicyConstants.Titles.WindowManagement,
                PermissionPolicyConstants.XrSpatialTracking => PermissionPolicyConstants.Titles.XrSpatialTracking,
                _ => Name
            };
        }
    }

    public string? Description
    {
        get
        {
            return Name switch
            {
                PermissionPolicyConstants.Accelerometer => PermissionPolicyConstants.Descriptions.Accelerometer,
                PermissionPolicyConstants.AmbientLightSensor => PermissionPolicyConstants.Descriptions.AmbientLightSensor,
                PermissionPolicyConstants.AttributionReporting => PermissionPolicyConstants.Descriptions.AttributionReporting,
                PermissionPolicyConstants.Autoplay => PermissionPolicyConstants.Descriptions.Autoplay,
                PermissionPolicyConstants.Bluetooth => PermissionPolicyConstants.Descriptions.Bluetooth,
                PermissionPolicyConstants.BrowsingTopics => PermissionPolicyConstants.Descriptions.BrowsingTopics,
                PermissionPolicyConstants.Camera => PermissionPolicyConstants.Descriptions.Camera,
                PermissionPolicyConstants.ComputePressure => PermissionPolicyConstants.Descriptions.ComputePressure,
                PermissionPolicyConstants.DisplayCapture => PermissionPolicyConstants.Descriptions.DisplayCapture,
                PermissionPolicyConstants.DocumentDomain => PermissionPolicyConstants.Descriptions.DocumentDomain,
                PermissionPolicyConstants.EncryptedMedia => PermissionPolicyConstants.Descriptions.EncryptedMedia,
                PermissionPolicyConstants.Fullscreen => PermissionPolicyConstants.Descriptions.Fullscreen,
                PermissionPolicyConstants.Gamepad => PermissionPolicyConstants.Descriptions.Gamepad,
                PermissionPolicyConstants.Geolocation => PermissionPolicyConstants.Descriptions.Geolocation,
                PermissionPolicyConstants.Gyroscope => PermissionPolicyConstants.Descriptions.Gyroscope,
                PermissionPolicyConstants.Hid => PermissionPolicyConstants.Descriptions.Hid,
                PermissionPolicyConstants.IdentityCredentials => PermissionPolicyConstants.Descriptions.IdentityCredentials,
                PermissionPolicyConstants.IdleDetection => PermissionPolicyConstants.Descriptions.IdleDetection,
                PermissionPolicyConstants.LocalFonts => PermissionPolicyConstants.Descriptions.LocalFonts,
                PermissionPolicyConstants.Magnetometer => PermissionPolicyConstants.Descriptions.Magnetometer,
                PermissionPolicyConstants.Microphone => PermissionPolicyConstants.Descriptions.Microphone,
                PermissionPolicyConstants.Midi => PermissionPolicyConstants.Descriptions.Midi,
                PermissionPolicyConstants.OptCredentials => PermissionPolicyConstants.Descriptions.OptCredentials,
                PermissionPolicyConstants.Payment => PermissionPolicyConstants.Descriptions.Payment,
                PermissionPolicyConstants.PictureInPicture => PermissionPolicyConstants.Descriptions.PictureInPicture,
                PermissionPolicyConstants.PublickeyCredentialsCreate => PermissionPolicyConstants.Descriptions.PublickeyCredentialsCreate,
                PermissionPolicyConstants.PublickeyCredentialsGet => PermissionPolicyConstants.Descriptions.PublickeyCredentialsGet,
                PermissionPolicyConstants.ScreenWakeLock => PermissionPolicyConstants.Descriptions.ScreenWakeLock,
                PermissionPolicyConstants.Serial => PermissionPolicyConstants.Descriptions.Serial,
                PermissionPolicyConstants.SpeakerSelection => PermissionPolicyConstants.Descriptions.SpeakerSelection,
                PermissionPolicyConstants.StorageAccess => PermissionPolicyConstants.Descriptions.StorageAccess,
                PermissionPolicyConstants.Usb => PermissionPolicyConstants.Descriptions.Usb,
                PermissionPolicyConstants.WebShare => PermissionPolicyConstants.Descriptions.WebShare,
                PermissionPolicyConstants.WindowManagement => PermissionPolicyConstants.Descriptions.WindowManagement,
                PermissionPolicyConstants.XrSpatialTracking => PermissionPolicyConstants.Descriptions.XrSpatialTracking,
                _ => string.Empty
            };
        }
    }
}
