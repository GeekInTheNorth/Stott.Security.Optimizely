namespace Stott.Security.Core.Features.Whitelist
{
    public class CspWhiteListOptions : ICspWhitelistOptions
    {
        public bool UseWhitelist { get; set; }

        public string WhitelistUrl { get; set; }
    }
}
