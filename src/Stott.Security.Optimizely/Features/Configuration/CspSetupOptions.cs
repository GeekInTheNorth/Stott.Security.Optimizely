using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Configuration
{
    public class CspSetupOptions
    {
        public string ConnectionStringName { get; set; }

        public bool UseWhitelist { get; set; }

        public string WhitelistUrl { get; set; }
    }
}
