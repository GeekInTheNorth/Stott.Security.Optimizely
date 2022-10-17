using System;

namespace Stott.Security.Optimizely.Features.Logging
{
    public interface ILoggingProvider
    {
        void Error(string errorMessage, Exception exception);

        void Information(string informationMessage);
    }
}
