namespace Stott.Security.Optimizely.Entities.Exceptions;

using System;

public class EntityExistsException : Exception
{
    public EntityExistsException(string message) : base(message)
    {
    }
}
