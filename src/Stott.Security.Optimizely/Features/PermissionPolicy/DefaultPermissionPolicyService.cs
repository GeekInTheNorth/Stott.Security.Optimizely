using System;
using System.Collections.Generic;
using System.Linq;

namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public sealed class DefaultPermissionPolicyService : IPermissionPolicyService
{
    public IList<PermissionPolicyDirectiveModel> GetAll(string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        return PermissionPolicyConstants.AllDirectives.Select(x => new PermissionPolicyDirectiveModel
        {
            Name = x,
            Title = GetTitle(x),
            Description = GetDescription(x),
            EnabledState = GetEnabledState(x),
            Sources = GetSources(x)
        }).Where(x => IsMatch(x, sourceFilter, enabledFilter)) .ToList();
    }

    private static bool IsMatch(PermissionPolicyDirectiveModel model, string? sourceFilter, PermissionPolicyEnabledFilter enabledFilter)
    {
        if (!string.IsNullOrWhiteSpace(sourceFilter))
        {
            if (model.Sources is null || !model.Sources.Any(x => x.Url?.Contains(sourceFilter) ?? false))
            {
                return false;
            }
        }

        return enabledFilter switch
        {
            PermissionPolicyEnabledFilter.AllEnabled => model.EnabledState != PermissionPolicyEnabledState.None,
            PermissionPolicyEnabledFilter.Disabled => model.EnabledState == PermissionPolicyEnabledState.None,
            PermissionPolicyEnabledFilter.AllSites => model.EnabledState == PermissionPolicyEnabledState.All,
            PermissionPolicyEnabledFilter.ThisSite => model.EnabledState == PermissionPolicyEnabledState.ThisSite,
            PermissionPolicyEnabledFilter.ThisAndSpecificSites => model.EnabledState == PermissionPolicyEnabledState.ThisAndSpecificSites,
            PermissionPolicyEnabledFilter.SpecificSites => model.EnabledState == PermissionPolicyEnabledState.SpecificSites,
            _ => true,
        };
    }

    private static PermissionPolicyEnabledState GetEnabledState(string name)
    {
        return name switch
        {
            PermissionPolicyConstants.Fullscreen => PermissionPolicyEnabledState.ThisSite,
            PermissionPolicyConstants.Geolocation => PermissionPolicyEnabledState.ThisAndSpecificSites,
            PermissionPolicyConstants.Autoplay => PermissionPolicyEnabledState.SpecificSites,
            _ => PermissionPolicyEnabledState.None
        };
    }

    private static List<PermissionPolicyUrl> GetSources(string name)
    {
        return name switch
        {
            PermissionPolicyConstants.Geolocation => new List<PermissionPolicyUrl> 
            { 
                new PermissionPolicyUrl { Id = Guid.NewGuid(), Url = "https://www.example.com" },
                new PermissionPolicyUrl { Id = Guid.NewGuid(), Url = "https://www.google.com" }
            },
            PermissionPolicyConstants.Autoplay => new List<PermissionPolicyUrl>
            {
                new PermissionPolicyUrl { Id = Guid.NewGuid(), Url = "https://www.vimeo.com" }
            },
            _ => new List<PermissionPolicyUrl>(0)
        };
    }

    private static string GetTitle(string name)
    {
        return name switch
        {
            PermissionPolicyConstants.Accelerometer => "Accelerometer",
            PermissionPolicyConstants.AmbientLightSensor => "Ambient Light Sensor",
            PermissionPolicyConstants.AttributionReporting => "Attribution Reporting",
            PermissionPolicyConstants.Autoplay => "Autoplay",
            PermissionPolicyConstants.Bluetooth => "Bluetooth",
            PermissionPolicyConstants.BrowsingTopics => "Browsing Topics",
            PermissionPolicyConstants.Camera => "Camera",
            PermissionPolicyConstants.ComputePressure => "Compute Pressure",
            PermissionPolicyConstants.DisplayCapture => "Display Capture",
            PermissionPolicyConstants.DocumentDomain => "Document Domain",
            PermissionPolicyConstants.EncryptedMedia => "Encrypted Media",
            PermissionPolicyConstants.Fullscreen => "Fullscreen",
            PermissionPolicyConstants.Gamepad => "Gamepad",
            PermissionPolicyConstants.Geolocation => "Geolocation",
            PermissionPolicyConstants.Gyroscope => "Gyroscope",
            PermissionPolicyConstants.Hid => "HID",
            PermissionPolicyConstants.IdentityCredentials => "Identity Credentials",
            PermissionPolicyConstants.IdleDetection => "Idle Detection",
            PermissionPolicyConstants.LocalFonts => "Local Fonts",
            PermissionPolicyConstants.Magnetometer => "Magnetometer",
            PermissionPolicyConstants.Microphone => "Microphone",
            PermissionPolicyConstants.Midi => "MIDI",
            PermissionPolicyConstants.OptCredentials => "Opt Credentials",
            PermissionPolicyConstants.Payment => "Payment",
            PermissionPolicyConstants.PictureInPicture => "Picture in Picture",
            PermissionPolicyConstants.PublickeyCredentialsCreate => "Create Public Key Credentials",
            PermissionPolicyConstants.PublickeyCredentialsGet => "Retrieve Public Key Credentials",
            PermissionPolicyConstants.ScreenWakeLock => "Screen Wake Lock",
            PermissionPolicyConstants.Serial => "Serial",
            PermissionPolicyConstants.SpeakerSelection => "Speaker Selection",
            PermissionPolicyConstants.StorageAccess => "Storage Access",
            PermissionPolicyConstants.Usb => "USB",
            PermissionPolicyConstants.WebShare => "Web Share",
            PermissionPolicyConstants.WindowManagement => "Window Management",
            PermissionPolicyConstants.XrSpatialTracking => "XR Spatial Tracking",
            _ => name
        };
    }

    private static string GetDescription(string name)
    {
        return name switch
        {
            PermissionPolicyConstants.Accelerometer => "Controls whether the site is allowed to gather information about the acceleration of the device through the Accelerometer interface.",
            PermissionPolicyConstants.AmbientLightSensor => "Controls whether the site is allowed to gather information about the amount of light in the environment around the device through the AmbientLightSensor interface.",
            PermissionPolicyConstants.AttributionReporting => "Controls whether the site is allowed to use the Attribution Reporting API.",
            PermissionPolicyConstants.Autoplay => "Controls whether the site is allowed to autoplay media.",
            PermissionPolicyConstants.Bluetooth => "Controls whether the site is allowed to access Bluetooth API of the device.",
            PermissionPolicyConstants.BrowsingTopics => "Controls whether the site is allowed to access Topics API.",
            PermissionPolicyConstants.Camera => "Controls whether the site is allowed to use video input devices such as the device camera.",
            PermissionPolicyConstants.ComputePressure => "Controls whether the site is allowed to access the Pressure API.",
            PermissionPolicyConstants.DisplayCapture => "Controls whether the site is allowed to access the Screen Capture API.",
            PermissionPolicyConstants.DocumentDomain => "Controls whether the site is allowed to set the Document Domain.",
            PermissionPolicyConstants.EncryptedMedia => "Controls whether the site is allowed to use the Encrypted Media Extensions API.",
            PermissionPolicyConstants.Fullscreen => "Controls whether the site is allowed to request the use of the full screen.",
            PermissionPolicyConstants.Gamepad => "Controls whether the site is allowed to access the Gamepad API.",
            PermissionPolicyConstants.Geolocation => "Controls whether the site is allowed to access the Geolocation interface.",
            PermissionPolicyConstants.Gyroscope => "Controls whether the site is allowed to access the Gyroscope interface.",
            PermissionPolicyConstants.Hid => "Controls whether the site is allowed to use the WebHID API to connect to uncommon or exotic human interface devices such as alternative keyboards or gamepads.",
            PermissionPolicyConstants.IdentityCredentials => "Controls whether the site is allowed to use the Federated Credential Management API (FedCM), and more specifically the navigator.credentials.get() method with an identity option.",
            PermissionPolicyConstants.IdleDetection => "Controls whether the site is allowed to use the Idle Detection API to detect when users are interacting with their devices. This can be used to report the user as available or away in chat interfaces.",
            PermissionPolicyConstants.LocalFonts => "Controls whether the site is allowed to gather data on the user's locally-installed fonts.",
            PermissionPolicyConstants.Magnetometer => "Controls whether the site is allowed to gather information about the orientation of the device through the Magnetometer interface.",
            PermissionPolicyConstants.Microphone => "Controls whether the site is allowed to use audio input devices such as a device microphone.",
            PermissionPolicyConstants.Midi => "Controls whether the site is allowed to use the Web MIDI API.",
            PermissionPolicyConstants.OptCredentials => "Controls whether the site is allowed to use the WebOTP API to request a one-time password (OTP) from a specially-formatted SMS message sent by the website's server.",
            PermissionPolicyConstants.Payment => "Controls whether the site is allowed to use the Payment Request API.",
            PermissionPolicyConstants.PictureInPicture => "Controls whether the site is allowed to play a video in a Picture-in-Picture mode.",
            PermissionPolicyConstants.PublickeyCredentialsCreate => "Controls whether the site is allowed to use the Web Authentication API to create new credentials.",
            PermissionPolicyConstants.PublickeyCredentialsGet => "Controls whether the site is allowed to use the Web Authentication API to retrieve credentials.",
            PermissionPolicyConstants.ScreenWakeLock => "Controls whether the site is allowed to use Screen Wake Lock API to indicate that the device should not dim or turn off the screen.",
            PermissionPolicyConstants.Serial => "Controls whether the site is allowed to use the Web Serial API to communicate with serial devices.",
            PermissionPolicyConstants.SpeakerSelection => "Controls whether the site is allowed to enumerate and select audio output devices.",
            PermissionPolicyConstants.StorageAccess => "Controls whether third party content (i.e. embedded in an iframe) is allowed to use the Storage Access API to request access to unpartitioned cookies.",
            PermissionPolicyConstants.Usb => "Controls whether the site is allowed to use the WebUSB API.",
            PermissionPolicyConstants.WebShare => "Controls whether the site is allowed to use Web Share API to share text, links, images, and other content to arbitrary destinations of the user's choice.",
            PermissionPolicyConstants.WindowManagement => "Controls whether the site is allowed to use the Window Management API to manage windows on multiple displays.",
            PermissionPolicyConstants.XrSpatialTracking => "Controls whether the site is allowed to use the WebXR Device API.",
            _ => name
        };
    }
}
