namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public interface ICspWhitelistOptions
    {
        bool UseWhitelist { get; }

        string WhitelistUrl { get; }
    }
}
