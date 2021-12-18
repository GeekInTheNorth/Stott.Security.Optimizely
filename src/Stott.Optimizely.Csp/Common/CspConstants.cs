using System.Collections.Generic;

namespace Stott.Optimizely.Csp.Common
{
    public static class CspConstants
    {
        public static List<string> Directives => new List<string>
        {
            "base-uri",
            "child-src",
            "connect-src",
            "default-src",
            "font-src",
            "form-action",
            "frame-ancestors",
            "frame-src",
            "img-src",
            "manifest-src",
            "media-src",
            "navigate-to",
            "object-src",
            "prefetch-src",
            "require-trusted-types-for",
            "sandbox",
            "script-src-attr",
            "script-src-elem",
            "script-src",
            "style-src-attr",
            "style-src-elem",
            "style-src",
            "trusted-types",
            "upgrade-insecure-requests",
            "worker-src"
        };
    }
}
