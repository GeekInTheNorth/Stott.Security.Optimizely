namespace Stott.Security.Core.Features.Whitelist
{
    public interface ICspWhitelistOptions
    {
        bool UseWhitelist { get; }

        string WhitelistUrl { get; }
    }
}
