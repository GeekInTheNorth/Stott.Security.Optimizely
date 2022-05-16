namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public interface ICspWhitelistOptions
    {
        bool UseWhitelist { get; }

        string WhitelistUrl { get; }

        string ConnectionString { get; set; }

        string ConnectionStringName { get; }
    }
}
