using System;

namespace Stott.Security.Optimizely.Features.Logging
{
    public interface ILoggingProviderFactory
    {
        ILoggingProvider GetLogger(Type type);
    }
}
