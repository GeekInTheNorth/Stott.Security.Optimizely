namespace Stott.Security.Optimizely.Attributes;

using System;

public class SecurityHeaderValueAttribute : Attribute
{
    public string SecurityHeaderValue { get; }

    public SecurityHeaderValueAttribute(string securityHeaderValue)
    {
        SecurityHeaderValue = securityHeaderValue;
    }
}
