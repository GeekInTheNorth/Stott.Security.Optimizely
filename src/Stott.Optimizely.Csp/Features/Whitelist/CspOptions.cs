namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public class CspOptions : ICspOptions
    {
        public bool UseWhitelist { get; set; }

        public string WhitelistUrl { get; set; }

        public string ConnectionString { get; set; }

        public string ConnectionStringName { get; set; }
    }
}
