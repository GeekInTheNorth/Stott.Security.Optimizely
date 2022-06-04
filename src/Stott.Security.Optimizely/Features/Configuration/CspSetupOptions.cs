using System.Collections.Generic;

namespace Stott.Security.Optimizely.Features.Configuration
{
    public class CspSetupOptions
    {
        public CspSetupOptions()
        {
            AllowedRoles = new List<string> { "CmsAdmins", "Administrator" };
            ConnectionStringName = "EPiServerDB";
        }

        public string ConnectionStringName { get; set; }

        public List<string> AllowedRoles { get; set; }

        public bool UseWhitelist { get; set; }

        public string WhitelistUrl { get; set; }
    }
}
