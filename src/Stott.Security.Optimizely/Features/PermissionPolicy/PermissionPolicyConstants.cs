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
}
