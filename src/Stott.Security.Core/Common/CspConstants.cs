namespace Stott.Security.Core.Common;

using System.Collections.Generic;

public static class CspConstants
{
    public const string AuthorizationPolicy = "StottSecurityCore";

    public static string LogPrefix => "[Stott.Security.Core]";

    public static int LogRetentionDays => 30;

    public static List<string> AllDirectives => new()
    {
        Directives.BaseUri,
        Directives.ChildSource,
        Directives.ConnectSource,
        Directives.DefaultSource,
        Directives.FontSource,
        Directives.FormAction,
        Directives.FrameAncestors,
        Directives.FrameSource,
        Directives.ImageSource,
        Directives.ManifestSource,
        Directives.MediaSource,
        Directives.NavigateTo,
        Directives.ObjectSource,
        Directives.PreFetchSource,
        Directives.RequireTrustedTypes,
        Directives.Sandbox,
        Directives.ScriptSourceAttribute,
        Directives.ScriptSourceElement,
        Directives.ScriptSource,
        Directives.StyleSourceAttribute,
        Directives.StyleSourceElement,
        Directives.StyleSource,
        Directives.TrustedTypes,
        Directives.UpgradeInsecureRequests,
        Directives.WorkerSource
    };

    public static List<string> AllSources => new()
    {
        Sources.Self,
        Sources.UnsafeEval,
        Sources.UnsafeInline,
        Sources.UnsafeHashes,
        Sources.None,
        Sources.SchemeBlob,
        Sources.SchemeData,
        Sources.SchemeFileSystem,
        Sources.SchemeHttp,
        Sources.SchemeHttps,
        Sources.SchemeMediaStream,
    };

    public static class Sources
    {
        public const string SchemeBlob = "blob:";

        public const string SchemeData = "data:";

        public const string SchemeFileSystem = "filesystem:";

        public const string SchemeHttp = "http:";

        public const string SchemeHttps = "https:";

        public const string SchemeMediaStream = "mediastream:";

        public const string Self = "'self'";

        public const string UnsafeEval = "'unsafe-eval'";

        public const string UnsafeHashes = "'unsafe-hashes'";

        public const string UnsafeInline = "'unsafe-inline'";

        public const string None = "'none'";
    }

    public static class Directives
    {
        public const string BaseUri = "base-uri";

        public const string ChildSource = "child-src";

        public const string ConnectSource = "connect-src";

        public const string DefaultSource = "default-src";

        public const string FontSource = "font-src";

        public const string FormAction = "form-action";

        public const string FrameAncestors = "frame-ancestors";

        public const string FrameSource = "frame-src";

        public const string ImageSource = "img-src";

        public const string ManifestSource = "manifest-src";

        public const string MediaSource = "media-src";

        public const string NavigateTo = "navigate-to";

        public const string ObjectSource = "object-src";

        public const string PreFetchSource = "prefetch-src";

        public const string RequireTrustedTypes = "require-trusted-types-for";

        public const string Sandbox = "sandbox";

        public const string ScriptSourceAttribute = "script-src-attr";

        public const string ScriptSourceElement = "script-src-elem";

        public const string ScriptSource = "script-src";

        public const string StyleSourceAttribute = "style-src-attr";

        public const string StyleSourceElement = "style-src-elem";

        public const string StyleSource = "style-src";

        public const string TrustedTypes = "trusted-types";

        public const string UpgradeInsecureRequests = "upgrade-insecure-requests";

        public const string WorkerSource = "worker-src";
    }

    public static class HeaderNames
    {
        public const string ReportOnlyContentSecurityPolicy = "Content-Security-Policy-Report-Only";

        public const string ContentSecurityPolicy = "Content-Security-Policy";

        public const string ContentTypeOptions = "X-Content-Type-Options";

        public const string XssProtection = "X-Xss-Protection";

        public const string ReferrerPolicy = "Referrer-Policy";

        public const string FrameOptions = "X-Frame-Options";

        public const string CrossOriginEmbedderPolicy = "Cross-Origin-Embedder-Policy";

        public const string CrossOriginOpenerPolicy = "Cross-Origin-Opener-Policy";

        public const string CrossOriginResourcePolicy = "Cross-Origin-Resource-Policy";
    }

    public static class CacheKeys
    {
        public const string CompiledCsp = "SSC_CompiledCsp";
    }
}
