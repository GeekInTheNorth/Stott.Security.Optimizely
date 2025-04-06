using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public static class PermissionPolicyConstants
{
    public const string PermissionPolicyHeader = "Permissions-Policy";

    public const string Accelerometer = "accelerometer";

    public const string AmbientLightSensor = "ambient-light-sensor";

    public const string AttributionReporting = "attribution-reporting";

    public const string Autoplay = "autoplay";

    public const string Bluetooth = "bluetooth";

    public const string BrowsingTopics = "browsing-topics";

    public const string Camera = "camera";

    public const string ComputePressure = "compute-pressure";

    public const string DisplayCapture = "display-capture";

    public const string DocumentDomain = "document-domain";

    public const string EncryptedMedia = "encrypted-media";

    public const string Fullscreen = "fullscreen";

    public const string Gamepad = "gamepad";

    public const string Geolocation = "geolocation";

    public const string Gyroscope = "gyroscope";

    public const string Hid = "hid";

    public const string IdentityCredentials = "identity-credentials";

    public const string IdleDetection = "idle-detection";

    public const string LocalFonts = "local-fonts";

    public const string Magnetometer = "magnetometer";

    public const string Microphone = "microphone";

    public const string Midi = "midi";

    public const string OptCredentials = "opt-credentials";

    public const string Payment = "payment";

    public const string PictureInPicture = "picture-in-picture";

    public const string PublickeyCredentialsCreate = "publickey-credentials-create";

    public const string PublickeyCredentialsGet = "publickey-credentials-get";

    public const string ScreenWakeLock = "screen-wake-lock";

    public const string Serial = "serial";

    public const string SpeakerSelection = "speaker-selection";

    public const string StorageAccess = "storage-access";

    public const string Usb = "usb";

    public const string WebShare = "web-share";

    public const string WindowManagement = "window-management";

    public const string XrSpatialTracking = "xr-spatial-tracking";

    public static List<string> AllDirectives => new()
    {
        Accelerometer,
        AmbientLightSensor,
        AttributionReporting,
        Autoplay,
        Bluetooth,
        BrowsingTopics,
        Camera,
        ComputePressure,
        DisplayCapture,
        DocumentDomain,
        EncryptedMedia,
        Fullscreen,
        Gamepad,
        Geolocation,
        Gyroscope,
        Hid,
        IdentityCredentials,
        IdleDetection,
        LocalFonts,
        Magnetometer,
        Microphone,
        Midi,
        OptCredentials,
        Payment,
        PictureInPicture,
        PublickeyCredentialsCreate,
        PublickeyCredentialsGet,
        ScreenWakeLock,
        Serial,
        SpeakerSelection,
        StorageAccess,
        Usb,
        WebShare,
        WindowManagement,
        XrSpatialTracking
    };

    public static class Titles
    {
        public const string Accelerometer = "Accelerometer";
        public const string AmbientLightSensor = "Ambient Light Sensor";
        public const string AttributionReporting = "Attribution Reporting";
        public const string Autoplay = "Autoplay";
        public const string Bluetooth = "Bluetooth";
        public const string BrowsingTopics = "Browsing Topics";
        public const string Camera = "Camera";
        public const string ComputePressure = "Compute Pressure";
        public const string DisplayCapture = "Display Capture";
        public const string DocumentDomain = "Document Domain";
        public const string EncryptedMedia = "Encrypted Media";
        public const string Fullscreen = "Fullscreen";
        public const string Gamepad = "Gamepad";
        public const string Geolocation = "Geolocation";
        public const string Gyroscope = "Gyroscope";
        public const string Hid = "HID";
        public const string IdentityCredentials = "Identity Credentials";
        public const string IdleDetection = "Idle Detection";
        public const string LocalFonts = "Local Fonts";
        public const string Magnetometer = "Magnetometer";
        public const string Microphone = "Microphone";
        public const string Midi = "MIDI";
        public const string OptCredentials = "Opt Credentials";
        public const string Payment = "Payment";
        public const string PictureInPicture = "Picture in Picture";
        public const string PublickeyCredentialsCreate = "Create Public Key Credentials";
        public const string PublickeyCredentialsGet = "Retrieve Public Key Credentials";
        public const string ScreenWakeLock = "Screen Wake Lock";
        public const string Serial = "Serial";
        public const string SpeakerSelection = "Speaker Selection";
        public const string StorageAccess = "Storage Access";
        public const string Usb = "USB";
        public const string WebShare = "Web Share";
        public const string WindowManagement = "Window Management";
        public const string XrSpatialTracking = "XR Spatial Tracking";
    }

    public static class Descriptions
    {
        public const string Accelerometer = "Controls whether the site is allowed to gather information about the acceleration of the device through the Accelerometer interface.";
        public const string AmbientLightSensor = "Controls whether the site is allowed to gather information about the amount of light in the environment around the device through the AmbientLightSensor interface.";
        public const string AttributionReporting = "Controls whether the site is allowed to use the Attribution Reporting API.";
        public const string Autoplay = "Controls whether the site is allowed to autoplay media.";
        public const string Bluetooth = "Controls whether the site is allowed to access Bluetooth API of the device.";
        public const string BrowsingTopics = "Controls whether the site is allowed to access Topics API.";
        public const string Camera = "Controls whether the site is allowed to use video input devices such as the device camera.";
        public const string ComputePressure = "Controls whether the site is allowed to access the Pressure API.";
        public const string DisplayCapture = "Controls whether the site is allowed to access the Screen Capture API.";
        public const string DocumentDomain = "Controls whether the site is allowed to set the Document Domain.";
        public const string EncryptedMedia = "Controls whether the site is allowed to use the Encrypted Media Extensions API.";
        public const string Fullscreen = "Controls whether the site is allowed to request the use of the full screen.";
        public const string Gamepad = "Controls whether the site is allowed to access the Gamepad API.";
        public const string Geolocation = "Controls whether the site is allowed to access the Geolocation interface.";
        public const string Gyroscope = "Controls whether the site is allowed to access the Gyroscope interface.";
        public const string Hid = "Controls whether the site is allowed to use the WebHID API to connect to uncommon or exotic human interface devices such as alternative keyboards or gamepads.";
        public const string IdentityCredentials = "Controls whether the site is allowed to use the Federated Credential Management API (FedCM), and more specifically the navigator.credentials.get() method with an identity option.";
        public const string IdleDetection = "Controls whether the site is allowed to use the Idle Detection API to detect when users are interacting with their devices. This can be used to report the user as available or away in chat interfaces.";
        public const string LocalFonts = "Controls whether the site is allowed to gather data on the user's locally-installed fonts.";
        public const string Magnetometer = "Controls whether the site is allowed to gather information about the orientation of the device through the Magnetometer interface.";
        public const string Microphone = "Controls whether the site is allowed to use audio input devices such as a device microphone.";
        public const string Midi = "Controls whether the site is allowed to use the Web MIDI API.";
        public const string OptCredentials = "Controls whether the site is allowed to use the WebOTP API to request a one-time password (OTP) from a specially-formatted SMS message sent by the website's server.";
        public const string Payment = "Controls whether the site is allowed to use the Payment Request API.";
        public const string PictureInPicture = "Controls whether the site is allowed to play a video in a Picture-in-Picture mode.";
        public const string PublickeyCredentialsCreate = "Controls whether the site is allowed to use the Web Authentication API to create new credentials.";
        public const string PublickeyCredentialsGet = "Controls whether the site is allowed to use the Web Authentication API to retrieve credentials.";
        public const string ScreenWakeLock = "Controls whether the site is allowed to use Screen Wake Lock API to indicate that the device should not dim or turn off the screen.";
        public const string Serial = "Controls whether the site is allowed to use the Web Serial API to communicate with serial devices.";
        public const string SpeakerSelection = "Controls whether the site is allowed to enumerate and select audio output devices.";
        public const string StorageAccess = "Controls whether third party content (i.e. embedded in an iframe) is allowed to use the Storage Access API to request access to unpartitioned cookies.";
        public const string Usb = "Controls whether the site is allowed to use the WebUSB API.";
        public const string WebShare = "Controls whether the site is allowed to use Web Share API to share text, links, images, and other content to arbitrary destinations of the user's choice.";
        public const string WindowManagement = "Controls whether the site is allowed to use the Window Management API to manage windows on multiple displays.";
        public const string XrSpatialTracking = "Controls whether the site is allowed to use the WebXR Device API.";
    }
}