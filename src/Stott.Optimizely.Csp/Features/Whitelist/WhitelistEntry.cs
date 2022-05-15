using System.Collections.Generic;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public class WhitelistEntry
    {
        public string SourceUrl { get; set; }

        public List<string> Directives { get; set; }
    }
}
