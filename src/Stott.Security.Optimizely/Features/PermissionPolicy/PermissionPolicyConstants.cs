using System;
using System.Collections.Generic;
using System.Linq;

namespace Stott.Security.Optimizely.Features.PermissionPolicy;

public static class PermissionPolicyConstants
{
    public const string PermissionPolicyHeader = "Permissions-Policy";

    public const string Accelerometer = "accelerometer";

    public const string AmbientLightSensor = "ambient-light-sensor";

    public const string AriaNotify = "aria-notify";

    public const string AttributionReporting = "attribution-reporting";

    public const string Autoplay = "autoplay";

    public const string Bluetooth = "bluetooth";

    public const string BrowsingTopics = "browsing-topics";

    public const string Camera = "camera";

    public const string CapturedSurfaceControl = "captured-surface-control";

    public const string ChUaHighEntropyValues = "ch-ua-high-entropy-values";

    public const string ComputePressure = "compute-pressure";

    public const string CrossOriginIsolated = "cross-origin-isolated";

    public const string DeferredFetch = "deferred-fetch";

    public const string DeferredFetchMinimal = "deferred-fetch-minimal";

    public const string DisplayCapture = "display-capture";

    public const string DocumentDomain = "document-domain";

    public const string EncryptedMedia = "encrypted-media";

    public const string Fullscreen = "fullscreen";

    public const string Gamepad = "gamepad";

    public const string Geolocation = "geolocation";

    public const string Gyroscope = "gyroscope";

    public const string Hid = "hid";

    public const string IdentityCredentialsGet = "identity-credentials-get";

    public const string IdleDetection = "idle-detection";

    public const string LanguageDetector = "language-detector";

    public const string LanguageModel = "language-model";

    public const string LocalFonts = "local-fonts";

    public const string LocalNetwork = "local-network";

    public const string LocalNetworkAccess = "local-network-access";

    public const string LoopbackNetwork = "loopback-network";

    public const string Magnetometer = "magnetometer";

    public const string Microphone = "microphone";

    public const string Midi = "midi";

    public const string OnDeviceSpeechRecognition = "on-device-speech-recognition";

    public const string OtpCredentials = "otp-credentials";

    public const string Payment = "payment";

    public const string PictureInPicture = "picture-in-picture";

    public const string PrivateStateTokenIssuance = "private-state-token-issuance";

    public const string PrivateStateTokenRedemption = "private-state-token-redemption";

    public const string PublickeyCredentialsCreate = "publickey-credentials-create";

    public const string PublickeyCredentialsGet = "publickey-credentials-get";

    public const string ScreenWakeLock = "screen-wake-lock";

    public const string Serial = "serial";

    public const string SpeakerSelection = "speaker-selection";

    public const string StorageAccess = "storage-access";

    public const string Summarizer = "summarizer";

    public const string Translator = "translator";

    public const string Usb = "usb";

    public const string WebShare = "web-share";

    public const string WindowManagement = "window-management";

    public const string XrSpatialTracking = "xr-spatial-tracking";

    /// <summary>
    /// The full set of directives understood by this module, ordered by directive name.
    /// </summary>
    /// <remarks>
    /// Aligned with https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Permissions-Policy
    /// </remarks>
    private static readonly IReadOnlyList<PermissionPolicyDirective> Directives =
    [
        new(Accelerometer, "Accelerometer", "Controls whether the site is allowed to gather information about the acceleration of the device through the Accelerometer interface."),
        new(AmbientLightSensor, "Ambient Light Sensor", "Controls whether the site is allowed to gather information about the amount of light in the environment around the device through the AmbientLightSensor interface."),
        new(AriaNotify, "ARIA Notify", "Controls whether the site is allowed to use the ariaNotify() method to fire screen reader announcements."),
        new(AttributionReporting, "Attribution Reporting", "Controls whether the site is allowed to use the Attribution Reporting API.", IsDeprecated: true),
        new(Autoplay, "Autoplay", "Controls whether the site is allowed to autoplay media."),
        new(Bluetooth, "Bluetooth", "Controls whether the site is allowed to access Bluetooth API of the device."),
        new(BrowsingTopics, "Browsing Topics", "Controls whether the site is allowed to access Topics API.", IsDeprecated: true),
        new(Camera, "Camera", "Controls whether the site is allowed to use video input devices such as the device camera."),
        new(CapturedSurfaceControl, "Captured Surface Control", "Controls whether the site is allowed to use the Captured Surface Control API to scroll and change the zoom level of a captured display surface."),
        new(ChUaHighEntropyValues, "High Entropy User Agent Values", "Controls whether the site is allowed to use the NavigatorUAData.getHighEntropyValues() method to retrieve high entropy user agent data."),
        new(ComputePressure, "Compute Pressure", "Controls whether the site is allowed to access the Pressure API."),
        new(CrossOriginIsolated, "Cross-Origin Isolated", "Controls whether the site can be treated as cross-origin isolated."),
        new(DeferredFetch, "Deferred Fetch", "Controls the allocation of the top level origin's fetchLater() quota."),
        new(DeferredFetchMinimal, "Deferred Fetch (Minimal)", "Controls the allocation of the shared cross-origin subframe fetchLater() quota."),
        new(DisplayCapture, "Display Capture", "Controls whether the site is allowed to access the Screen Capture API."),
        new(DocumentDomain, "Document Domain", "Controls whether the site is allowed to set the Document Domain.", IsDeprecated: true),
        new(EncryptedMedia, "Encrypted Media", "Controls whether the site is allowed to use the Encrypted Media Extensions API."),
        new(Fullscreen, "Fullscreen", "Controls whether the site is allowed to request the use of the full screen."),
        new(Gamepad, "Gamepad", "Controls whether the site is allowed to access the Gamepad API."),
        new(Geolocation, "Geolocation", "Controls whether the site is allowed to access the Geolocation interface."),
        new(Gyroscope, "Gyroscope", "Controls whether the site is allowed to access the Gyroscope interface."),
        new(Hid, "HID", "Controls whether the site is allowed to use the WebHID API to connect to uncommon or exotic human interface devices such as alternative keyboards or gamepads."),
        new(IdentityCredentialsGet, "Identity Credentials", "Controls whether the site is allowed to use the Federated Credential Management API (FedCM), and more specifically the navigator.credentials.get() method with an identity option."),
        new(IdleDetection, "Idle Detection", "Controls whether the site is allowed to use the Idle Detection API to detect when users are interacting with their devices. This can be used to report the user as available or away in chat interfaces."),
        new(LanguageDetector, "Language Detector", "Controls whether the site is allowed to access the language detection functionality of the Translator and Language Detector APIs."),
        new(LanguageModel, "Language Model", "Controls whether the site is allowed to access the Prompt API."),
        new(LocalFonts, "Local Fonts", "Controls whether the site is allowed to gather data on the user's locally-installed fonts."),
        new(LocalNetwork, "Local Network", "Controls whether the site is allowed to make network requests to local addresses."),
        new(LocalNetworkAccess, "Local Network Access", "Controls whether the site is allowed to make network requests to local and loopback addresses."),
        new(LoopbackNetwork, "Loopback Network", "Controls whether the site is allowed to make network requests to loopback addresses."),
        new(Magnetometer, "Magnetometer", "Controls whether the site is allowed to gather information about the orientation of the device through the Magnetometer interface."),
        new(Microphone, "Microphone", "Controls whether the site is allowed to use audio input devices such as a device microphone."),
        new(Midi, "MIDI", "Controls whether the site is allowed to use the Web MIDI API."),
        new(OnDeviceSpeechRecognition, "On-Device Speech Recognition", "Controls whether the site is allowed to access the on-device speech recognition functionality of the Web Speech API."),
        new(OtpCredentials, "OTP Credentials", "Controls whether the site is allowed to use the WebOTP API to request a one-time password (OTP) from a specially-formatted SMS message sent by the website's server."),
        new(Payment, "Payment", "Controls whether the site is allowed to use the Payment Request API."),
        new(PictureInPicture, "Picture in Picture", "Controls whether the site is allowed to play a video in a Picture-in-Picture mode."),
        new(PrivateStateTokenIssuance, "Private State Token Issuance", "Controls whether the site is allowed to use private state token issuance operations."),
        new(PrivateStateTokenRedemption, "Private State Token Redemption", "Controls whether the site is allowed to use private state token redemption and send redemption record operations."),
        new(PublickeyCredentialsCreate, "Create Public Key Credentials", "Controls whether the site is allowed to use the Web Authentication API to create new credentials."),
        new(PublickeyCredentialsGet, "Retrieve Public Key Credentials", "Controls whether the site is allowed to use the Web Authentication API to retrieve credentials."),
        new(ScreenWakeLock, "Screen Wake Lock", "Controls whether the site is allowed to use Screen Wake Lock API to indicate that the device should not dim or turn off the screen."),
        new(Serial, "Serial", "Controls whether the site is allowed to use the Web Serial API to communicate with serial devices."),
        new(SpeakerSelection, "Speaker Selection", "Controls whether the site is allowed to enumerate and select audio output devices."),
        new(StorageAccess, "Storage Access", "Controls whether third party content (i.e. embedded in an iframe) is allowed to use the Storage Access API to request access to unpartitioned cookies."),
        new(Summarizer, "Summarizer", "Controls whether the site is allowed to access the Summarizer API."),
        new(Translator, "Translator", "Controls whether the site is allowed to access the translation functionality of the Translator and Language Detector APIs."),
        new(Usb, "USB", "Controls whether the site is allowed to use the WebUSB API."),
        new(WebShare, "Web Share", "Controls whether the site is allowed to use Web Share API to share text, links, images, and other content to arbitrary destinations of the user's choice."),
        new(WindowManagement, "Window Management", "Controls whether the site is allowed to use the Window Management API to manage windows on multiple displays."),
        new(XrSpatialTracking, "XR Spatial Tracking", "Controls whether the site is allowed to use the WebXR Device API.")
    ];

    private const string LegacyIdentityCredentials = "identity-credentials";

    private const string LegacyOtpCredentials = "opt-credentials";

    /// <summary>
    /// Maps directive names which this module has previously used onto their current equivalent.
    /// Consulted when importing settings which may have been exported by an earlier version.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, string> LegacyDirectiveNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { LegacyIdentityCredentials, IdentityCredentialsGet },
        { LegacyOtpCredentials, OtpCredentials }
    };

    private static readonly IReadOnlyDictionary<string, PermissionPolicyDirective> DirectiveLookUp =
        Directives.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Every directive name recognised by this module, including those which have been deprecated.
    /// Deprecated directives remain valid so that existing configurations can still be edited or removed.
    /// </summary>
    public static List<string> AllDirectives => Directives.Select(x => x.Name).ToList();

    /// <summary>
    /// The directives presented to a user by default, excluding those which have been deprecated.
    /// </summary>
    public static IReadOnlyList<PermissionPolicyDirective> DefaultDirectiveDefinitions { get; } = Directives.Where(x => !x.IsDeprecated).ToList();

    /// <summary>
    /// The directive names presented to a user by default, excluding those which have been deprecated.
    /// </summary>
    public static List<string> DefaultDirectives => DefaultDirectiveDefinitions.Select(x => x.Name).ToList();

    /// <summary>
    /// Translates a directive name which this module previously used into its current equivalent.
    /// Any name which is not a known legacy name is returned unaltered.
    /// </summary>
    public static string? ResolveLegacyName(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && LegacyDirectiveNames.TryGetValue(name, out var currentName) ? currentName : name;
    }

    /// <summary>
    /// Retrieves the metadata for a given directive name, returning null when the name is not recognised.
    /// </summary>
    public static PermissionPolicyDirective? Find(string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && DirectiveLookUp.TryGetValue(name, out var directive) ? directive : null;
    }
}
