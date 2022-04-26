namespace Stott.Optimizely.Csp.Features.Configuration
{
    public interface ICspWhiteListOptions
    {
        public bool UseWhiteList { get; }

        public string WhiteListUrl { get; }
    }
}
