using System;

namespace Stott.Security.Core.Entities.Exceptions
{
    public class EntityExistsException : Exception
    {
        public EntityExistsException(string message) : base(message)
        {
        }
    }
}
