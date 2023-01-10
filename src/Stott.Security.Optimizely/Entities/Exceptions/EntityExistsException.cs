namespace Stott.Security.Optimizely.Entities.Exceptions;

using System;

public sealed class EntityExistsException : Exception
{
    public EntityExistsException(string message) : base(message)
    {
    }
}