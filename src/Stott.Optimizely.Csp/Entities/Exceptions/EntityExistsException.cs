using System;

namespace Stott.Optimizely.Csp.Entities.Exceptions
{
    public class EntityExistsException : Exception
    {
        public EntityExistsException(string message) : base(message)
        {
        }
    }
}
