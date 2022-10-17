using System;

using EPiServer.Logging;

using Stott.Security.Optimizely.Features.Logging;

namespace Stott.Security.Optimizely.Features.Logging
{
    public class LoggingProvider : ILoggingProvider
    {
        private readonly ILogger _logger;

        public LoggingProvider(ILogger logger)
        {
            _logger = logger;
        }

        public void Error(string errorMessage, Exception exception)
        {
            _logger?.Error(errorMessage, exception);
        }

        public void Information(string informationMessage)
        {
            _logger?.Information(informationMessage);
        }
    }
}
