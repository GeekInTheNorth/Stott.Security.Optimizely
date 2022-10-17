namespace Stott.Security.Optimizely.Common
{
    public class CspOptions
    {
        public bool UseWhitelist { get; set; }

        public string WhitelistUrl { get; set; }

        public string ConnectionStringName { get; set; }

        public string AllowedRoles { get; set; }
    }
}
