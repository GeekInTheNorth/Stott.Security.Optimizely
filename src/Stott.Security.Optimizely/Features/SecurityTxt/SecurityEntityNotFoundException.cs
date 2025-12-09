using System;

namespace Stott.Security.Optimizely.Features.SecurityTxt;

[Serializable]
public class SecurityEntityNotFoundException : Exception
{
    public SecurityEntityNotFoundException()
    {
    }

    public SecurityEntityNotFoundException(Guid id)
        : base($"A security entry could not be found with the id of '{id}'")
    {
    }

    public SecurityEntityNotFoundException(string? message) : base(message)
    {
    }

    public SecurityEntityNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}