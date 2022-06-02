namespace Stott.Security.Core.Features.Whitelist
{
    public interface ICspOptions
    {
        bool UseWhitelist { get; }

        string WhitelistUrl { get; }

        string ConnectionString { get; set; }

        string ConnectionStringName { get; }
    }
}
