using System;

using EPiServer.Logging;

using Stott.Security.Core.Features.Logging;

namespace Stott.Security.Optimizely.Features.Logging
{
    public class LoggingProviderFactory : ILoggingProviderFactory
    {
        public ILoggingProvider GetLogger(Type type)
        {
            return new LoggingProvider(LogManager.GetLogger(type));
        }
    }
}
