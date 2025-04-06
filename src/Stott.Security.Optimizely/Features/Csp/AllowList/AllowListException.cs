namespace Stott.Security.Optimizely.Features.Csp.AllowList;

using System;

public sealed class AllowListException : Exception
{
    public AllowListException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}