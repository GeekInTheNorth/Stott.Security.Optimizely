namespace Stott.Security.Optimizely.Common;

using System.Collections.Generic;

public static class CspConstants
{
    public const string ModuleName = "Stott.Security.Optimizely";

    public const string AuthorizationPolicy = "Stott:SecurityOptimizely:Policy";

    public static string LogPrefix => "[Stott.Security.Optimizely]";

    public const string CorsPolicy = "Stott:SecurityOptimizely:CORS";

    public const string NoncePlaceholder = "##NONCE##";

    public const string StrictDynamic = "'strict-dynamic'";

    public static int LogRetentionDays => 30;

    public const int TwoYearsInSeconds = 63072000;

    /// <summary>
    /// A collection of directives which can take URL style sources.
    /// </summary>
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
        Directives.ScriptSourceAttribute,
        Directives.ScriptSourceElement,
        Directives.ScriptSource,
        Directives.StyleSourceAttribute,
        Directives.StyleSourceElement,
        Directives.StyleSource,
        Directives.WorkerSource
    };

    public static List<string> NonceDirectives = new()
    {
        Directives.ScriptSourceElement,
        Directives.ScriptSource,
        Directives.StyleSourceElement,
        Directives.StyleSource,
    };

    public static List<string> AllSources => new()
    {
        Sources.Self,
        Sources.UnsafeEval,
        Sources.WebAssemblyUnsafeEval,
        Sources.UnsafeInline,
        Sources.UnsafeHashes,
        Sources.InlineSpeculationRules,
        Sources.None,
        Sources.SchemeBlob,
        Sources.SchemeData,
        Sources.SchemeFileSystem,
        Sources.SchemeHttp,
        Sources.SchemeHttps,
        Sources.SchemeWs,
        Sources.SchemeWss,
        Sources.SchemeMediaStream,
    };

    public static class Sources
    {
        public const string SchemeBlob = "blob:";

        public const string SchemeData = "data:";

        public const string SchemeFileSystem = "filesystem:";

        public const string SchemeHttp = "http:";

        public const string SchemeHttps = "https:";

        public const string SchemeWs = "ws:";

        public const string SchemeWss = "wss:";

        public const string SchemeMediaStream = "mediastream:";

        public const string Self = "'self'";

        public const string UnsafeEval = "'unsafe-eval'";

        public const string WebAssemblyUnsafeEval = "'wasm-unsafe-eval'";

        public const string UnsafeHashes = "'unsafe-hashes'";

        public const string UnsafeInline = "'unsafe-inline'";

        public const string InlineSpeculationRules = "'inline-speculation-rules'";

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

        // public const string RequireTrustedTypes = "require-trusted-types-for";

        public const string Sandbox = "sandbox";

        public const string ScriptSourceAttribute = "script-src-attr";

        public const string ScriptSourceElement = "script-src-elem";

        public const string ScriptSource = "script-src";

        public const string StyleSourceAttribute = "style-src-attr";

        public const string StyleSourceElement = "style-src-elem";

        public const string StyleSource = "style-src";

        // public const string TrustedTypes = "trusted-types";

        public const string UpgradeInsecureRequests = "upgrade-insecure-requests";

        public const string WorkerSource = "worker-src";
    }

    public static class HeaderNames
    {
        public const string ReportingEndpoints = "Reporting-Endpoints";

        public const string ReportOnlyContentSecurityPolicy = "Content-Security-Policy-Report-Only";

        public const string ContentSecurityPolicy = "Content-Security-Policy";

        public const string ContentTypeOptions = "X-Content-Type-Options";

        public const string XssProtection = "X-Xss-Protection";

        public const string ReferrerPolicy = "Referrer-Policy";

        public const string FrameOptions = "X-Frame-Options";

        public const string CrossOriginEmbedderPolicy = "Cross-Origin-Embedder-Policy";

        public const string CrossOriginOpenerPolicy = "Cross-Origin-Opener-Policy";

        public const string CrossOriginResourcePolicy = "Cross-Origin-Resource-Policy";

        public const string StrictTransportSecurity = "Strict-Transport-Security";
    }

    public static class CacheKeys
    {
        public const string CompiledHeaders = "StottSecurity_headers";
    }

    public static class RegexPatterns
    {
        public const string UrlDomain = "^([a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{3,}\\.{1}[a-z0-9\\/\\-\\._\\:\\*\\[\\]\\@]{2,})$";

        public const string UrlLocalHost = "^([a-z]{2,5}\\:{1}\\/\\/localhost\\:([0-9]{1,5}|\\*{1}))$";
    }
}