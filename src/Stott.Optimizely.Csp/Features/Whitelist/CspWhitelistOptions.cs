namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public class CspWhitelistOptions : ICspWhitelistOptions
    {
        public bool UseWhitelist { get; set; }

        public string WhitelistUrl { get; set; }
    }
}
