namespace Stott.Optimizely.Csp.Features.Configuration
{
    public class CspWhiteListOptions : ICspWhiteListOptions
    {
        public bool UseWhiteList { get; set; }

        public string WhiteListUrl { get; set; }
    }
}
