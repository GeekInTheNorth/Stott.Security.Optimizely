namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public interface IWhitelistService
    {
        void AddToWhitelist(string violationSource, string directive);

        bool IsOnWhitelist(string violationSource, string directive);
    }
}
