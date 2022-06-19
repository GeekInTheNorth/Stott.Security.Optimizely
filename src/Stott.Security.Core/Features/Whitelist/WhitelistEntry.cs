using System.Collections.Generic;

namespace Stott.Security.Core.Features.Whitelist
{
    public class WhitelistEntry
    {
        public string SourceUrl { get; set; }

        public List<string> Directives { get; set; }
    }
}
