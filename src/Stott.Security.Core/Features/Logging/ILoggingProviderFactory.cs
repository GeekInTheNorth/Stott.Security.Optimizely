using System;

namespace Stott.Security.Core.Features.Logging
{
    public interface ILoggingProviderFactory
    {
        ILoggingProvider GetLogger(Type type);
    }
}
