using System;

namespace Stott.Security.Optimizely.Entities.Exceptions
{
    public class EntityExistsException : Exception
    {
        public EntityExistsException(string message) : base(message)
        {
        }
    }
}
