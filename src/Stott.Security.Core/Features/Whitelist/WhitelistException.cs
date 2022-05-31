using System;

namespace Stott.Security.Core.Features.Whitelist
{
    public class WhitelistException : Exception
    {
        public WhitelistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
