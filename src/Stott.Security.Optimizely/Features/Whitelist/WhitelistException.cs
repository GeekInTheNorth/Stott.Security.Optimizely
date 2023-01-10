namespace Stott.Security.Optimizely.Features.Whitelist;

using System;

public sealed class WhitelistException : Exception
{
    public WhitelistException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}