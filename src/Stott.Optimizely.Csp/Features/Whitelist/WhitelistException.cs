using System;

namespace Stott.Optimizely.Csp.Features.Whitelist
{
    public class WhitelistException : Exception
    {
        public WhitelistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
